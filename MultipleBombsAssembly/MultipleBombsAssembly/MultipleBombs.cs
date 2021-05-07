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
            Logger.Log("Initializing");

            DestroyImmediate(GetComponent<KMService>()); //Hide from Mod Selector

            gameInfo = GetComponent<KMGameInfo>();
            gameCommands = GetComponent<KMGameCommands>();

            gameManager = new GameManager(this, gameInfo, gameCommands);

            LastFreeplaySettings = new MultipleBombsFreeplaySettings(1);

            GameObject infoObject = new GameObject("MultipleBombs_Info");
            infoObject.transform.parent = gameObject.transform;
            publicProperties = infoObject.AddComponent<MultipleBombsProperties>();
            publicProperties.MultipleBombs = this;

            Logger.Log("Basic initialization finished");

            GameplayMusicControllerManager = MusicManager.Instance.GameplayMusicController.gameObject.AddComponent<GameplayMusicControllerManager>();

            StartCoroutine(UpdateCoroutine());

            Logger.Log("Initialized");
        }

        public void Update()
        {
            gameManager.Update();
        }

        public void LateUpdate()
        {
            gameManager.LateUpdate();
        }

        public IEnumerator UpdateCoroutine()
        {
            yield return null;
            while (true)
            {
                gameManager.CoroutineUpdate();
                yield return null;
            }
        }

        public void OnDestroy()
        {
            if (SceneManager.Instance != null && SceneManager.Instance.CurrentState != SceneManager.State.ModManager)
                throw new NotImplementedException();

            Logger.Log("Destroying");

            if (GameplayMusicControllerManager != null)
                Destroy(GameplayMusicControllerManager);

            Logger.Log("Destroyed");
        }

        //This is the interface for Factory
        private Bomb createBomb(int generatorSettingIndex, Vector3 position, Vector3 eulerAngles, int seed, List<KMBombInfo> knownBombInfos)
        {
            if (!(gameManager.CurrentState is GameplayStateManager gameplayStateManager))
                throw new InvalidOperationException("Bomb can only be spawned while in the Gameplay state.");

            if (knownBombInfos == null)
                knownBombInfos.AddRange(FindObjectsOfType<KMBombInfo>());

            return gameplayStateManager.CreateBomb(generatorSettingIndex, position, eulerAngles, seed, knownBombInfos);
        }

        public int CurrentFreeplayBombCount
        {
            get
            {
                if (gameManager.CurrentState is SetupStateManager setupStateManager)
                    return setupStateManager.FreeplayDeviceManager.FreeplayBombCount;
                else
                    return LastFreeplaySettings.BombCount;
            }
            set
            {
                if (SceneManager.Instance.CurrentState != SceneManager.State.Setup)
                    throw new InvalidOperationException("You can only set the current Freeplay bomb count in the Setup room.");
                if (value < 1)
                    throw new Exception("The bomb count must be greater than 0.");
                if (value > MultipleBombsModManager.GetMaximumBombs())
                    throw new Exception("The specified bomb count is greater than the current maximum bomb count.");
                ((SetupStateManager)gameManager.CurrentState).FreeplayDeviceManager.FreeplayBombCount = value;
            }
        }
    }
}
