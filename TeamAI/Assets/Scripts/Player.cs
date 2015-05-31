using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public Vector3 m_idlePosition;
    public string designation;

    float maxSpeed = 1.0f;
    Vector3 m_velocity;

	// Use this for initialization
	void Start() 
    {
        m_velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update() 
    {
        //this.transform.position = m_idlePosition;
        moveTowards();
	}

    void moveTowards()
    {
        Vector3 toTarget = (m_idlePosition - this.transform.position);
        float distSqr = toTarget.sqrMagnitude;

        if (distSqr > 0.05f)
        {
            Vector3 desiredVelocity = toTarget.normalized * maxSpeed;
            m_velocity = (desiredVelocity - m_velocity);

            this.transform.position = this.transform.position + m_velocity * Time.deltaTime;
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
