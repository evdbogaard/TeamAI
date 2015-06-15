using UnityEngine;
using System.Collections;

namespace TeamAI
{
    public class StateBlueBall : State
    {
        public override void enter()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Entering BlueBall");
            Global.CoachRed.setDirectOpponents();
            Global.gameRunning = true;
        }

        public override void execute()
        {
            //throw new System.NotImplementedException();
            Global.CoachBlue.m_statistics.timeInPossession += Time.deltaTime;
            Global.gameTime -= Time.deltaTime;

            Global.CoachBlue.calculateOffence();
            Global.CoachRed.calculateDefence();
        }

        public override void exit()
        {
            //throw new System.NotImplementedException();
        }
    }
}