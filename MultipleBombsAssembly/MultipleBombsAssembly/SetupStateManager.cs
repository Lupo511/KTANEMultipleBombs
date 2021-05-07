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
        public FreeplayDeviceManager FreeplayDeviceManager { get; }

        public SetupStateManager(MultipleBombs multipleBombs, SetupState setupState)
        {
            Logger.Log("Setting up setup state");

            SetupRoom setupRoom = setupState.Room.GetComponent<SetupRoom>();

            FreeplayDeviceManager = setupRoom.FreeplayDevice.gameObject.AddComponent<FreeplayDeviceManager>();
            FreeplayDeviceManager.Initialize(multipleBombs, this);

            MissionDetailPageManager missionDetailPageManager = setupRoom.BombBinder.MissionDetailPage.gameObject.AddComponent<MissionDetailPageManager>();
            missionDetailPageManager.Initialize(this);

            if (setupRoom.TournamentWhiteboard != null)
            {
                TournamentDetailPageManager tournamentDetailPageManager = setupRoom.TournamentWhiteboard.TournamentDetailPage.gameObject.AddComponent<TournamentDetailPageManager>();
                tournamentDetailPageManager.Initialize(this);
            }

            Logger.Log("Setup state set up");
        }
    }
}
