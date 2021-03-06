﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeamAI
{
    public class PersonalBehavior
    {
        public int behavior;
        public int zoneID;
        public int typeToDefend;
    }

    public class Strategy
    {
        public int formation;
        public int risk;
        public int attackLine;
        public int midfieldLine;
        public int defendLine;

        public float score;

        public List<PersonalBehavior> m_personal;
        public Strategy()
        {
            formation = 0;
            score = 3.0f;

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
            int newX = 0;

            switch (defendLine)
            {
                case 0:
                    //attack
                    newX = x - 1;
                    if (newX < 0) newX = 0;
                    retVal.Add(y * 4 + newX);
                    retVal.Add(((y + 1) % 3) * 4 + x);
                    retVal.Add(((y + 2) % 3) * 4 + x);
                    break;
                case 1:
                    //neutral
                    newX = x - 1;
                    if (newX < 0) newX = 0;
                    retVal.Add(y * 4 + newX);
                    retVal.Add(((y + 1) % 3) * 4 + newX);
                    retVal.Add(((y + 2) % 3) * 4 + newX);
                    break;
                case 2:
                    //defense
                    newX = x - 1;
                    if (newX < 0) newX = 0;
                    if (newX > 1) newX = 1;
                    retVal.Add(y * 4 + newX);
                    retVal.Add(((y + 1) % 3) * 4 + newX);
                    retVal.Add(((y + 2) % 3) * 4 + newX);
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
            int newX = 0;

            switch (midfieldLine)
            {
                case 0:
                    //attack
                    newX = x + 1;
                    if (newX > 3) newX = 3;
                    retVal.Add(y * 4 + newX);
                    retVal.Add(((y + 1) % 3) * 4 + newX);
                    retVal.Add(((y + 2) % 3) * 4 + newX);
                    break;
                case 1:
                    //neutral
                    if (y + 1 <= 2)
                        retVal.Add((y + 1) * 4 + x);
                    if (y - 1 >= 0)
                        retVal.Add((y - 1) * 4 + x);
                    if (x - 1 > 0)
                        retVal.Add(y * 4 + x - 1);
                    if (x + 1 < 3)
                        retVal.Add(y * 4 + x + 1);
                    break;
                case 2:
                    //defense
                    retVal.Add(((y + 1) % 3) * 4 + x);
                    retVal.Add(((y + 2) % 3) * 4 + x);
                    if (x - 1 > 0)
                        retVal.Add(y * 4 + x - 1);
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