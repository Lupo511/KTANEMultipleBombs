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
        public GameManagerState CurrentState { get; set; }

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

            //To-do: finish properly
            if (state == KMGameInfo.State.Setup || state == KMGameInfo.State.Gameplay)
                CurrentState = new GameManagerState(state, multipleBombs, kmGameCommands);
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
