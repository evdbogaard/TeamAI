using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

public class Player : MonoBehaviour 
{
    public Vector3 m_idlePosition;
    public Vector3 m_startPosition;
    public string designation;

    public Vector3 m_planDesignation;
    public bool m_usingPlan;

    public Vector3 m_strategyDestination;
    public bool m_runningGame;

    public Player directOpponent;

    public Vector3 m_target;
    public float lastRefresh;

    float m_normalSpeed = 1.0f;
    float m_sprintSpeed = 1.5f;

    float maxSpeed = 1.0f;
    Vector3 m_velocity;

    public Manager coach;

    public float downTime;

	// Use this for initialization
	void Start() 
    {
        m_velocity = Vector3.zero;
        m_usingPlan = false;
        downTime = 0.0f;

        m_runningGame = false;
        lastRefresh = 0.0f;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (downTime > 0.0f)
            downTime -= Time.deltaTime;

        lastRefresh += Time.deltaTime;

        //if (Global.sBall.controller == this)
        //    moveTowards(Global.sBall.transform.position, m_normalSpeed);
        if (m_usingPlan)
            moveTowards(m_target, m_sprintSpeed);
        else
            moveTowards(m_target, m_normalSpeed);

        /*if (!m_runningGame)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                m_runningGame = true;

            m_strategyDestination = m_idlePosition;

            moveTowards(m_startPosition, 1.0f);
            return;
        }

        //this.transform.position = m_idlePosition;
        if (coach.teamControlsBall())
        {
            if (m_usingPlan)
            {
                moveTowards(m_planDesignation, 1.5f);
            }
            else if (Global.sBall.controller != this)
            {
                moveTowards(m_strategyDestination, 1.0f);
            }
            else
            {
                if (!Global.sBall.controllerInRange())
                {
                    moveTowards(Global.sBall.transform.position, 1.0f);
                }
                else if (Global.planGridId(Global.sBall.transform.position) == 7)
                {
                    //Debug.Log("Shot at goal");
                    Global.sBall.kick(GameObject.Find("Goal_Red").transform.position, 2.0f);
                }
            }
        }
        else
        {
            if (directOpponent == Global.sBall.controller)
            {
                Vector3 target = directOpponent.transform.position - coach.goal.transform.position;// -directOpponent.transform.position;
                float distance = target.magnitude;
                Vector3 targetN = target.normalized;

                moveTowards(coach.goal.transform.position + targetN * distance * 0.5f, 1.0f);
            }
            else
            {
                Vector3 target = Global.sBall.transform.position - directOpponent.transform.position;
                float distance = target.magnitude;
                Vector3 targetN = target.normalized;

                moveTowards(directOpponent.transform.position + targetN * distance * 0.35f, 1.0f);
            }
        }*/
	}

    public void kickBall(Vector3 ballTarget)
    {
        if (Global.sBall.controllerInRange())
        {
            Vector3 dir = (ballTarget - Global.sBall.transform.position);
            Vector3 dirN = dir.normalized;

            Quaternion rotationTop = Quaternion.Euler(0.0f, 0.0f, 10.0f);
            Quaternion rotationBottom = Quaternion.Euler(0.0f, 0.0f, -10.0f);
            Vector3 dirTop = rotationTop * dir;
            Vector3 dirBot = rotationBottom * dir;

            // Check direct interception possibility
            RaycastHit2D hit = Physics2D.Raycast(Global.sBall.transform.position, dirN, dir.magnitude);
            RaycastHit2D hit2 = Physics2D.Raycast(Global.sBall.transform.position, dirTop.normalized, dir.magnitude);
            RaycastHit2D hit3 = Physics2D.Raycast(Global.sBall.transform.position, dirBot.normalized, dir.magnitude);
            //Debug.DrawLine(Global.sBall.transform.position, Global.sBall.transform.position + dir);
            //Debug.DrawLine(Global.sBall.transform.position, Global.sBall.transform.position + dirTop.normalized * dir.magnitude);
            //Debug.DrawLine(Global.sBall.transform.position, Global.sBall.transform.position + dirBot.normalized * dir.magnitude);
            //RaycastHit2D hit = Physics2D.Raycast(Global.sBall.transform.position, Vector2.right);
            if (hit || hit2 || hit3)
            {
                if (detectHit(hit) || detectHit(hit2) || detectHit(hit3))
                    return;
            }
            Global.sBall.kick(ballTarget, 2.0f);
        }
    }

