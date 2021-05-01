using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class GameManagerState
    {
        public IMultipleBombsStateManager StateManager { get; set; }
        private Queue<Action> lateUpdatePostedDelegates;

        public GameManagerState(IMultipleBombsStateManager stateManager)
        {
            StateManager = stateManager;
        }

        public void Update()
        {
            StateManager.Update();
        }

        public void LateUpdate()
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
