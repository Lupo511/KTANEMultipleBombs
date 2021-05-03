using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class StateManager
    {
        private Queue<Action> lateUpdatePostedDelegates;
        private List<StateCoroutine> coroutines;

        public StateManager()
        {
            lateUpdatePostedDelegates = new Queue<Action>();
            coroutines = new List<StateCoroutine>();
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
                StateCoroutine coroutine = coroutines[i];
                if (coroutine.CoroutineUpdate())
                    coroutines.RemoveAt(i);
            }
        }

        public void PostToLateUpdate(Action action)
        {
            lateUpdatePostedDelegates.Enqueue(action);
        }

        public void StartCoroutine(IEnumerator<ICoroutineYieldable> enumerator)
        {
            coroutines.Add(new StateCoroutine(enumerator));
        }
    }
}
