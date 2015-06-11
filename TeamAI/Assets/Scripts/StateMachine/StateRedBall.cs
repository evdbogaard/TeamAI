using UnityEngine;
using System.Collections;

namespace TeamAI
{
    public class StateRedBall : State
    {
        public override void enter()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Entering RedBall");
            Global.CoachBlue.setDirectOpponents();
            Global.gameRunning = true;
        }

        public override void execute()
        {
            Global.CoachRed.calculateOffence();
            Global.CoachBlue.calculateDefence();
            //throw new System.NotImplementedException();
        }

        public override void exit()
        {
            //throw new System.NotImplementedException();
        }
    }
}