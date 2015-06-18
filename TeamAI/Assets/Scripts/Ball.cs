using UnityEngine;
using System.Collections;
using TeamAI;

public class Ball : MonoBehaviour 
{
    public Player controller;
    public float friction;
    public Vector3 velocity;

    private Vector3 m_previousPosition;
    float ballStillTimer;

    float ballStealCooldownTime = 1.0f;
    float currentStealTime = 0.0f;

	// Use this for initialization
	void Start() 
    {
        velocity = Vector3.zero;
        friction = -0.15f;
        m_previousPosition = this.transform.position;
        ballStillTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (currentStealTime > 0.0f)
            currentStealTime -= Time.deltaTime;

        if (Global.gameRunning)
        {
            if (this.transform.position == m_previousPosition)
            {
                ballStillTimer += Time.deltaTime;
                //Debug.Log(ballStillTimer);
                if (ballStillTimer > 5.0f)
                {
                    ballStillTimer = 0.0f;
                    if (Global.CoachBlue.teamControlsBall())
                    {
                        this.controller = Global.CoachRed.FieldPlayers[0];
                        Global.CoachRed.newBallHolder();
                    }
                    else
                    {
                        this.controller = Global.CoachBlue.FieldPlayers[0];
                        Global.CoachBlue.newBallHolder();
                    }
                    GameObject.Find("Field").GetComponent<GameStateManager>().changeState(new StateFreeKick());
                }
            }
            else
            {
                m_previousPosition = this.transform.position;
                ballStillTimer = 0.0f;
            }
        }
        else
            ballStillTimer = 0.0f;
	}

    void FixedUpdate()
    {
        if (velocity.sqrMagnitude > friction * friction)
        {
            velocity += velocity.normalized * friction * Time.fixedDeltaTime;
            this.transform.position = this.transform.position + velocity * Time.fixedDeltaTime;
        }
    }

    public void kick(Vector3 target, float strength)
    {
        if (controllerInRange())
        {
            velocity = (target - this.transform.position).normalized * 2.0f;
            //kickonce = true;
        }
    }

    public bool controllerInRange()
    {
        if ((controller.transform.position - this.transform.position).sqrMagnitude < 0.1f)
            return true;
        else
            return false;
    }

    public float timeToReachTarget(Vector3 target)
    {
        return (timeToReachTarget(target, velocity.magnitude));
    }

    public float timeToReachTarget(Vector3 target, float speed)
    {
        Vector3 startPos = this.transform.position;
        Vector3 endPos = target;

        float distToCover = (endPos - startPos).magnitude;

        //float speed = velocity.magnitude;

        float term = speed * speed + 2.0f * distToCover * friction;
        if (term <= 0)
            return -1.0f;
        else
        {
            float v = Mathf.Sqrt(term);
            return (v - speed) / friction;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name);
        if (!Global.gameRunning)
            return;

        if (col.gameObject.name.Contains("Goal"))
        {
            TeamAI.Global.sField.GetComponent<Field>().scored = true;
            velocity = Vector3.zero;
            if (col.gameObject.name.Contains("Red"))
            {
                this.controller = TeamAI.Global.CoachRed.FieldPlayers[0];
                this.transform.position = new Vector3(0.1f, 0.0f, 0.0f);
                this.controller.coach.newBallHolder();
                TeamAI.Global.blueGoals += 1;
            }
            else
            {
                this.controller = TeamAI.Global.CoachBlue.FieldPlayers[0];
                this.transform.position = new Vector3(-0.1f, 0.0f, 0.0f);
                this.controller.coach.newBallHolder();
                TeamAI.Global.redGoals += 1;
            }
            GameObject.Find("Field").GetComponent<GameStateManager>().changeState(new TeamAI.StateKickoff());
            return;
        }

        /*if (col.gameObject.name == "Player_Red")
            TeamAI.Global.sField.GetComponent<Field>().failed = true;*/

        if (!col.gameObject.name.Contains("Player"))
            return;

        //if (col.gameObject.GetComponent<Player>().downTime > 0.0f)
        //    return;

        Player receiver = col.gameObject.GetComponent<Player>();

        if (controllerInRange())
            return;

        if (!receiver.coach.teamControlsBall() /*&& currentStealTime <= 0.0f*/)
        {
            currentStealTime = ballStealCooldownTime;
            if (receiver.name.Contains("Red"))
            {
                GameObject.Find("Field").GetComponent<GameStateManager>().changeState(new TeamAI.StateRedBall());
            }
            else
            {
                GameObject.Find("Field").GetComponent<GameStateManager>().changeState(new TeamAI.StateBlueBall());
            }
        }
        else
        {
            this.controller.coach.m_statistics.passesReceived += 1;
        }

        this.controller = receiver;
        this.controller.m_usingPlan = false;
        this.controller.coach.newBallHolder();
        velocity = Vector3.zero;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name.Contains("Player"))
        {
            col.gameObject.GetComponent<Player>().downTime = 1.0f;
        }

        if (col.gameObject.name.Contains("Field"))
        {
            if (this.transform.position.y < Global.sFieldBounds.min.y)
            {
                this.transform.position = new Vector3(transform.position.x, Global.sFieldBounds.min.y + 0.1f, 0.0f);
            }
            else if (this.transform.position.y > Global.sFieldBounds.max.y)
            {
                this.transform.position = new Vector3(transform.position.x, Global.sFieldBounds.max.y - 0.1f, 0.0f);
            }
            else if (this.transform.position.x < Global.sFieldBounds.min.x)
            {
                this.transform.position = new Vector3(Global.sFieldBounds.min.x + 0.1f, transform.position.y, 0.0f);
            }
            else if (this.transform.position.y < Global.sFieldBounds.max.x)
            {
                this.transform.position = new Vector3(Global.sFieldBounds.max.x - 0.1f, transform.position.y, 0.0f);
            }

            velocity = Vector3.zero;
            if (Global.CoachBlue.teamControlsBall())
            {
                this.controller = Global.CoachRed.FieldPlayers[0];
                Global.CoachRed.newBallHolder();
            }
            else
            {
                this.controller = Global.CoachBlue.FieldPlayers[0];
                Global.CoachBlue.newBallHolder();
            }
            col.gameObject.GetComponent<GameStateManager>().changeState(new StateFreeKick());
        }
    }
}
