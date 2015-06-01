using UnityEngine;
using System.Collections;
using TeamAI;

public class Player : MonoBehaviour 
{
    public Vector3 m_idlePosition;
    public string designation;

    public Vector3 m_planDesignation;
    public bool m_usingPlan;

    public Player directOpponent;

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
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (downTime > 0.0f)
            downTime -= Time.deltaTime;

        //this.transform.position = m_idlePosition;
        if (coach.teamControlsBall())
        {
            if (m_usingPlan)
            {
                moveTowards(m_planDesignation, 1.5f);
            }
            else if (Global.sBall.controller != this)
            {
                moveTowards(m_idlePosition, 1.0f);
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
        }
	}

    public void setSupportRole(Vector3 destination)
    {
        Debug.Log(timeToReachPos(destination));
        m_usingPlan = true;
        m_planDesignation = destination;
    }

    public float timeToReachPos(Vector3 pos)
    {
        float distance = (pos - this.transform.position).magnitude;

        float currentSpeed = (m_usingPlan) ? 1.5f : 0.0f;

        float time = distance / currentSpeed;
        
        return time;
    }

    void moveTowards(Vector3 target, float currentSpeed)
    {
        Vector3 toTarget = (target - this.transform.position);
        float distSqr = toTarget.sqrMagnitude;

        if (distSqr > 0.05f)
        {
            Vector3 desiredVelocity = toTarget.normalized * currentSpeed;
            m_velocity = (desiredVelocity - m_velocity);

            this.transform.position = this.transform.position + desiredVelocity * Time.fixedDeltaTime;
        }
        else
            m_velocity = Vector3.zero;
    }



    public Vector3 IdlePosition
    {
        get { return m_idlePosition; }
        set { m_idlePosition = value; }
    }
}
