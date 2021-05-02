using Assets.Scripts.Missions;
using Assets.Scripts.Pacing;
using Assets.Scripts.Records;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class GameplayStateManager : StateManager
    {
        private static FieldInfo gameplayStateRoomGOField;
        private static FieldInfo gameplayStateLightBulbField;
        private KMGameCommands gameCommands;
        private MultipleBombsMissionDetails currentMission;
        private Dictionary<Bomb, BombEvents.BombSolvedEvent> bombSolvedEvents;
        private Dictionary<Bomb, BombComponentEvents.ComponentPassEvent> bombComponentPassEvents;
        private Dictionary<Bomb, BombComponentEvents.ComponentStrikeEvent> bombComponentStrikeEvents;

        static GameplayStateManager()
        {
            gameplayStateRoomGOField = typeof(GameplayState).GetField("roomGO", BindingFlags.Instance | BindingFlags.NonPublic);
            gameplayStateLightBulbField = typeof(GameplayState).GetField("lightBulb", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public GameplayStateManager(MultipleBombs multipleBombs, GameplayState gameplayState, KMGameCommands gameCommands)
        {
            this.gameCommands = gameCommands;
            if (bombSolvedEvents == null)
                bombSolvedEvents = new Dictionary<Bomb, BombEvents.BombSolvedEvent>();
            if (bombComponentPassEvents == null)
                bombComponentPassEvents = new Dictionary<Bomb, BombComponentEvents.ComponentPassEvent>();
            if (bombComponentStrikeEvents == null)
                bombComponentStrikeEvents = new Dictionary<Bomb, BombComponentEvents.ComponentStrikeEvent>();

            //Process mission to load
            List<ComponentPool> multipleBombsComponentPools = null;
            Mission mission = null;
            if (GameplayState.MissionToLoad == FreeplayMissionGenerator.FREEPLAY_MISSION_ID)
            {
                mission = FreeplayMissionGenerator.Generate(GameplayState.FreeplaySettings);
                currentMission = new MultipleBombsMissionDetails(multipleBombs.CurrentFreePlayBombCount, mission.GeneratorSetting);
            }
            else if (GameplayState.MissionToLoad == ModMission.CUSTOM_MISSION_ID)
            {
                mission = GameplayState.CustomMission;
                currentMission = MultipleBombsMissionDetails.ReadMission(GameplayState.CustomMission, true, out multipleBombsComponentPools);
            }
            else
            {
                mission = MissionManager.Instance.GetMission(GameplayState.MissionToLoad);
                currentMission = MultipleBombsMissionDetails.ReadMission(mission, true, out multipleBombsComponentPools);
            }

            //Select a valid room for the required bomb count
            if (currentMission.BombCount > 1 && GameplayState.GameplayRoomPrefabOverride == null)
            {
                Debug.Log("[MultipleBombs]Initializing room");
                List<GameplayRoom> rooms = new List<GameplayRoom>();
                if (currentMission.BombCount <= 2) //To-do: match game behaviour and only pick default room if no valid mod rooms are available?
                    rooms.Add(gameplayState.GameplayRoomPool.Default.GetComponent<GameplayRoom>());
                foreach (GameObject gameplayRoomObject in gameplayState.GameplayRoomPool.Objects)
                {
                    GameplayRoom gameplayRoom = gameplayRoomObject.GetComponent<GameplayRoom>();
                    if (currentMission.BombCount <= MultipleBombsModManager.GetRoomSupportedBombCount(gameplayRoom))
                        rooms.Add(gameplayRoom);
                }
                if (rooms.Count == 0) //To-do: match game behaviour and use default room with less bombs if no room is available?
                {
                    Debug.Log("[MultipleBombs]No room found that supports " + currentMission.BombCount + " bombs");
                    SceneManager.Instance.ReturnToSetupState();
                    return;
                }
                else
                {
                    GameplayRoom roomPrefab = rooms[UnityEngine.Random.Range(0, rooms.Count)];
                    UnityEngine.Object.Destroy((GameObject)gameplayStateRoomGOField.GetValue(SceneManager.Instance.GameplayState));
                    GameObject room = UnityEngine.Object.Instantiate(roomPrefab.gameObject, Vector3.zero, Quaternion.identity);
                    room.transform.parent = SceneManager.Instance.GameplayState.transform;
                    room.transform.localScale = Vector3.one;
                    gameplayStateRoomGOField.SetValue(SceneManager.Instance.GameplayState, room);
                    gameplayStateLightBulbField.SetValue(SceneManager.Instance.GameplayState, GameObject.Find("LightBulb"));
                    room.SetActive(false);
                    UnityEngine.Object.FindObjectOfType<BombGenerator>().BombPrefabOverride = room.GetComponent<GameplayRoom>().BombPrefabOverride;
                    Debug.Log("[MultipleBombs]Room initialized");
                }
            }

            //Subscribe to bomb events to correct data and dispatch custom ones
            BombComponentEvents.OnComponentPass += onComponentPassEvent;
            BombComponentEvents.OnComponentStrike += onComponentStrikeEvent;
            BombEvents.OnBombDetonated += onBombDetonatedEvent;
            Debug.Log("[MultipleBombs]Events initialized");

            //Setup results screen
            ResultPageMonitor freePlayDefusedPageMonitor = SceneManager.Instance.PostGameState.Room.BombBinder.ResultFreeplayDefusedPage.gameObject.AddComponent<ResultPageMonitor>();
            freePlayDefusedPageMonitor.MultipleBombs = multipleBombs;
            freePlayDefusedPageMonitor.SetCurrentMission(currentMission);
            ResultPageMonitor freePlayExplodedPageMonitor = SceneManager.Instance.PostGameState.Room.BombBinder.ResultFreeplayExplodedPage.gameObject.AddComponent<ResultPageMonitor>();
            freePlayExplodedPageMonitor.MultipleBombs = multipleBombs;
            freePlayExplodedPageMonitor.SetCurrentMission(currentMission);
            ResultPageMonitor missionDefusedPageMonitor = SceneManager.Instance.PostGameState.Room.BombBinder.ResultDefusedPage.gameObject.AddComponent<ResultPageMonitor>();
            missionDefusedPageMonitor.MultipleBombs = multipleBombs;
            missionDefusedPageMonitor.SetCurrentMission(currentMission);
            ResultPageMonitor missionExplodedPageMonitor = SceneManager.Instance.PostGameState.Room.BombBinder.ResultExplodedPage.gameObject.AddComponent<ResultPageMonitor>();
            missionExplodedPageMonitor.MultipleBombs = multipleBombs;
            missionExplodedPageMonitor.SetCurrentMission(currentMission);
            ResultPageMonitor tournamentPageMonitor = SceneManager.Instance.PostGameState.Room.BombBinder.ResultTournamentPage.gameObject.AddComponent<ResultPageMonitor>();
            tournamentPageMonitor.MultipleBombs = multipleBombs;
            tournamentPageMonitor.SetCurrentMission(currentMission);
            Debug.Log("[MultipleBombs]Result screens initialized");

            //Let the game generate the bomb and then continue setup
            PostToLateUpdate(() => setupBombs(gameplayState, mission, multipleBombsComponentPools));

            //Start the start round coroutine
            StartCoroutine(StartRound(gameplayState));
        }

        private void setupBombs(GameplayState gameplayState, Mission mission, List<ComponentPool> multipleBombsComponentPools)
        {
            //Restore MultipleBombs previously removed component pools
            if (GameplayState.MissionToLoad == ModMission.CUSTOM_MISSION_ID || GameplayState.MissionToLoad != FreeplayMissionGenerator.FREEPLAY_MISSION_ID)
            {
                mission.GeneratorSetting.ComponentPools.AddRange(multipleBombsComponentPools);
            }

            Debug.Log("[MultipleBombs]Setting up bombs");

            Debug.Log("[MultipleBombs]Bombs to spawn: " + currentMission.BombCount);

            if (currentMission.BombCount > 1)
            {
                List<KMBombInfo> redirectedBombInfos = new List<KMBombInfo>();

                //Process vanilla bomb
                Bomb vanillaBomb = SceneManager.Instance.GameplayState.Bomb;

                GameObject spawn0 = GameObject.Find("MultipleBombs_Spawn_0");
                if (spawn0 != null)
                {
                    vanillaBomb.gameObject.transform.position = spawn0.transform.position;
                    vanillaBomb.gameObject.transform.rotation = spawn0.transform.rotation;
                }
                else
                {
                    vanillaBomb.gameObject.transform.position += new Vector3(-0.4f, 0, 0);
                    vanillaBomb.gameObject.transform.eulerAngles += new Vector3(0, -30, 0);
                }
                vanillaBomb.GetComponent<FloatingHoldable>().Initialize();

                redirectPresentBombInfos(vanillaBomb, redirectedBombInfos);

                processBombEvents(vanillaBomb);
                Debug.Log("[MultipleBombs]Vanilla bomb initialized");

                //Create a random to generate subsequent bomb seeds
                System.Random random;
                if (GameplayState.BombSeedToUse == -1)
                    random = new System.Random();
                else
                    random = new System.Random(GameplayState.BombSeedToUse);

                //Generate bombs
                for (int i = 1; i < currentMission.BombCount; i++)
                {
                    GeneratorSetting generatorSetting;
                    if (!currentMission.GeneratorSettings.TryGetValue(i, out generatorSetting))
                    {
                        generatorSetting = currentMission.GeneratorSettings[0];
                    }

                    Vector3 position;
                    Vector3 eulerAngles;
                    GameObject spawn = GameObject.Find("MultipleBombs_Spawn_" + i);
                    if (spawn != null)
                    {
                        position = spawn.transform.position;
                        eulerAngles = spawn.transform.eulerAngles;
                    }
                    else
                    {
                        if (i == 1)
                        {
                            position = SceneManager.Instance.GameplayState.Room.BombSpawnPosition.transform.position + new Vector3(0.4f, 0, 0);
                            eulerAngles = new Vector3(0, 30, 0);
                        }
                        else
                        {
                            Debug.LogError("[MultipleBombs]The current gameplay room doesn't support " + (i + 1) + " bombs");
                            break;
                        }
                    }

                    Bomb bomb = CreateBomb(generatorSetting, position, eulerAngles, random.Next(), redirectedBombInfos);

                    bomb.GetTimer().text.gameObject.SetActive(false);
                    bomb.GetTimer().LightGlow.enabled = false;

                    //Insert the bomb in the selectable hierarchy
                    Selectable mainSelectable = gameplayState.Room.GetComponent<Selectable>();
                    List<Selectable> children = mainSelectable.Children.ToList();
                    //Increase row length by one (adding nulls at the end of every row except for the bombs row)
                    int row = 0;
                    for (int j = mainSelectable.ChildRowLength; j <= children.Count; j += mainSelectable.ChildRowLength)
                    {
                        if (row != gameplayState.Room.BombSpawnPosition.SelectableIndexY)
                        {
                            children.Insert(j, null);
                            j++;
                        }
                        row++;
                    }
                    mainSelectable.ChildRowLength++;
                    //Correct the default selectable index
                    mainSelectable.DefaultSelectableIndex = gameplayState.Room.BombSpawnPosition.SelectableIndexY * mainSelectable.ChildRowLength + gameplayState.Room.BombSpawnPosition.SelectableIndexX;
                    //Insert the bomb
                    children.Insert(mainSelectable.DefaultSelectableIndex + i, bomb.GetComponent<Selectable>());
                    mainSelectable.Children = children.ToArray();
                    bomb.GetComponent<Selectable>().Parent = mainSelectable;
                    KTInputManager.Instance.SelectableManager.ConfigureSelectableAreas(KTInputManager.Instance.RootSelectable);
                }

                vanillaBomb.GetComponent<Selectable>().Parent.Init();
                Debug.Log("[MultipleBombs]All bombs generated");

                //Setup pacing events
                //To-do: move event subscribing to pace maker and monitor creation to first frame
                PaceMakerMonitor monitor = UnityEngine.Object.FindObjectOfType<PaceMaker>().gameObject.AddComponent<PaceMakerMonitor>();
                foreach (Bomb bomb in SceneManager.Instance.GameplayState.Bombs)
                {
                    if (bomb != vanillaBomb) //The vanilla bomb is still handled by PaceMaker
                        bomb.GetTimer().TimerTick = (TimerComponent.TimerTickEvent)Delegate.Combine(bomb.GetTimer().TimerTick, new TimerComponent.TimerTickEvent((elapsed, remaining) => monitor.OnBombTimerTick(bomb, elapsed, remaining)));
                }
                Debug.Log("[MultipleBombs]Pacing events initalized");
            }
        }

        private IEnumerator<ICoroutineYieldable> StartRound(GameplayState gameplayState)
        {
            yield return new CoroutineTimeDelay(2);
            foreach (Bomb bomb in gameplayState.Bombs)
            {
                if (bomb != gameplayState.Bomb)
                {
                    Debug.Log("[MultipleBombs]Activating custom bomb timer");
                    bomb.GetTimer().text.gameObject.SetActive(true);
                    bomb.GetTimer().LightGlow.enabled = true;
                    Debug.Log("[MultipleBombs]Custom bomb timer activated");
                }
            }
        }

        public Bomb CreateBomb(KMGeneratorSetting generatorSetting, Vector3 position, Vector3 eulerAngles, int seed, List<KMBombInfo> knownBombInfos)
        {
            Debug.Log("[MultipleBombs]Creating new bomb");
            GameObject spawnPointGO = new GameObject("CustomBombSpawnPoint");
            spawnPointGO.transform.position = position;
            spawnPointGO.transform.eulerAngles = eulerAngles;
            Bomb bomb = gameCommands.CreateBomb(null, generatorSetting, spawnPointGO, seed.ToString()).GetComponent<Bomb>();
            Debug.Log("[MultipleBombs]Bomb spawned");

            redirectPresentBombInfos(bomb, knownBombInfos);
            Debug.Log("[MultipleBombs]KMBombInfos redirected");

            processBombEvents(bomb);
            Debug.Log("[MultipleBombs]Bomb created");
            return bomb;
        }

        public Bomb CreateBomb(GeneratorSetting generatorSetting, Vector3 position, Vector3 eulerAngles, int seed, List<KMBombInfo> knownBombInfos)
        {
            return CreateBomb(GeneratorSettingUtils.CreateModFromGeneratorSetting(generatorSetting), position, eulerAngles, seed, knownBombInfos);
        }

        private void redirectPresentBombInfos(Bomb bomb, List<KMBombInfo> knownBombInfos)
        {
            if (knownBombInfos == null)
                knownBombInfos = new List<KMBombInfo>();

            foreach (KMBombInfo info in UnityEngine.Object.FindObjectsOfType<KMBombInfo>())
            {
                if (!knownBombInfos.Contains(info))
                {
                    //To-do: just implement entirely here
                    BombInfoRedirection.RedirectBombInfo(info, bomb);
                    ModBombInfo modInfo = info.GetComponent<ModBombInfo>();
                    foreach (BombEvents.BombSolvedEvent bombSolvedDelegate in BombEvents.OnBombSolved.GetInvocationList())
                    {
                        if (bombSolvedDelegate.Target != null && ReferenceEquals(bombSolvedDelegate.Target, modInfo))
                        {
                            if (bombSolvedEvents.ContainsKey(bomb))
                                bombSolvedEvents[bomb] += bombSolvedDelegate;
                            else
                                bombSolvedEvents.Add(bomb, bombSolvedDelegate);
                            BombEvents.OnBombSolved -= bombSolvedDelegate;
                            break;
                        }
                    }
                    knownBombInfos.Add(info);
                }
            }
        }

        private void processBombEvents(Bomb bomb)
        {
            //Process timer and strike indicator bomb solved event handler
            List<Delegate> bombSolvedDelegates = BombEvents.OnBombSolved.GetInvocationList().ToList();
            for (int i = bombSolvedDelegates.Count - 1; i >= 0; i--)
            {
                BombEvents.BombSolvedEvent bombSolvedDelegate = (BombEvents.BombSolvedEvent)bombSolvedDelegates[i];

                if (bombSolvedDelegate.Target != null && (ReferenceEquals(bombSolvedDelegate.Target, bomb.GetTimer()) || ReferenceEquals(bombSolvedDelegate.Target, bomb.StrikeIndicator)))
                {
                    BombEvents.OnBombSolved -= bombSolvedDelegate;
                    if (bombSolvedEvents.ContainsKey(bomb))
                        bombSolvedEvents[bomb] += bombSolvedDelegate;
                    else
                        bombSolvedEvents.Add(bomb, bombSolvedDelegate);
                    bombSolvedDelegates.RemoveAt(i);
                }
            }

            //Process bomb component pass and strike events
            List<Delegate> componentPassDelegates = BombComponentEvents.OnComponentPass.GetInvocationList().ToList();
            List<Delegate> componentStrikeDelegates = BombComponentEvents.OnComponentStrike.GetInvocationList().ToList();
            foreach (BombComponent component in bomb.BombComponents)
            {
                //Remove bomb's event handler (because we need to only win the game if all bombs have been solved)
                component.OnPass = onComponentPass;

                //Process needy component event handlers
                if (component is NeedyComponent)
                {
                    for (int i = bombSolvedDelegates.Count - 1; i >= 0; i--)
                    {
                        BombEvents.BombSolvedEvent bombSolvedDelegate = (BombEvents.BombSolvedEvent)bombSolvedDelegates[i];
                        if (bombSolvedDelegate != null && ReferenceEquals(bombSolvedDelegate.Target, component))
                        {
                            BombEvents.OnBombSolved -= bombSolvedDelegate;
                            if (bombSolvedEvents.ContainsKey(bomb))
                                bombSolvedEvents[bomb] += bombSolvedDelegate;
                            else
                                bombSolvedEvents.Add(bomb, bombSolvedDelegate);
                            bombSolvedDelegates.RemoveAt(i);
                            break;
                        }
                    }
                    for (int i = componentPassDelegates.Count - 1; i >= 0; i--)
                    {
                        BombComponentEvents.ComponentPassEvent componentPassEvent = (BombComponentEvents.ComponentPassEvent)componentPassDelegates[i];
                        if (componentPassEvent.Target != null && ReferenceEquals(componentPassEvent.Target, component))
                        {
                            BombComponentEvents.OnComponentPass -= componentPassEvent;
                            if (bombComponentPassEvents.ContainsKey(bomb))
                                bombComponentPassEvents[bomb] += componentPassEvent;
                            else
                                bombComponentPassEvents.Add(bomb, componentPassEvent);
                            componentPassDelegates.RemoveAt(i);
                            break;
                        }
                    }
                    for (int i = componentStrikeDelegates.Count - 1; i >= 0; i--)
                    {
                        BombComponentEvents.ComponentStrikeEvent componentStrikeEvent = (BombComponentEvents.ComponentStrikeEvent)componentStrikeDelegates[i];
                        if (componentStrikeEvent.Target != null && ReferenceEquals(componentStrikeEvent.Target, component))
                        {
                            BombComponentEvents.OnComponentStrike -= componentStrikeEvent;
                            if (bombComponentStrikeEvents.ContainsKey(bomb))
                                bombComponentStrikeEvents[bomb] += componentStrikeEvent;
                            else
                                bombComponentStrikeEvents.Add(bomb, componentStrikeEvent);
                            componentStrikeDelegates.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private bool onComponentPass(BombComponent source)
        {
            Debug.Log("[MultipleBombs]A component was solved");

            if (source.Bomb.HasDetonated)
                return false;

            RecordManager.Instance.RecordModulePass();

            if (source.Bomb.IsSolved())
            {
                Debug.Log("[MultipleBombs]A bomb was solved (A winner is you!!)");

                source.Bomb.GetTimer().StopTimer();
                source.Bomb.GetTimer().Blink(1.5f);

                DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransformAndForget("bomb_defused", source.Bomb.transform, 1f, null, 0f, null);

                if (BombEvents.OnBombSolved != null)
                    BombEvents.OnBombSolved();
                if (bombSolvedEvents.ContainsKey(source.Bomb) && bombSolvedEvents[source.Bomb] != null)
                    bombSolvedEvents[source.Bomb].Invoke();

                SceneManager.Instance.GameplayState.OnBombSolved();

                //Check if all bombs have been solved and if yes win the game
                foreach (Bomb bomb in SceneManager.Instance.GameplayState.Bombs)
                    if (!bomb.IsSolved())
                        return true;

                Debug.Log("[MultipleBombs]All bombs solved, what a winner!");

                GameRecord currentRecord = RecordManager.Instance.GetCurrentRecord();

                //Set the remaining time correctly
                float maxTime = currentRecord.FreeplaySettings != null ? currentRecord.FreeplaySettings.Time : MissionManager.Instance.GetMission(currentRecord.MissionID).GeneratorSetting.TimeLimit;
                RecordManager.Instance.SetResult(GameResultEnum.Defused, maxTime - source.Bomb.GetTimer().TimeRemaining, SceneManager.Instance.GameplayState.GetElapsedRealTime());
                return true;
            }
            return false;
        }

        private void onComponentPassEvent(BombComponent component, bool finalPass)
        {
            if (bombComponentPassEvents.ContainsKey(component.Bomb) && bombComponentPassEvents[component.Bomb] != null)
                bombComponentPassEvents[component.Bomb].Invoke(component, finalPass);
        }

        private void onComponentStrikeEvent(BombComponent component, bool finalStrike)
        {
            //If we filled the strikes record append an empty one (for the next strike)
            GameRecord currentRecord = RecordManager.Instance.GetCurrentRecord();
            if (!finalStrike && currentRecord.GetStrikeCount() == currentRecord.Strikes.Length)
            {
                List<StrikeSource> strikes = currentRecord.Strikes.ToList();
                strikes.Add(null);
                currentRecord.Strikes = strikes.ToArray();
            }

            if (bombComponentStrikeEvents.ContainsKey(component.Bomb) && bombComponentStrikeEvents[component.Bomb] != null)
                bombComponentStrikeEvents[component.Bomb].Invoke(component, finalStrike);
        }

        private void onBombDetonatedEvent()
        {
            foreach (Bomb bomb in SceneManager.Instance.GameplayState.Bombs)
            {
                bomb.GetTimer().StopTimer();
            }

            //If there was remaining time (exploded due to strikes) set the remaining time in the record correctly
            if (RecordManager.Instance.GetCurrentRecord().Result == GameResultEnum.ExplodedDueToStrikes)
            {
                float timeElapsed = -1;
                foreach (Bomb bomb in SceneManager.Instance.GameplayState.Bombs)
                {
                    if (!bomb.IsSolved())
                    {
                        float bombTimeElapsed = bomb.GetTimer().TimeElapsed;
                        if (bombTimeElapsed > timeElapsed)
                            timeElapsed = bombTimeElapsed;
                    }
                }

                if (timeElapsed != -1)
                    RecordManager.Instance.SetResult(GameResultEnum.ExplodedDueToStrikes, timeElapsed, SceneManager.Instance.GameplayState.GetElapsedRealTime());
            }
        }
    }
}
