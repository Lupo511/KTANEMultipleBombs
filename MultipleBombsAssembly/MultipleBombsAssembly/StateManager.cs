using MultipleBombsAssembly.Resources;
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
        private Queue<StateCoroutine> newCoroutines;
        private List<StateCoroutine> coroutines;
        public ResourceManager ResourceManager { get; }

        public StateManager(ResourceManager resourceManager)
        {
            startPostedDelegates = new Queue<Action>();
            lateUpdatePostedDelegates = new Queue<Action>();
            newCoroutines = new Queue<StateCoroutine>();
            coroutines = new List<StateCoroutine>();
            ResourceManager = resourceManager;
        }

        public virtual void EnterState() { }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public virtual void ExitState() { }

        private void CurrentStartNotifier_OnStart()
        {
            while (startPostedDelegates.Count > 0)
            {
                startPostedDelegates.Dequeue()();
            }

            UnityEngine.Object.Destroy(currentStartNotifier.gameObject);
            currentStartNotifier = null;
        }

        public void RunLateUpdate()
        {
            while (lateUpdatePostedDelegates.Count > 0)
            {
                lateUpdatePostedDelegates.Dequeue()();
            }

            //Note: since new coroutines are added in LateUpdate, starting a coroutine from another object's LateUpdate that runs after this will make the coroutine skip 1 frame
            while (newCoroutines.Count > 0)
            {
                coroutines.Add(newCoroutines.Dequeue());
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
            newCoroutines.Enqueue(new StateCoroutine(enumerator));
        }
    }
}
