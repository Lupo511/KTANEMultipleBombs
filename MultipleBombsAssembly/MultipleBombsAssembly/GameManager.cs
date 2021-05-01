using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class GameManager
    {
        private MultipleBombs multipleBombs;
        public GameManagerState CurrentState { get; set; }

        public GameManager(MultipleBombs multipleBombs, KMGameInfo kmGameInfo)
        {
            this.multipleBombs = multipleBombs;

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
                    CurrentState = new GameManagerState(new MultipleBombsSetupStateManager(multipleBombs, SceneManager.Instance.SetupState));
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
                CurrentState.LateUpdate();
        }
    }
}
