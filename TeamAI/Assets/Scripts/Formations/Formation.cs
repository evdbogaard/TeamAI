using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeamAI
{
    public enum eDesignation
    {
        eGoalie,
        eDefender,
        eMidfielder,
        eAttacker,
    }

    public class PlayerInfo
    {
        public Vector3 position;
        public eDesignation designation;
        public bool toggle; // Editor only!!!
    }

    public class Formation
    {
        public List<PlayerInfo> m_playersInfo = new List<PlayerInfo>();
        public string FormationName = "";

    }
}