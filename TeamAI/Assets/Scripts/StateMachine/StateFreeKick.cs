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
            //throw new System.NotImplementedException();
        }

        public override void execute()
        {
            timer -= Time.deltaTime;

            if (Global.CoachBlue.teamControlsBall())
            {
                Global.CoachBlue.calculateOffence();
                Global.CoachRed.calculateDefence();
            }
            else
            {
                Global.CoachBlue.calculateDefence();
                Global.CoachRed.calculateDefence();
            }


            if (timer < 0.0f)
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