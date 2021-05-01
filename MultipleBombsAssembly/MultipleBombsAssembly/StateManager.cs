using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class StateManager
    {
        private Queue<Action> lateUpdatePostedDelegates;

        public StateManager()
        {
            lateUpdatePostedDelegates = new Queue<Action>();
        }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public void RunLateUpdateDelegates()
        {
            while (lateUpdatePostedDelegates.Count > 0)
            {
                lateUpdatePostedDelegates.Dequeue()();
            }
        }

        public void PostToLateUpdate(Action action)
        {
            lateUpdatePostedDelegates.Enqueue(action);
        }
    }
}
