using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class GameManager
    {
        private MultipleBombs multipleBombs;
        private KMGameCommands kmGameCommands;
        public StateManager CurrentState { get; set; }

        public GameManager(MultipleBombs multipleBombs, KMGameInfo kmGameInfo, KMGameCommands kmGameCommands)
        {
            this.multipleBombs = multipleBombs;
            this.kmGameCommands = kmGameCommands;

            kmGameInfo.OnStateChange += onStateChange;
        }

        private void onStateChange(KMGameInfo.State state)
        {
            if (CurrentState != null)
            {
                CurrentState = null;
            }

            switch (state)
            {
                case KMGameInfo.State.Setup:
                    CurrentState = new SetupStateManager(multipleBombs, SceneManager.Instance.SetupState);
                    break;
                case KMGameInfo.State.Gameplay:
                    CurrentState = new GameplayStateManager(multipleBombs, SceneManager.Instance.GameplayState, kmGameCommands);
                    break;
            }
        }

        public void Update()
        {
            if (CurrentState != null)
                CurrentState.Update();
        }

        public void LateUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.LateUpdate();
                CurrentState.RunLateUpdateDelegates();
            }
        }
    }
}
