using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class MultipleBombsSetupStateManager : IMultipleBombsStateManager
    {
        public FreeplayDeviceManager FreeplayDeviceManager { get; }

        public MultipleBombsSetupStateManager(MultipleBombs multipleBombs, SetupState setupState)
        {
            Debug.Log("[MultipleBombs]Setting up setup state");

            SetupRoom setupRoom = setupState.Room.GetComponent<SetupRoom>();

            FreeplayDeviceManager = setupRoom.FreeplayDevice.gameObject.AddComponent<FreeplayDeviceManager>();
            FreeplayDeviceManager.Initialize(multipleBombs);

            MissionDetailPageManager missionDetailPageMonitor = setupRoom.BombBinder.MissionDetailPage.gameObject.AddComponent<MissionDetailPageManager>();
            missionDetailPageMonitor.Initialize(multipleBombs);

            if (setupRoom.TournamentWhiteboard != null)
            {
                TournamentDetailPageManager tournamentDetailPageMonitor = setupRoom.TournamentWhiteboard.TournamentDetailPage.gameObject.AddComponent<TournamentDetailPageManager>();
                tournamentDetailPageMonitor.Initialize(multipleBombs);
            }

            Debug.Log("[MultipleBombs]Setup state set up");
        }

        public void Update()
        {

        }
    }
}
