using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

public class Manager : MonoBehaviour 
{
    private List<Player> m_fieldPlayers;
    //private GridPoint[] m_influenceGrid

    private Formation m_currentFormation;
    public int selectedFormation = 0;
    private int oldSelectedFormation = 0;

    public GameObject goal;

    private List<Plan> m_offensivePlans;

    private Plan m_currentPlan;
    private Player m_support;

	// Use this for initialization
	void Start() 
    {
        m_fieldPlayers = new List<Player>();
        m_offensivePlans = new List<Plan>();

        m_currentPlan = null;

        m_currentFormation = Global.sFormations[0] as Formation;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Player player = this.transform.GetChild(i).GetComponent<Player>();
            if (player)
            {
                PlayerInfo pi = m_currentFormation.m_playersInfo[i] as PlayerInfo;
                Vector3 temp = pi.position - new Vector3(36.0f, 68.0f, 0.0f);
                temp.z = 0.0f;

                player.coach = this;

                // Field is 600 x 382
                // Ingame X: 36 - 700 or -6 6
                float x = (pi.position.x / 600.0f * 12.0f) - 6.0f;
                float y = (pi.position.y / 382.0f * 7.5f) - 3.75f;
                //x = (x / 600.0f * 12.0f) - 6.0f;
                //x = (x * 12.0f) - 6.0f;

                if (this.name.Contains("Red"))
                {
                    x = -x;
                    y = -y;

                    player.directOpponent = Global.CoachBlue.FieldPlayers[i];
                }

                //Vector3 testCoord = Camera.main.ScreenToWorldPoint(temp);
                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.designation = pi.designation.ToString();
                m_fieldPlayers.Add(player);
            }
        }

        //for (int i = 0; i < m_fieldPlayers.Count; i++)
        //{
            //float distSqr = (m_fieldPlayers[i].transform.position - Global.sBall.transform.position).sqrMagnitude;
        //}
        if (name.Contains("Blue"))
            Global.sBall.controller = m_fieldPlayers[3];
       
        // Test Plan
        Plan p = new Plan();
        p.ballPos = 6;
        p.teamPos = 7;
        p.opponentPos = 11;
        p.destinationPos = 3;

        m_offensivePlans.Add(p);

        Plan p2 = new Plan();
        p2.ballPos = 5;
        p2.teamPos = 1;
        p2.opponentPos = 11;
        p2.destinationPos = 2;
        m_offensivePlans.Add(p2);

        Plan p4 = new Plan();
        p4.ballPos = 2;
        p4.teamPos = 7;
        p4.opponentPos = 11;
        p4.destinationPos = 3;
        m_offensivePlans.Add(p4);

        Plan p3 = new Plan();
        p3.ballPos = 3;
        p3.teamPos = 6;
        p3.opponentPos = 11;
        p3.destinationPos = 7;
        m_offensivePlans.Add(p3);
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (selectedFormation != oldSelectedFormation)
        {
            oldSelectedFormation = selectedFormation;
            m_currentFormation = Global.sFormations[selectedFormation] as Formation;
            for (int i = 0; i < m_currentFormation.m_playersInfo.Count; i++)
            {
                PlayerInfo pi = m_currentFormation.m_playersInfo[i] as PlayerInfo;
                Player player = m_fieldPlayers[i] as Player;
                float x = (pi.position.x / 600.0f * 12.0f) - 6.0f;
                float y = (pi.position.y / 382.0f * 7.5f) - 3.75f;

                if (this.name.Contains("Red"))
                {
                    x = -x;
                    y = -y;
                }

                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.designation = pi.designation.ToString();
            }
        }

        int stop = 0;
        //Debug.Log(Input.mousePosition);
        //addScoreToGrid(0, 1.0f);
        //addScoreToGrid(Global.GridSizeX + 1, 1.0f);
        //addScoreToGrid((Global.GridSizeY - 1) * Global.GridSizeX, 1.0f);

        calculateInfluenceMaps();

