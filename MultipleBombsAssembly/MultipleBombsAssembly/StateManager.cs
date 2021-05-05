using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class StateManager
    {
        private StartNotifier currentStartNotifier;
        private Queue<Action> startPostedDelegates;
        private Queue<Action> lateUpdatePostedDelegates;
        private List<StateCoroutine> coroutines;

        public StateManager()
        {
            startPostedDelegates = new Queue<Action>();
            lateUpdatePostedDelegates = new Queue<Action>();
            coroutines = new List<StateCoroutine>();
        }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        private void CurrentStartNotifier_OnStart()
        {
            while (startPostedDelegates.Count > 0)
            {
                startPostedDelegates.Dequeue()();
            }

            UnityEngine.Object.Destroy(currentStartNotifier.gameObject);
            currentStartNotifier = null;
        }

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

        public void PostToStart(Action action)
        {
            if (currentStartNotifier == null)
            {
                currentStartNotifier = new UnityEngine.GameObject("StartNotifier").AddComponent<StartNotifier>();
                currentStartNotifier.OnStart += CurrentStartNotifier_OnStart;
            }

            startPostedDelegates.Enqueue(action);
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
