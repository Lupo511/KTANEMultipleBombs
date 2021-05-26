using MultipleBombsAssembly.Resources;
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
        private ResourceManager resourceManager;
        public StateManager CurrentState { get; set; }

        public GameManager(MultipleBombs multipleBombs, KMGameInfo kmGameInfo, KMGameCommands kmGameCommands, ResourceManager resourceManager)
        {
            this.multipleBombs = multipleBombs;
            this.kmGameCommands = kmGameCommands;
            this.resourceManager = resourceManager;

            kmGameInfo.OnStateChange += onStateChange;
        }

        private void onStateChange(KMGameInfo.State state)
        {
            if (CurrentState != null)
            {
                CurrentState.ExitState();
                CurrentState = null;
            }

            switch (state)
            {
                case KMGameInfo.State.Setup:
                    if (I2.Loc.LocalizationManager.CurrentCulture != resourceManager.CurrentCulture)
                        resourceManager.LoadNewCulture(I2.Loc.LocalizationManager.CurrentCulture);
                    else if (!resourceManager.ResourcesLoaded)
                        resourceManager.LoadResources();

                    CurrentState = new SetupStateManager(multipleBombs, SceneManager.Instance.SetupState);
                    break;
                case KMGameInfo.State.Gameplay:
                    CurrentState = new GameplayStateManager(multipleBombs, SceneManager.Instance.GameplayState, kmGameCommands);
                    break;
            }

            if (CurrentState != null)
            {
                CurrentState.EnterState();
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
                CurrentState.RunLateUpdate();
            }
        }

        public void CoroutineUpdate()
        {
            if (CurrentState != null)
                CurrentState.RunCoroutines();
        }
    }
}
