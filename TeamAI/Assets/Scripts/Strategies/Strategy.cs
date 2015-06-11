using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeamAI
{
    public class PersonalBehavior
    {
        public int behavior;
        public int zoneID;
    }

    public class Strategy
    {
        public int formation;
        public int risk;
        public int attackLine;
        public int midfieldLine;
        public int defendLine;

        public List<PersonalBehavior> m_personal;
        public Strategy()
        {
            formation = 0;

            m_personal = new List<PersonalBehavior>();
        }

        public int getDefenseLine(int x, int y)
        {
            int tmpX = x - 1;
            if (tmpX < 0)
                tmpX = 0;
            return tmpX;
        }

        public List<int> validDefenseLineIDs(int x, int y)
        {
            List<int> retVal = new List<int>();

            switch (defendLine)
            {
                case 0:
                    //attack
                    break;
                case 1:
                    //neutral
                    int newX = x - 1;
                    if (newX < 0) newX = 0;
                    retVal.Add(y * 4 + newX);
                    retVal.Add(((y + 1) % 3) * 4 + newX);
                    retVal.Add(((y + 2) % 3) * 4 + newX);
                    break;
                case 2:
                    //defense
                    break;
                default:
                    break;
            }

            if (retVal.Contains(y * 4 + x))
                retVal.Remove(y * 4 + x);

            return retVal;
        }

        public List<int> validMidLineIDs(int x, int y)
        {
            List<int> retVal = new List<int>();

            switch (midfieldLine)
            {
                case 0:
                    //attack
                    break;
                case 1:
                    //neutral
                    retVal.Add(((y + 1) % 3) * 4 + x);
                    retVal.Add(((y + 2) % 3) * 4 + x);
                    if (x - 1 > 0)
                        retVal.Add(y * 4 + x - 1);
                    if (x + 1 < 3)
                        retVal.Add(y * 4 + x + 1);
                    break;
                case 2:
                    //defense
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public List<int> validAttackLineIDs(int x, int y)
        {
            List<int> retVal = new List<int>();

            int newX = x + 2;
            if (newX > 3) newX = 3;

            retVal.Add(y * 4 + newX);
            retVal.Add(((y + 1) % 3) * 4 + newX);
            retVal.Add(((y + 2) % 3) * 4 + newX);

            if (retVal.Contains(y * 4 + x))
                retVal.Remove(y * 4 + x);

            return retVal;
        }
    }
}