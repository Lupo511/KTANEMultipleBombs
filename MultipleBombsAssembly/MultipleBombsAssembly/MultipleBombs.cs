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
        private Resources.ResourceManager resourceManager;
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

            resourceManager = new Resources.ResourceManager(new ModHelper("MultipleBombs"), I2.Loc.LocalizationManager.CurrentCulture);
            gameManager = new GameManager(this, gameInfo, gameCommands, resourceManager);

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
        private Bomb createBomb(int bombIndex, Vector3 position, Vector3 eulerAngles, int seed, List<KMBombInfo> knownBombInfos)
        {
            if (!(gameManager.CurrentState is GameplayStateManager gameplayStateManager))
                throw new InvalidOperationException("Bombs can only be spawned while in the Gameplay state.");

            MultipleBombsMissionDetails mission = null;
            if (GameplayState.MissionToLoad == FreeplayMissionGenerator.FREEPLAY_MISSION_ID)
            {
                mission = new MultipleBombsMissionDetails(CurrentFreeplayBombCount, FreeplayMissionGenerator.Generate(GameplayState.FreeplaySettings).GeneratorSetting);
            }
            else if (GameplayState.MissionToLoad == ModMission.CUSTOM_MISSION_ID)
            {

                mission = MultipleBombsMissionDetails.ReadMission(GameplayState.CustomMission);
            }
            else
            {
                mission = MultipleBombsMissionDetails.ReadMission(MissionManager.Instance.GetMission(GameplayState.MissionToLoad));
            }

            GeneratorSetting generatorSetting;
            if (!mission.GeneratorSettings.TryGetValue(bombIndex, out generatorSetting))
                generatorSetting = mission.GeneratorSettings[0];

            if (knownBombInfos == null)
                knownBombInfos = FindObjectsOfType<KMBombInfo>().ToList();

            return gameplayStateManager.CreateBomb(generatorSetting, position, eulerAngles, seed, knownBombInfos);
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
