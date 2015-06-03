using UnityEngine;
using System.Collections;
using TeamAI;

public class GameStateManager : MonoBehaviour
{
    private State m_currentState;

    void Start()
    {
        //Global.sBall.controller = Global.CoachBlue.FieldPlayers[0];
        m_currentState = new StateKickoff();
        m_currentState.enter();
    }

    public void changeState(State nextState)
    {
        m_currentState.exit();
        m_currentState = nextState;
        m_currentState.enter();
    }

    void Update()
    {
        m_currentState.execute();
    }
}