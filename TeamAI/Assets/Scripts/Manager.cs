using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

public class Statistics
{
    public int passes;
    public int passesReceived;
    public float timeInPossession;
    public int foulsMade;

    public Statistics()
    {
        passes = 0;
        passesReceived = 0;
        timeInPossession = 0;
        foulsMade = 0;
    }
}

public class Manager : MonoBehaviour 
{
    private List<Player> m_fieldPlayers;
    //private GridPoint[] m_influenceGrid

    private Formation m_currentFormation;
    private Strategy m_currentStrategy;
    public int selectedFormation = 0;
    private int oldSelectedFormation = 0;

    public int strategyNumber = 0;

    public Statistics m_statistics;

    public GameObject goal;

    bool passCounts = true;

    //private List<Plan> m_offensivePlans;

    private Plan m_currentPlan;
    private Player m_support;

    private Manager enemy;

	// Use this for initialization
	void Start() 
    {
        m_fieldPlayers = new List<Player>();
        //m_offensivePlans = new List<Plan>();
        m_statistics = new Statistics();

        m_currentPlan = null;
        passCounts = true;

        m_currentFormation = Global.sFormations[selectedFormation] as Formation;
        m_currentStrategy = Global.sStrategies[strategyNumber];

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
                    enemy = Global.CoachBlue;
                }
                else
                    enemy = Global.CoachRed;