    private bool detectHit(RaycastHit2D hit)
    {
        if (hit)
        {
            if (hit.collider.gameObject.name != this.name)
            {
                //Debug.Log(hit.collider.gameObject.name);
                this.coach.newBallHolder();
                return true;
            }
        }

        return false;
    }

    public void setSupportRole(Vector3 destination)
    {
        //Debug.Log(timeToReachPos(destination));
        m_usingPlan = true;
        m_planDesignation = destination;
        m_target = destination;
    }

    public float timeToReachPos(Vector3 pos)
    {
        float distance = (pos - this.transform.position).magnitude;

        float currentSpeed = (m_usingPlan) ? m_sprintSpeed : m_normalSpeed;

        float time = distance / currentSpeed;
        
        return time;
    }

    void moveTowards(Vector3 target, float currentSpeed)
    {
        Vector3 toTarget = (target - this.transform.position);
        float distSqr = toTarget.sqrMagnitude;

        if (distSqr > 0.05f)
        {
            Vector3 desiredVelocity = toTarget.normalized;
            m_velocity = (desiredVelocity - m_velocity);

            Vector3 pos = this.transform.position;
            Vector3 ahead = pos + (target - pos).normalized * 1.0f; // MAX_SEE_AHEAD
            Vector3 ahead2 = pos + (target - pos).normalized * 0.5f;
            Vector3 ahead3 = pos + (target - pos).normalized * 0.1f;

            //Debug.DrawLine(pos, ahead2);

            List<Player> obstacles = new List<Player>();
            //if (coach.name.Contains("Red"))
                obstacles.AddRange(Global.CoachBlue.FieldPlayers);
            //else
                obstacles.AddRange(Global.CoachRed.FieldPlayers);

            int mostDangerousOpponent = -1;
            for (int i = 0; i < obstacles.Count; i++)
            {
                Vector3 opp = obstacles[i].transform.position;

                float dist1 = (opp - ahead).magnitude;
                float dist2 = (opp - ahead2).magnitude;
                float dist3 = (opp - ahead3).magnitude;
                float dist4 = (opp - this.transform.position).magnitude;

                float radius = 0.4f;
                if (dist1 <= radius || dist2 <= radius || dist3 <= radius || dist4 <= radius)
                {
                    if (mostDangerousOpponent == -1)
                    {
                        mostDangerousOpponent = i;
                    }
                    else if ((obstacles[mostDangerousOpponent].transform.position - pos).sqrMagnitude < (opp - pos).sqrMagnitude)
                    {
                        mostDangerousOpponent = i;
                    }
                }
            }

            Vector3 result = (target - pos).normalized;
            Vector3 avoidance = Vector3.zero;
            if (Global.sBall.controller != this && mostDangerousOpponent != -1)
            {
                avoidance = Vector3.zero;
                avoidance.x = ahead.x - obstacles[mostDangerousOpponent].transform.position.x;
                avoidance.y = ahead.y - obstacles[mostDangerousOpponent].transform.position.y;
                avoidance.Normalize();

                avoidance = avoidance * 1.0f;

                result = (result + avoidance).normalized;

            }

            this.transform.position = this.transform.position + (desiredVelocity + avoidance).normalized * currentSpeed * Time.fixedDeltaTime;
        }
        else
            m_velocity = Vector3.zero;
    }



    public Vector3 IdlePosition
    {
        get { return m_idlePosition; }
        set { m_idlePosition = value; }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.name.Contains("Player"))
        {
            //Debug.Log(col.gameObject.name);
            Vector3 pos = this.transform.position;
            Vector3 opp = col.gameObject.transform.position;
            this.moveTowards(pos + (pos - opp).normalized, m_normalSpeed);
        }
    }
}
