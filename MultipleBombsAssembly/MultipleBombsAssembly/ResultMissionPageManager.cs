using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class ResultMissionPageManager : MonoBehaviour
    {
        private GameplayStateManager gameplayStateManager;
        private MultipleBombsMissionDetails currentMission;
        private ResultMissionPage page;
        private TextMeshPro numBombs;

        public void Initialize(GameplayStateManager gameplayStateManager, MultipleBombsMissionDetails currentMission)
        {
            this.gameplayStateManager = gameplayStateManager;
            this.currentMission = currentMission;

            page = GetComponent<ResultMissionPage>();
            numBombs = Instantiate(page.NumStrikes, page.NumStrikes.transform.position, page.NumStrikes.transform.rotation, page.NumStrikes.transform.parent);
            numBombs.gameObject.SetActive(false);
            Destroy(numBombs.GetComponent<Localize>());
            numBombs.transform.localPosition += new Vector3(0, 0.012f, 0);
            numBombs.text = "X Bombs";
        }

        public void OnEnable()
        {
            gameplayStateManager.PostToStart(setupPage);
        }

        public void OnDisable()
        {
            numBombs.gameObject.SetActive(false);
        }

        public void setupPage()
        {
            //Don't run if the object has been disabled
            if (!enabled)
                return;

            MissionDetailPageManager.UpdateMissionDetailInformation(currentMission, null, MultipleBombsModManager.GetMaximumBombs(), null, page.InitialTime, page.NumModules, page.NumStrikes, numBombs);
        }
    }
}