                //Vector3 testCoord = Camera.main.ScreenToWorldPoint(temp);
                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.m_startPosition = new Vector3(startX, -y, 0.0f);
                player.designation = pi.designation.ToString();
                m_fieldPlayers.Add(player);
            }
        }

        if (name.Contains("Red"))
            Global.sBall.controller = m_fieldPlayers[3];

        //setDirectOpponents();
	}

    public void setDirectOpponents()
    {
        List<int> takenEnemies = new List<int>();
        for (int i = 0; i < m_currentStrategy.m_personal.Count; i++)
        {
            PersonalBehavior pb = m_currentStrategy.m_personal[i];
            if (pb.behavior != 0)
                continue;

            int role = pb.typeToDefend;
            switch (role)
            {
                case 0:
                    List<int> defenders = enemy.getDefenders();
                    for (int j = 0; j < defenders.Count; j++)
                    {
                        if (takenEnemies.Contains(defenders[j]))
                            continue;

                        m_fieldPlayers[i].directOpponent = enemy.FieldPlayers[defenders[j]];
                        takenEnemies.Add(defenders[j]);
                        break;
                    }
                    break;
                case 1:
                    List<int> midfielders = enemy.getMidfielders();
                    for (int j = 0; j < midfielders.Count; j++)
                    {
                        if (takenEnemies.Contains(midfielders[j]))
                            continue;

                        m_fieldPlayers[i].directOpponent = enemy.FieldPlayers[midfielders[j]];
                        takenEnemies.Add(midfielders[j]);
                        break;
                    }
                    break;
                case 2:
                    List<int> attackers = enemy.getAttackers();
                    for (int j = 0; j < attackers.Count; j++)
                    {
                        if (takenEnemies.Contains(attackers[j]))
                            continue;

                        m_fieldPlayers[i].directOpponent = enemy.FieldPlayers[attackers[j]];
                        takenEnemies.Add(attackers[j]);
                        break;
                    }
                    break;
                default:
                    break;
            }

            if (m_fieldPlayers[i].directOpponent == null)
            {
                for (int j = enemy.FieldPlayers.Count-1; j >= 0; j--)
                {
                    if (takenEnemies.Contains(j))
                        continue;

                    m_fieldPlayers[i].directOpponent = enemy.FieldPlayers[j];
                    takenEnemies.Add(j);
                }
            }
            
        }
    }

    public List<int> getDefenders()
    {
        List<int> retVal = new List<int>();
        for (int i = 0; i < m_currentFormation.m_playersInfo.Count; i++)
        {
            PlayerInfo pi = m_currentFormation.m_playersInfo[i];
            if (pi.designation.ToString() == "eDefender")
                retVal.Add(i);
        }

        return retVal;
    }

    public List<int> getMidfielders()
    {
        List<int> retVal = new List<int>();
        for (int i = 0; i < m_currentFormation.m_playersInfo.Count; i++)
        {
            PlayerInfo pi = m_currentFormation.m_playersInfo[i];
            if (pi.designation.ToString() == "eMidfielder")
                retVal.Add(i);
        }

        return retVal;
    }

    public List<int> getAttackers()
    {
        List<int> retVal = new List<int>();
        for (int i = 0; i < m_currentFormation.m_playersInfo.Count; i++)
        {
            PlayerInfo pi = m_currentFormation.m_playersInfo[i];
            if (pi.designation.ToString() == "eAttacker")
                retVal.Add(i);
        }

        return retVal;
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
        else if (m_currentPlan == null)
        {
            m_currentPlan = selectPlan(out m_support);
        }
        else
        {
            Vector3 destination = m_support.m_planDesignation;
            float supportTime = m_support.timeToReachPos(destination);
            float ballTime = Global.sBall.timeToReachTarget(destination, 2.0f);

            Debug.DrawLine(m_support.transform.position, destination);
            Debug.DrawLine(Global.sBall.transform.position, destination);

            if (supportTime < ballTime)
            {
                if (passCounts && Global.sBall.controllerInRange())
                {
                    passCounts = false;
                    m_statistics.passes += 1;
                }
                else if (!Global.sBall.controllerInRange())
                {
                    passCounts = true;
                }

                Global.sBall.controller.kickBall(destination);
                //Global.sBall.kick(destination, 2.0f);
            }
        }

        positionOffence();
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

    private bool isGridFree(int testId)
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            int id = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));
            if (id == testId)
                return false;
        }

        return true;
    }

    private void checkMultipleInSameGrid()
    {
        List<Player> defenders = new List<Player>();
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            if (m_fieldPlayers[i].designation == "eDefender")
                defenders.Add(m_fieldPlayers[i]);
        }

        int ballX;
        int ballY;
        positionToId(Global.sBall.transform.position, out ballX, out ballY);

        Strategy s = new Strategy();
        int defenseLine = s.getDefenseLine(ballX, ballY);

        List<int> occupyList = new List<int>();
        List<Player> redo = new List<Player>();
        for (int i = 0; i < defenders.Count; i++)
        {
            int id = idToRed(Global.planGridId(defenders[i].m_target));
            if (occupyList.Contains(id))
            {
                int x;
                int y;
                positionToId(defenders[i].m_target, out x, out y);
                y = 0;

                bool foundNewGrid = false;
                while (!isGridFree(y * 3 + x))
                {
                    y++;
                    if (y == 3)
                    {
                        y = 0;
                        x++;
                    }

                    if (x == 4)
                        break;
                }

                Vector3 newTarget;
                if (getSafePointInGrid(idToRed(y * 4 + x), out newTarget))
                {
                    occupyList.Add(y * 3 + x);
                    defenders[i].m_target = newTarget;
                }
            }
            else
                occupyList.Add(id);
        }

        /*for (int i = 0; i < m_fieldPlayers.Count; i++)
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


                /*if (y == 0)
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
                }*/

           /*     Vector3 newTarget;
                if (getSafePointInGrid(idToRed(y * 4 + x), out newTarget))
                    m_fieldPlayers[i].m_target = newTarget;
            }
        }*/
    }

    void OnGUI()
    {
        float x = 10.0f;
        if (name.Contains("Red"))
            x = 600.0f;

        GUI.Label(new Rect(x, 10, 100, 100), "Fouls made: " + m_statistics.foulsMade.ToString());
        GUI.Label(new Rect(x, 30, 100, 100), "passes: " + m_statistics.passes.ToString());
        GUI.Label(new Rect(x, 50, 100, 100), "passes arrived: " + m_statistics.passesReceived.ToString());
        GUI.Label(new Rect(x, 70, 100, 100), "possession: " + m_statistics.timeInPossession.ToString());
    }

    private void positionOffence()
    {
        int ballX;
        int ballY;
        positionToId(Global.sBall.transform.position, out ballX, out ballY);

        List<int> validDef = m_currentStrategy.validDefenseLineIDs(ballX, ballY);
        List<int> validMid = m_currentStrategy.validMidLineIDs(ballX, ballY);
        List<int> validAtt = m_currentStrategy.validAttackLineIDs(ballX, ballY);
        for (int i = 0; i < validDef.Count; i++)
        {
            if (validMid.Contains(validDef[i]))
                validMid.Remove(validDef[i]);
        }
        for (int i = 0; i < validAtt.Count; i++)
        {
            if (validMid.Contains(validAtt[i]))
                validMid.Remove(validAtt[i]);
        }
        //int defenseLine = s.getDefenseLine(ballX, ballY);

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
                if (validDef.Count == 0)
                    continue;

                int targetID = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));
                if (validDef.Contains(targetID))
                {
                    validDef.Remove(targetID);
                    continue;
                }

                newGridId = validDef[0];
                validDef.RemoveAt(0);
            }
            else if (m_fieldPlayers[i].designation == "eMidfielder")
            {
                if (validMid.Count == 0)
                    continue;

                int targetID = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));
                if (validMid.Contains(targetID))
                {
                    validMid.Remove(targetID);
                    continue;
                }

                newGridId = validMid[0];
                validMid.RemoveAt(0);
            }
            else if (m_fieldPlayers[i].designation == "eAttacker")
            {
                if (validAtt.Count == 0)
                    continue;

                int targetID = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));
                if (validAtt.Contains(targetID))
                {
                    validAtt.Remove(targetID);
                    continue;
                }

                newGridId = validAtt[0];
                validAtt.RemoveAt(0);
            }

            Vector3 newTarget;
            if (getSafePointInGrid(idToRed(newGridId), out newTarget))
                m_fieldPlayers[i].m_target = newTarget;
        }
    }

    public void calculateDefence()
    {
        /*for (int i = 0; i < m_fieldPlayers.Count; i++)
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
        }*/

        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            PersonalBehavior pb = m_currentStrategy.m_personal[i];

            switch (pb.behavior)
            {
                case 0:
                    Vector3 target = m_fieldPlayers[i].directOpponent.transform.position - goal.transform.position;;
                    float distance = target.magnitude;
                    Vector3 targetN = target.normalized;
                    m_fieldPlayers[i].m_target = m_fieldPlayers[i].m_target = goal.transform.position + targetN * distance * 0.75f;
                    break;
                case 1:
                    // zone
                    int zoneID = idToRed(pb.zoneID);
                    int playerTargetID = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));

                    if (zoneID != playerTargetID)
                    {
                        m_fieldPlayers[i].m_target = Global.PlanGrid[zoneID].position;
                    }
                    else
                    {
                        for (int j = 0; j < enemy.FieldPlayers.Count; j++)
                        {
                            int enemyID = idToRed(Global.planGridId(enemy.FieldPlayers[j].transform.position));
                            if (enemyID == zoneID)
                            {
                                Vector3 target2 = Global.sBall.transform.position - enemy.FieldPlayers[j].transform.position;
                                float distance2 = target2.magnitude;
                                Vector3 targetN2 = target2.normalized;

                                m_fieldPlayers[i].m_target = enemy.FieldPlayers[j].transform.position + targetN2 * distance2 * 0.35f;
                            }
                        }
                    }
                    break;
                case 2:
                    if (m_currentStrategy.attackLine == 0)
                    {
                        int x;
                        int y;
                        positionToId(Global.sBall.transform.position, out x, out y);

                        if (x >= 2)
                            m_fieldPlayers[i].m_target = Global.sBall.transform.position;
                        else
                        {
                            int targetID = idToRed(Global.planGridId(m_fieldPlayers[i].m_target));
                            if (targetID != 6)
                            {
                                Vector3 newTarget;
                                if (getSafePointInGrid(idToRed(6), out newTarget))
                                {
                                    m_fieldPlayers[i].m_target = newTarget;
                                }
                            }
                        }
                    }
                    else
                        if ((Global.sBall.transform.position - m_fieldPlayers[i].transform.position).sqrMagnitude > 1.0f)
                            m_fieldPlayers[i].m_target = Global.sBall.transform.position;
                        else
                            m_fieldPlayers[i].m_target = m_fieldPlayers[i].transform.position;
                    break;
                default:
                    break;
            }

            /*if (tmp.shouldDefend[i])
            {
                Vector3 target = m_fieldPlayers[i].directOpponent.transform.position - goal.transform.position;
                float distance = target.magnitude;
                Vector3 targetN = target.normalized;

                m_fieldPlayers[i].m_target = goal.transform.position + targetN * distance * 0.75f;

                if (idToRed(Global.planGridId(m_fieldPlayers[i].m_target)) == 4)
                {
                    target = Global.sBall.transform.position - m_fieldPlayers[i].directOpponent.transform.position;
                    distance = target.magnitude;
                    targetN = target.normalized;

                    m_fieldPlayers[i].m_target = m_fieldPlayers[i].directOpponent.transform.position + targetN * distance * 0.35f;
                }

                //positionDefencively(m_fieldPlayers[i]);

            }
            else
            {
                positionPlayer(m_fieldPlayers[i]);
            }*/
        }

        checkMultipleInSameGrid();
    }

    public void positionDefencively(Player player)
    {
        int ballX;
        int ballY;
        positionToId(Global.sBall.transform.position, out ballX, out ballY);

        int ballID = idToRed(Global.planGridId(Global.sBall.transform.position));

        int newX = 0;
        int newY = 0;
        switch (player.designation)
        {
            case "eDefender":
                newX = ballX - 1;
                newY = ballY;

                if (ballID == 2 || ballID == 3 || ballID == 6 || ballID == 7 || ballID == 10 || ballID == 11)
                {
                    if (player.lastRefresh > 1.0f)
                    {
                        player.lastRefresh = 0.0f;
                    }
                    else
                    {
                        // position defender on own field close to line
                        int playerId = idToRed(Global.planGridId(player.m_target));
                        if (playerId == 0 || playerId == 1 || playerId == 4 || playerId == 5 || playerId == 8 || playerId == 9)
                        {
                            return;
                        }
                    }

                    newX = ballX - 1;
                    newY = ballY;
                }

                break;
            case "eMidfielder":
                break;
            case "eAttacker":
                break;
            default:
                break;
        }

        Vector3 newTarget;
        if (getSafePointInGrid(idToRed(newY * 4 + newX), out newTarget))
            player.m_target = newTarget;
    }

    public void positionPlayer(Player player)
    {
        int ballX;
        int ballY;
        positionToId(Global.sBall.transform.position, out ballX, out ballY);

        int newX = 0;
        int newY = 0;
        switch (player.designation)
        {
            case "eDefender":
                break;
            case "eMidfielder":
                break;
            case "eAttacker":
                newY = ballY;
                newX = ballX + 2;

                if (ballX >= 2)
                {
                    player.m_target = Global.sBall.transform.position;
                    return;
                }

                break;
            default:
                break;
        }

        Vector3 newTarget;
        if (getSafePointInGrid(idToRed(newY * 4 + newX), out newTarget))
            player.m_target = newTarget;
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
        Global.sBall.controller.m_target = Global.sBall.transform.position;
        m_currentPlan = null;
        m_support = null;

        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            m_fieldPlayers[i].m_usingPlan = false;
        }
    }

    private Plan selectPlan(out Player supportPlayer)
    {
        int ballId = Global.planGridId(Global.sBall.transform.position);
        ballId = idToRed(ballId);

        // Find eligable plans;
        
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
        }

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
                        // calculate field control level
                        float score = 0.0f;
                        for (int i = 0; i < Global.PlanGrid[idToRed(p2.destinationPos)].ids.Count; i++)
                        {
                            score += Global.Grid[Global.PlanGrid[idToRed(p2.destinationPos)].ids[i]].score;
                        }

                        if (name.Contains("Red"))
                            score *= -1.0f;

                        float risk = System.Convert.ToSingle(m_currentStrategy.risk);
                        risk = risk / 100.0f * 2.0f - 1.0f;

                        if (score >= risk)
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
                }
                possiblePlans.Remove(p2);
            }
        }

        possiblePlans = new List<Plan>(Global.sRelativePlans);
        while (possiblePlans.Count != 0)
        {
            int result = Global.sRandom.Next(0, possiblePlans.Count - 1);
            Plan p = possiblePlans[result];

            for (int j = 0; j < m_fieldPlayers.Count; j++)
            {
                if (m_fieldPlayers[j] == Global.sBall.controller)
                    continue;

                int tmp = idToRed(Global.planGridId(m_fieldPlayers[j].transform.position));
                int newTeamPos = ballId + p.teamPos;
                if (newTeamPos >= 3 * 4 || newTeamPos < 0)
                    continue;
                if (tmp == ballId + p.teamPos)
                {
                    int newTeamDest = ballId + p.destinationPos;
                    if (newTeamDest >= 3 * 4 || newTeamDest < 0)
                        continue;
                    // calculate field control level
                    float score = 0.0f;
                    for (int i = 0; i < Global.PlanGrid[idToRed(ballId + p.destinationPos)].ids.Count; i++)
                    {
                        score += Global.Grid[Global.PlanGrid[idToRed(ballId + p.destinationPos)].ids[i]].score;
                    }

                    if (name.Contains("Red"))
                        score *= -1.0f;

                    float risk = System.Convert.ToSingle(m_currentStrategy.risk);
                    risk = risk / 100.0f * 2.0f - 1.0f;

                    if (score >= risk)
                    {
                        Vector3 dest;
                        int destinationId = ballId + p.destinationPos;
                        if (getSafePointInGrid(idToRed(destinationId), out dest))
                        {
                            //foundTeammate = true;
                            supportPlayer = m_fieldPlayers[j];
                            supportPlayer.setSupportRole(dest);
                            //Debug.Log("Plan " + i);
                            return p;
                        }
                    }
                }
            }
            possiblePlans.Remove(p);
        }

        // Relative plan test
        /*for (int i = 0; i < Global.sRelativePlans.Count; i++)
        {
            Plan p = Global.sRelativePlans[i];

            for (int j = 0; j < m_fieldPlayers.Count; j++)
            {
                if (m_fieldPlayers[j] == Global.sBall.controller)
                    continue;

                int tmp = idToRed(Global.planGridId(m_fieldPlayers[j].transform.position));
                if (tmp == ballId + p.teamPos)
                {
                    Vector3 dest;
                    int destinationId = ballId + p.destinationPos;
                    if (getSafePointInGrid(idToRed(destinationId), out dest))
                    {
                        //foundTeammate = true;
                        supportPlayer = m_fieldPlayers[j];
                        supportPlayer.setSupportRole(dest);
                        //Debug.Log("Plan " + i);
                        return p;
                    }
                }
            }
        }*/

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
            //System.Random r = new System.Random();
            int randomOutcome = Global.sRandom.Next(0, safeIds.Count - 1);
            pos = Global.Grid[safeIds[randomOutcome]].position;

            float dst = (pos - Global.sBall.transform.position).sqrMagnitude;

            int attempts = 0;

            while (attempts < 10 && (pos - Global.sBall.transform.position).sqrMagnitude < 5.0f)
            {
                attempts++;
                randomOutcome = Global.sRandom.Next(0, safeIds.Count - 1);
                pos = Global.Grid[safeIds[randomOutcome]].position;
                dst = (pos - Global.sBall.transform.position).sqrMagnitude;
                int stop = 0;
            }

            if (attempts < 10)
            {
                //Debug.Log(dst);
                return true;
            }
            else return false;
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
