using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeamAI
{
    public class Plan
    {
        List<int> m_ballPos;
        List<int> m_teamPos;
        List<int> m_opponentPos;
        List<int> m_destinationPos;

        public int ballPos;
        public int teamPos;
        public int opponentPos;
        public int destinationPos;
        public bool isRelative;

        public Plan()
        {
            m_ballPos = new List<int>();
            m_teamPos = new List<int>();
            m_opponentPos = new List<int>();
            m_destinationPos = new List<int>();
            isRelative = false;
        }
    }
}