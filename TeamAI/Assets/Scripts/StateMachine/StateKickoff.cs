using UnityEngine;
using System.Collections;

namespace TeamAI
{
    public class StateKickoff : State
    {
        public override void enter()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Entering Kickoff");

            Global.CoachBlue.setStartPositions();
            Global.CoachRed.setStartPositions();
        }

        public override void execute()
        {
            if (Global.CoachBlue.teamInStartPosition() && 
                Global.CoachRed.teamInStartPosition())
            {
                State newState;
                if (Global.CoachBlue.teamControlsBall())
                    newState = new StateBlueBall();
                else
                    newState = new StateRedBall();
                GameObject.Find("Field").GetComponent<GameStateManager>().changeState(newState);
            }

            //Debug.Log("Test");
            //throw new System.NotImplementedException();
        }

        public override void exit()
        {
            //throw new System.NotImplementedException();
        }
    }
}