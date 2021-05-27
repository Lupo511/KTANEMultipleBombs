using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class SetupStateManager : StateManager
    {
        public FreeplayDeviceManager FreeplayDeviceManager { get; private set; }
        private MultipleBombs multipleBombs;
        private SetupState setupState;

        public SetupStateManager(Resources.ResourceManager resourceManager, MultipleBombs multipleBombs, SetupState setupState) : base(resourceManager)
        {
            this.multipleBombs = multipleBombs;
            this.setupState = setupState;
        }

        public override void EnterState()
        {
            Logger.Log("Setting up setup state");

            SetupRoom setupRoom = setupState.Room.GetComponent<SetupRoom>();

            FreeplayDeviceManager = setupRoom.FreeplayDevice.gameObject.AddComponent<FreeplayDeviceManager>();
            FreeplayDeviceManager.Initialize(multipleBombs, this, ResourceManager);

            MissionDetailPageManager missionDetailPageManager = setupRoom.BombBinder.MissionDetailPage.gameObject.AddComponent<MissionDetailPageManager>();
            missionDetailPageManager.Initialize(this, ResourceManager);

            if (setupRoom.TournamentWhiteboard != null)
            {
                TournamentDetailPageManager tournamentDetailPageManager = setupRoom.TournamentWhiteboard.TournamentDetailPage.gameObject.AddComponent<TournamentDetailPageManager>();
                tournamentDetailPageManager.Initialize(this, ResourceManager);
            }

            Logger.Log("Setup state set up");
        }
    }
}
