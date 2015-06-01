using UnityEngine;
using System.Collections;
using TeamAI;

public class Player : MonoBehaviour 
{
    public Vector3 m_idlePosition;
    public string designation;

    public Vector3 m_planDesignation;
    public bool m_usingPlan;

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
        if (m_usingPlan)
        {
            moveTowards(m_planDesignation);
        }
        else if (Global.sBall.controller != this)
        {
            moveTowards(m_idlePosition);
        }
        else
        {
            if (!Global.sBall.controllerInRange())
            {
                moveTowards(Global.sBall.transform.position);
            }
            else if (Global.planGridId(Global.sBall.transform.position) == 7)
            {
                Debug.Log("Shot at goal");
                Global.sBall.kick(GameObject.Find("Goal_Red").transform.position, 2.0f);
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

        float time = distance / maxSpeed;
        
        return time;
    }

    void moveTowards(Vector3 target)
    {
        Vector3 toTarget = (target - this.transform.position);
        float distSqr = toTarget.sqrMagnitude;

        if (distSqr > 0.05f)
        {
            Vector3 desiredVelocity = toTarget.normalized * maxSpeed;
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
