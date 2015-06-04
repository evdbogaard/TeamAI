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

    //private List<Plan> m_offensivePlans;

    private Plan m_currentPlan;
    private Player m_support;

	// Use this for initialization
	void Start() 
    {
        m_fieldPlayers = new List<Player>();
        //m_offensivePlans = new List<Plan>();

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

                float startX = ((x + 5.5f) / 2.0f) - 5.5f;

                if (this.name.Contains("Red"))
                {
                    x = -x;
                    y = -y;
                    startX = -startX;

                    player.directOpponent = Global.CoachBlue.FieldPlayers[i];
                }

                //Vector3 testCoord = Camera.main.ScreenToWorldPoint(temp);
                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.m_startPosition = new Vector3(startX, -y, 0.0f);
                player.designation = pi.designation.ToString();
                m_fieldPlayers.Add(player);
            }
        }

        //for (int i = 0; i < m_fieldPlayers.Count; i++)
        //{
            //float distSqr = (m_fieldPlayers[i].transform.position - Global.sBall.transform.position).sqrMagnitude;
        //}
        if (name.Contains("Red"))
            Global.sBall.controller = m_fieldPlayers[3];
       
        // Test Plan
        /*Plan p = new Plan();
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
        m_offensivePlans.Add(p3);*/
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

                float startX = ((x + 5.5f) / 2.0f) - 5.5f;

                if (this.name.Contains("Red"))
                {
                    x = -x;
                    y = -y;
                    startX = -startX;
                }

                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.m_startPosition = new Vector3(startX, -y, 0.0f);
                player.designation = pi.designation.ToString();
            }
        }

        calculateInfluenceMaps();

        /*if (!m_fieldPlayers[0].m_runningGame)
            return;

        int stop = 0;
        //Debug.Log(Input.mousePosition);
        //addScoreToGrid(0, 1.0f);
        //addScoreToGrid(Global.GridSizeX + 1, 1.0f);
        //addScoreToGrid((Global.GridSizeY - 1) * Global.GridSizeX, 1.0f);

        if (teamControlsBall())
        {
            // Detect plan
            if (m_currentPlan == null)
            {
                m_currentPlan = selectPlan(out m_support);
            }
            else
            {
                Vector3 destination = m_support.m_planDesignation;// Global.PlanGrid[m_currentPlan.destinationPos].position;
                float supportTime = m_support.timeToReachPos(destination);
                float ballTime = Global.sBall.timeToReachTarget(destination, ((destination - Global.sBall.transform.position).normalized * 2.0f).magnitude);

                if (supportTime < ballTime)
                {
                    Global.sBall.kick(destination, 2.0f);
                }

                int ballId = Global.planGridId(Global.sBall.transform.position);
                int defenceId = ballId - 1;
                int closestDefender = 0;
                float closestDistance = float.MaxValue;
                for (int i = 0; i < m_fieldPlayers.Count; i++)
                {
                    if (m_fieldPlayers[i] == Global.sBall.controller)
                        continue;

                    if (m_fieldPlayers[i] == m_support)
                        continue;

                    if (m_fieldPlayers[i].designation == eDesignation.eDefender.ToString())
                    {
                        float dst = (m_fieldPlayers[i].transform.position - Global.PlanGrid[defenceId].position).sqrMagnitude;
                        if (dst < closestDistance)
                        {
                            closestDefender = i;
                            closestDistance = dst;
                        }
                    }
                }

                Vector3 newDest = Vector3.zero;
                if (getSafePointInGrid(defenceId, out newDest))
                {
                    m_fieldPlayers[closestDefender].m_strategyDestination = newDest;
                }
            }
        }
        else
        {
            // Defensive tactics
        }*/
	}

    public void setDirectOpponents()
    {
        List<Player> availableEnemyPlayers;
        if (name.Contains("Blue"))
            availableEnemyPlayers = new List<Player>(Global.CoachRed.FieldPlayers);
        else
            availableEnemyPlayers = new List<Player>(Global.CoachBlue.FieldPlayers);

        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            // get closest enemy player
            int id = 0;
            float closestDist = float.MaxValue;
            for (int j = 0; j < availableEnemyPlayers.Count; j++)
            {
                float dist = (m_fieldPlayers[i].transform.position - availableEnemyPlayers[j].transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    id = j;
                }
            }

            m_fieldPlayers[i].directOpponent = availableEnemyPlayers[id];
            availableEnemyPlayers.RemoveAt(id);
        }


    }

    public void setStartPositions()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            m_fieldPlayers[i].m_target = m_fieldPlayers[i].m_startPosition;
        }

        if (teamControlsBall())
        {
            for (int i = 0; i < m_fieldPlayers.Count; i++)
            {
                if (m_fieldPlayers[i].designation == "eAttacker")
                {
                    m_fieldPlayers[i].m_target = Global.sBall.transform.position;
                    Global.sBall.controller = m_fieldPlayers[i];
                    break;
                }
            }
        }
    }

    public bool teamInStartPosition()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if ((m_fieldPlayers[i].transform.position - m_fieldPlayers[i].m_target).sqrMagnitude > 0.1f)
                return false;
        }

        return true;
    }

    public int idToRed(int id)
    {
        if (name.Contains("Red"))
        {
            int newId = 0;
            int y = id / 4;
            int x = 3 - (id - (y * 4));

            return y * 4 + x;
        }
        return id;
    }

    public void calculateOffence()
    {
        int ballGridID = Global.planGridId(Global.sBall.transform.position);
        ballGridID = idToRed(ballGridID);
        if (ballGridID == 7)
        {
            // hardcoded shoot
            if (name.Contains("Red"))
                Global.sBall.controller.kickBall(Global.CoachBlue.goal.transform.position);
            else
                Global.sBall.controller.kickBall(Global.CoachRed.goal.transform.position);
        }

        if (m_currentPlan == null)
        {
            m_currentPlan = selectPlan(out m_support);
        }
        else
        {
            int stop = 0;

            Vector3 destination = m_support.m_planDesignation;
            float supportTime = m_support.timeToReachPos(destination);
            float ballTime = Global.sBall.timeToReachTarget(destination, 2.0f);

            if (supportTime < ballTime)
            {
                Global.sBall.controller.kickBall(destination);
                //Global.sBall.kick(destination, 2.0f);
            }
        }

        positionOffenceAttackers();
        positionOffenceMidfielders();
        positionOffenceDefenders();
        checkMultipleInSameGrid();
    }

    private void positionToId(Vector3 pos, out int x, out int y)
    {
        int id = Global.planGridId(pos);

        y = id / 4;
        x = id - (y * 4);

        if (name.Contains("Red"))
            x = 3 - x;
    }

    private void checkMultipleInSameGrid()
    {
        List<int> occupyList = new List<int>();
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if (m_fieldPlayers[i] == m_support || m_fieldPlayers[i] == Global.sBall.controller || m_fieldPlayers[i].designation != "eDefender")
                continue;

            Vector3 target = m_fieldPlayers[i].m_target;
            int x = 0;
            int y = 0;
            positionToId(target, out x, out y);
            int id = y * 4 + x;

            if (!occupyList.Contains(id))
            {
                occupyList.Add(id);
            }
            else
            {
                if (y == 0)
                    y += 1;
                else if (y == 2)
                    y -= 1;
                else
                {
                    System.Random r = new System.Random();
                    if (r.Next(0, 10) > 5)
                        y += 1;
                    else
                        y -= 1;
                }

                Vector3 newTarget;
                if (getSafePointInGrid(y * 4 + x, out newTarget))
                    m_fieldPlayers[i].m_target = newTarget;
            }
        }
    }

    private void positionOffenceDefenders()
    {
        int ballX;
        int ballY;
        positionToId(Global.sBall.transform.position, out ballX, out ballY);

        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if (m_fieldPlayers[i] == m_support || m_fieldPlayers[i] == Global.sBall.controller)// || m_fieldPlayers[i].designation != "eDefender")
                continue;

            Vector3 target = m_fieldPlayers[i].m_target;
            int x = 0;
            int y = 0;
            positionToId(target, out x, out y);

            int newGridId = 0;
            int newGridX = 0;
            int newGridY = 0;

            if (m_fieldPlayers[i].designation == "eDefender")
            {
                if (x == ballX - 1 && y == ballY ||
                    x == ballX - 1 && y == ballY + 1 ||
                    x == ballX - 1 && y == ballY - 1)
                    continue;

                newGridId = ballY * 4 + ballX - 1;
                newGridX = ballX - 1;
                newGridY = ballY;
            }
            else if (m_fieldPlayers[i].designation == "eMidfielder")
            {
                if (x == ballX && y == ballY - 1 ||
                    x == ballX && y == ballY + 1 ||
                    x == ballX + 1 && y == ballY - 1 ||
                    x == ballX + 1 && y == ballY ||
                    x == ballX + 1 && y == ballY + 1)
                    continue;

                //if (ballX == 0)
                    newGridId = y * 4 + x + 1;
                    newGridX = ballX + 1;
                    newGridY = ballY;
                //else if ((ballX == 1 || ballX == 2) && ballY )
                //newGridId = 0;
            }
            else if (m_fieldPlayers[i].designation == "eAttacker")
            {
                if (x >= ballX + 1)
                    continue;

                if (ballX == 2)
                {
                    newGridId = y * 4 + x + 1;
                    newGridX = ballX + 1;
                    newGridY = ballY;
                }
                else
                {
                    newGridId = y * 4 + x + 2;
                    newGridX = ballX + 2;
                    newGridY = ballY;
                }
            }

            if (name.Contains("Red"))
                newGridX = 3 - newGridX;

            Vector3 newTarget;
            if (getSafePointInGrid(newGridY * 4 + newGridX, out newTarget))
                m_fieldPlayers[i].m_target = newTarget;
        }
    }

    private void positionOffenceMidfielders()
    {

    }

    private void positionOffenceAttackers()
    {

    }

    public void calculateDefence()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if (m_fieldPlayers[i].directOpponent == Global.sBall.controller)
            {
                Vector3 target = m_fieldPlayers[i].directOpponent.transform.position - goal.transform.position;// -directOpponent.transform.position;
                float distance = target.magnitude;
                Vector3 targetN = target.normalized;

                m_fieldPlayers[i].m_target = goal.transform.position + targetN * distance * 0.5f;// moveTowards(coach.goal.transform.position + targetN * distance * 0.5f, 1.0f);
            }
            else
            {
                Vector3 target = Global.sBall.transform.position - m_fieldPlayers[i].directOpponent.transform.position;
                float distance = target.magnitude;
                Vector3 targetN = target.normalized;

                m_fieldPlayers[i].m_target = m_fieldPlayers[i].directOpponent.transform.position + targetN * distance * 0.35f;//

                //moveTowards(directOpponent.transform.position + targetN * distance * 0.35f, 1.0f);
            }
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
        m_support = null;
    }

    private Plan selectPlan(out Player supportPlayer)
    {
        // Find eligable plans;
        int ballId = Global.planGridId(Global.sBall.transform.position);
        ballId = idToRed(ballId);
        supportPlayer = Global.sBall.controller;

        // Find plan with correct BallID
        List<Plan> possiblePlans = new List<Plan>();
        for (int i = 0; i < Global.sPlans.Count; i++)
        {
            Plan p = Global.sPlans[i];
            if (p.ballPos != ballId)
                continue;

            if (!Global.sBall.controllerInRange())
                continue;

            possiblePlans.Add(p);

            // find teammate positions
            /*bool foundTeammate = false;
            int teamId;
            for (int t = 0; t < m_fieldPlayers.Count; t++)
            {
                if (m_fieldPlayers[t] == Global.sBall.controller)
                    continue;

                int tmp = Global.planGridId(m_fieldPlayers[t].transform.position);
                tmp = idToRed(tmp);
                if (p.teamPos == tmp)
                {
                    possiblePlans.Add(p);

                    Vector3 dest;
                    if (getSafePointInGrid(idToRed(p.destinationPos), out dest))
                    {
                        teamId = tmp;
                        foundTeammate = true;
                        supportPlayer = m_fieldPlayers[t];
                        supportPlayer.setSupportRole(dest);
                        //Debug.Log("Plan " + i);
                        break;
                    }
                }
            }*/

            //if (!foundTeammate)
            //    continue;

            //return p;
        }

        //bool foundTeammate = false;
        if (possiblePlans.Count != 0)
        {
            while (possiblePlans.Count != 0)
            {
                int result = Global.sRandom.Next(0, possiblePlans.Count - 1);
                Plan p2 = possiblePlans[result];

                int teamId;
                for (int t = 0; t < m_fieldPlayers.Count; t++)
                {
                    if (m_fieldPlayers[t] == Global.sBall.controller)
                        continue;

                    int tmp = Global.planGridId(m_fieldPlayers[t].transform.position);
                    tmp = idToRed(tmp);
                    if (p2.teamPos == tmp)
                    {
                        Vector3 dest;
                        if (getSafePointInGrid(idToRed(p2.destinationPos), out dest))
                        {
                            teamId = tmp;
                            //foundTeammate = true;
                            supportPlayer = m_fieldPlayers[t];
                            supportPlayer.setSupportRole(dest);
                            //Debug.Log("Plan " + i);
                            return p2;
                        }
                    }
                }
                possiblePlans.Remove(p2);
            }
        }

        return null;
    }

    private bool getSafePointInGrid(int id, out Vector3 pos)
    {
        pos = Vector3.zero;

        if (id < 0 || id >= 4 * 3)
            return false;

        List<int> strategyGridIds = Global.PlanGrid[id].ids;
        List<int> safeIds = new List<int>();
        for (int i = 0; i < strategyGridIds.Count; i++)
        {
            if (name.Contains("Blue"))
            {
                if (Global.Grid[strategyGridIds[i]].score >= 0.0f)
                    safeIds.Add(strategyGridIds[i]);
            }
            else
            {
                if (Global.Grid[strategyGridIds[i]].score <= 0.0f)
                    safeIds.Add(strategyGridIds[i]);
            }
        }

        if (safeIds.Count > 0)
        {
            System.Random r = new System.Random();
            int randomOutcome = r.Next(0, safeIds.Count - 1);
            pos = Global.Grid[safeIds[randomOutcome]].position;
            return true;
        }
        else
            return false;
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
