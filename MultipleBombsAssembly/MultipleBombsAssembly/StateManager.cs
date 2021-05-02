using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class StateManager
    {
        private Queue<Action> lateUpdatePostedDelegates;
        private List<IEnumerator<ICoroutineYieldable>> coroutines;

        public StateManager()
        {
            lateUpdatePostedDelegates = new Queue<Action>();
            coroutines = new List<IEnumerator<ICoroutineYieldable>>();
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

        public void RunCoroutines()
        {
            for (int i = coroutines.Count - 1; i >= 0; i--)
            {
                IEnumerator<ICoroutineYieldable> coroutine = coroutines[i];
                if (coroutine.Current == null || coroutine.Current.IsFinished)
                {
                    if (!coroutine.MoveNext())
                        coroutines.RemoveAt(i);
                }
            }
        }

        public void PostToLateUpdate(Action action)
        {
            lateUpdatePostedDelegates.Enqueue(action);
        }

        public void StartCoroutine(IEnumerator<ICoroutineYieldable> coroutine)
        {
            if (coroutine.MoveNext())
                coroutines.Add(coroutine);
        }
    }
}
