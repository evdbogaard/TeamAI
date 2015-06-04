﻿using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
    public Player controller;
    public float friction;
    public Vector3 velocity;

    bool kickonce = false;

    float ballStealCooldownTime = 1.0f;
    float currentStealTime = 0.0f;

	// Use this for initialization
	void Start() 
    {
        velocity = Vector3.zero;
        friction = -0.15f;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (currentStealTime > 0.0f)
            currentStealTime -= Time.deltaTime;
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

        if (col.gameObject.name.Contains("Goal"))
        {
            TeamAI.Global.sField.GetComponent<Field>().scored = true;
            velocity = Vector3.zero;
            if (col.gameObject.name.Contains("Red"))
            {
                this.controller = TeamAI.Global.CoachRed.FieldPlayers[0];
                this.transform.position = new Vector3(0.1f, 0.0f, 0.0f);
                TeamAI.Global.blueGoals += 1;
            }
            else
            {
                this.controller = TeamAI.Global.CoachBlue.FieldPlayers[0];
                this.transform.position = new Vector3(-0.1f, 0.0f, 0.0f);
                TeamAI.Global.redGoals += 1;
            }
            GameObject.Find("Field").GetComponent<GameStateManager>().changeState(new TeamAI.StateKickoff());
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
    }
}
