using MultipleBombsAssembly.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class ResultFreeplayPageManager : MonoBehaviour
    {
        private GameplayStateManager gameplayStateManager;
        private ResourceManager resourceManager;
        private MultipleBombsMissionDetails currentMission;
        private ResultFreeplayPage page;
        private TextMeshPro numBombs;

        public void Initialize(GameplayStateManager gameplayStateManager, ResourceManager resourceManager, MultipleBombsMissionDetails currentMission)
        {
            this.gameplayStateManager = gameplayStateManager;
            this.resourceManager = resourceManager;
            this.currentMission = currentMission;

            page = GetComponent<ResultFreeplayPage>();
            numBombs = Instantiate(page.FreeplayModules, page.FreeplayModules.transform.position, page.FreeplayModules.transform.rotation, page.FreeplayModules.transform.parent);
            numBombs.gameObject.SetActive(false);
            Destroy(numBombs.GetComponent<I2.Loc.Localize>());
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

            if (currentMission.BombCount > 1)
            {
                currentMission.GetMissionInfo(out float time, out int modules, out int strikes);

                numBombs.text = string.Format(resourceManager.GetPluralStringValue("BombBinder_TextBombs", currentMission.BombCount), currentMission.BombCount);
                numBombs.gameObject.SetActive(true);
                page.FreeplayTime.text = string.Format("{0}:{1:00}", (int)time / 60, time % 60);
                Localization.SetTerm("BombBinder/txtModuleCount", page.FreeplayModules.gameObject);
                Localization.SetParameter("MODULE_COUNT", modules.ToString(), page.FreeplayModules.gameObject);
                Localization.SetTerm(strikes == currentMission.BombCount ? "BombBinder/results_HardcoreOn" : "BombBinder/results_HardcoreOff", page.FreeplayHardcore.gameObject); //Assumes always positive strikes
            }
        }
    }
}
