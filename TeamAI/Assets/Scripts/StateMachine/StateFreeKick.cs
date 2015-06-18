using UnityEngine;
using System.Collections;

namespace TeamAI
{
    public class StateFreeKick : State
    {
        float timer = 0.0f;
        public override void enter()
        {
            Debug.Log("Entering freekick");
            timer = 3.0f;
            Global.gameRunning = false;

            if (Global.CoachBlue.teamControlsBall())
                Global.CoachRed.m_statistics.foulsMade += 1;
            else
                Global.CoachBlue.m_statistics.foulsMade += 1;
            //throw new System.NotImplementedException();
        }

        public override void execute()
        {
            timer -= Time.deltaTime;

            if (Global.CoachBlue.teamControlsBall())
            {
                Global.CoachBlue.calculateOffence();
                Global.CoachRed.calculateDefence();
                Global.CoachRed.moveOutsideBallRadius();
            }
            else
            {
                Global.CoachBlue.calculateDefence();
                Global.CoachRed.calculateOffence();
                Global.CoachBlue.moveOutsideBallRadius();
            }

            if (Global.sBall.controllerInRange())
            {
                State newState;
                if (Global.CoachBlue.teamControlsBall())
                    newState = new StateBlueBall();
                else
                    newState = new StateRedBall();
                GameObject.Find("Field").GetComponent<GameStateManager>().changeState(newState);
            }
            //throw new System.NotImplementedException();
        }

        public override void exit()
        {
            Global.gameRunning = true;
            //throw new System.NotImplementedException();
        }
    }
}