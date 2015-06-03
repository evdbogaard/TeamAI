using UnityEngine;
using System.Collections;

namespace TeamAI
{
    public abstract class State
    {
        public abstract void enter();
        public abstract void execute();
        public abstract void exit();
    }
}