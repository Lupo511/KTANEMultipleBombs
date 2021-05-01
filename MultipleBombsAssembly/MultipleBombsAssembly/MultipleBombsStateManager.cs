using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class MultipleBombsStateManager
    {
        private Queue<Action> lateUpdatePostedDelegates;

        public MultipleBombsStateManager()
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