        if (teamControlsBall())
        {
            // Detect plan
            if (m_currentPlan == null)
            {
                m_currentPlan = selectPlan(out m_support);
            }
            else
            {
                Vector3 destination = Global.PlanGrid[m_currentPlan.destinationPos].position;
                float supportTime = m_support.timeToReachPos(destination);
                float ballTime = Global.sBall.timeToReachTarget(destination, ((destination - Global.sBall.transform.position).normalized * 2.0f).magnitude);

                if (supportTime < ballTime)
                {
                    Global.sBall.kick(destination, 2.0f);
                }

                //Debug.Log("Support: " + supportTime.ToString());
                //Debug.Log("Ball: " + ballTime.ToString());
            }
        }
        else
        {
            // Defensive tactics
        }
	}

    public bool teamControlsBall()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if (m_fieldPlayers[i] == Global.sBall.controller)
                return true;
        }
        return false;
    }

    public void newBallHolder()
    {
        m_currentPlan = null;
    }

    private Plan selectPlan(out Player supportPlayer)
    {
        // Find eligable plans;
        int ballId = Global.planGridId(Global.sBall.transform.position);
        supportPlayer = Global.sBall.controller;

        // Find plan with correct BallID
        for (int i = 0; i < m_offensivePlans.Count; i++)
        {
            Plan p = m_offensivePlans[i];
            if (p.ballPos != ballId)
                continue;

            if (!Global.sBall.controllerInRange())
                continue;

            // find teammate positions
            bool foundTeammate = false;
            int teamId;
            for (int t = 0; t < m_fieldPlayers.Count; t++)
            {
                if (m_fieldPlayers[t] == Global.sBall.controller)
                    continue;

                int tmp = Global.planGridId(m_fieldPlayers[t].transform.position);
                if (p.teamPos == tmp)
                {
                    teamId = tmp;
                    foundTeammate = true;
                    supportPlayer = m_fieldPlayers[t];
                    supportPlayer.setSupportRole(Global.PlanGrid[p.destinationPos].position);
                    break;
                }
            }

            if (!foundTeammate)
                continue;

            return p;
        }

        return null;
    }

    private void calculateInfluenceMaps()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            int id = getGridID(m_fieldPlayers[i]);

            addScoreToGrid(id, 1.0f);

            addScoreToGrid(id + 1, 0.5f);
            addScoreToGrid(id - 1, 0.5f);
            addScoreToGrid(id + Global.GridSizeX, 0.5f);
            addScoreToGrid(id - Global.GridSizeX, 0.5f);
            addScoreToGrid(id + Global.GridSizeX + 1, 0.5f);
            addScoreToGrid(id + Global.GridSizeX - 1, 0.5f);
            addScoreToGrid(id - Global.GridSizeX + 1, 0.5f);
            addScoreToGrid(id - Global.GridSizeX - 1, 0.5f);
        }
    }

    private void addScoreToGrid(int id, float score)
    {
        if (id < 0 || id >= Global.GridSizeX * Global.GridSizeY)
            return;

        float currentScore = Global.Grid[id].score;
        float addedValue = score;

        if (this.name.Contains("Red"))
            addedValue *= -1.0f;

        float finalScore = currentScore + addedValue;
        if (finalScore > 0.0f)
            finalScore = Mathf.Min(1.0f, finalScore);
        else
            finalScore = Mathf.Max(-1.0f, finalScore);

        Global.Grid[id].score = finalScore;
    }

    private void addScoreToSurroundingGrids(int id, float score, int range)
    {

    }

    private int getGridID(Player player)
    {
        for (int i = 0; i < Global.GridSizeX * Global.GridSizeY; i++)
        {
            GridPoint gp = Global.Grid[i];
            if (player.transform.position.x > gp.min.x && player.transform.position.x < gp.max.x &&
                player.transform.position.y > gp.min.y && player.transform.position.y < gp.max.y)
            {
                return i;
            }
        }
        return 0;
    }

    public List<Player> FieldPlayers
    {
        get { return m_fieldPlayers; }
    }
}
