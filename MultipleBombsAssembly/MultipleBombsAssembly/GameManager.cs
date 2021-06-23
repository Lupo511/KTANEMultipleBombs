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
        private SceneManager currentSceneManager;
        private SetupStateManager setupStateManager;
        private GameplayStateManager gameplayStateManager;
        public StateManager CurrentState { get; set; }

        public GameManager(MultipleBombs multipleBombs, KMGameInfo kmGameInfo, KMGameCommands kmGameCommands, ResourceManager resourceManager)
        {
            this.multipleBombs = multipleBombs;
            this.kmGameCommands = kmGameCommands;
            this.resourceManager = resourceManager;

            currentSceneManager = SceneManager.Instance;

            kmGameInfo.OnStateChange += onStateChange;
        }

        private void onStateChange(KMGameInfo.State state)
        {
            if (CurrentState != null)
            {
                CurrentState.ExitState();
                CurrentState = null;
            }

            if (SceneManager.Instance != currentSceneManager) //States have been recreated, thus we set our state managers to be recreated as well
            {
                currentSceneManager = SceneManager.Instance;
                setupStateManager = null;
                gameplayStateManager = null;
            }

            switch (state)
            {
                case KMGameInfo.State.Setup:
                    if (I2.Loc.LocalizationManager.CurrentCulture != resourceManager.CurrentCulture)
                        resourceManager.LoadNewCulture(I2.Loc.LocalizationManager.CurrentCulture);
                    else if (!resourceManager.ResourcesLoaded)
                        resourceManager.LoadResources();

                    if (setupStateManager == null)
                    {
                        setupStateManager = new SetupStateManager(resourceManager, multipleBombs, SceneManager.Instance.SetupState);
                        setupStateManager.Create();
                    }
                    CurrentState = setupStateManager;
                    break;
                case KMGameInfo.State.Gameplay:
                    if (gameplayStateManager == null)
                    {
                        gameplayStateManager = new GameplayStateManager(resourceManager, multipleBombs, SceneManager.Instance.GameplayState, kmGameCommands);
                        gameplayStateManager.Create();
                    }
                    CurrentState = gameplayStateManager;
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
