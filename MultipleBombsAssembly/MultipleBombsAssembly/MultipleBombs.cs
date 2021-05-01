using Assets.Scripts.Missions;
using Assets.Scripts.Pacing;
using Assets.Scripts.Props;
using Assets.Scripts.Records;
using Assets.Scripts.Settings;
using Assets.Scripts.Tournaments;
using Events;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class MultipleBombs : MonoBehaviour
    {
        public GameManager gameManager;
        public MultipleBombsFreeplaySettings LastFreeplaySettings { get; set; }
        public GameplayMusicControllerManager GameplayMusicControllerManager { get; set; }
        private KMGameInfo gameInfo;
        private KMGameCommands gameCommands;
        private MultipleBombsProperties publicProperties;

        public void Awake()
        {
            Debug.Log("[MultipleBombs]Initializing");

            DestroyImmediate(GetComponent<KMService>()); //Hide from Mod Selector

            gameInfo = GetComponent<KMGameInfo>();
            gameCommands = GetComponent<KMGameCommands>();

            gameManager = new GameManager(this, gameInfo);

            LastFreeplaySettings = new MultipleBombsFreeplaySettings(1);

            GameObject infoObject = new GameObject("MultipleBombs_Info");
            infoObject.transform.parent = gameObject.transform;
            publicProperties = infoObject.AddComponent<MultipleBombsProperties>();
            publicProperties.MultipleBombs = this;

            Debug.Log("[MultipleBombs]Basic initialization finished");

            GameplayMusicControllerManager = MusicManager.Instance.GameplayMusicController.gameObject.AddComponent<GameplayMusicControllerManager>();

            Debug.Log("[MultipleBombs]Initialized");
        }

        public void Update()
        {
            gameManager.Update();
        }

        public void LateUpdate()
        {
            gameManager.LateUpdate();
        }

        public void OnDestroy()
        {
            if (SceneManager.Instance != null && SceneManager.Instance.CurrentState != SceneManager.State.ModManager)
                throw new NotImplementedException();

            Debug.Log("[MultipleBombs]Destroying");

            if (GameplayMusicControllerManager != null)
                Destroy(GameplayMusicControllerManager);

            Debug.Log("[MultipleBombs]Destroyed");
        }

        //To-do: readd create bomb method used by factory

        public int CurrentFreePlayBombCount
        {
            get
            {
                if (gameManager.CurrentState.StateManager is MultipleBombsSetupStateManager setupStateManager)
                    return setupStateManager.FreeplayDeviceManager.FreeplayBombCount;
                else
                    return LastFreeplaySettings.BombCount;
            }
            set
            {
                if (SceneManager.Instance.CurrentState != SceneManager.State.Setup)
                    throw new InvalidOperationException("You can only set the current FreePlay bomb count in the Setup room.");
                if (value < 1)
                    throw new Exception("The bomb count must be greater than 0.");
                if (value > MultipleBombsModManager.GetMaximumBombs())
                    throw new Exception("The specified bomb count is greater than the current maximum bomb count.");
                ((MultipleBombsSetupStateManager)gameManager.CurrentState.StateManager).FreeplayDeviceManager.FreeplayBombCount = value;
            }
        }
    }
}
