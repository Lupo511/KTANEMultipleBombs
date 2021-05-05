using Assets.Scripts.Missions;
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
    public class TournamentDetailPageManager : MonoBehaviour
    {
        private static FieldInfo currentMissionField;
        private static FieldInfo canStartField;
        private SetupStateManager setupStateManager;
        private TournamentDetailPage page;
        private TextMeshPro textBombs;

        static TournamentDetailPageManager()
        {
            currentMissionField = typeof(MissionDetailPage).BaseType.GetField("currentMission", BindingFlags.Instance | BindingFlags.NonPublic);
            canStartField = typeof(MissionDetailPage).GetField("canStartMission", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Initialize(SetupStateManager setupStateManager)
        {
            this.setupStateManager = setupStateManager;

            page = GetComponent<TournamentDetailPage>();

            //Add the mission bomb count label by cloning and modifying the strikes label
            textBombs = Instantiate(page.TextStrikes, page.TextStrikes.transform.position, page.TextStrikes.transform.rotation, page.TextStrikes.transform.parent);
            textBombs.gameObject.SetActive(false);
            Destroy(textBombs.GetComponent<Localize>());
            textBombs.transform.localPosition += new Vector3(0, 0.012f, 0);
            textBombs.text = "X Bombs";
        }

        private void OnEnable()
        {
            setupStateManager.PostToStart(setupPage);
        }

        private void OnDisable()
        {
            textBombs.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Destroy(textBombs);
        }

        private void setupPage()
        {
            //Don't run if the object has been disabled
            if (!enabled)
                return;

            //Read the mission and update the page data
            Mission currentMission = (Mission)currentMissionField.GetValue(page);
            bool canStart = MissionDetailPageManager.UpdateMissionDetailInformation(MultipleBombsMissionDetails.ReadMission(currentMission), currentMission.DescriptionTerm, MultipleBombsModManager.GetMaximumBombs(), page.TextDescription, page.TextTime, page.TextModuleCount, page.TextStrikes, textBombs);
            canStartField.SetValue(page, canStart);
        }
    }
}
