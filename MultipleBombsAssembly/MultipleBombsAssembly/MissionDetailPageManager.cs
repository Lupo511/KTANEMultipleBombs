﻿using Assets.Scripts.Missions;
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
    public class MissionDetailPageManager : MonoBehaviour
    {
        private static FieldInfo currentMissionField;
        private static FieldInfo canStartField;
        private SetupStateManager setupStateManager;
        private MissionDetailPage page;
        private TextMeshPro textBombs;

        static MissionDetailPageManager()
        {
            currentMissionField = typeof(MissionDetailPage).BaseType.GetField("currentMission", BindingFlags.Instance | BindingFlags.NonPublic);
            canStartField = typeof(MissionDetailPage).GetField("canStartMission", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Initialize(SetupStateManager setupStateManager)
        {
            this.setupStateManager = setupStateManager;

            page = GetComponent<MissionDetailPage>();

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
            //Read the mission and update the page data
            Mission currentMission = (Mission)currentMissionField.GetValue(page);
            bool canStart = UpdateMissionDetailInformation(MultipleBombsMissionDetails.ReadMission(currentMission), currentMission.DescriptionTerm, MultipleBombsModManager.GetMaximumBombs(), page.TextDescription, page.TextTime, page.TextModuleCount, page.TextStrikes, textBombs);
            canStartField.SetValue(page, canStart);
        }

        public static bool UpdateMissionDetailInformation(MultipleBombsMissionDetails missionDetails, string descriptionTerm, int maxBombCount, TextMeshPro textDescription, TextMeshPro textTime, TextMeshPro textModuleCount, TextMeshPro textStrikes, TextMeshPro textBombs)
        {
            bool canStart = false;

            List<string> missingModTypes = new List<string>();
            int maxModuleCount = Math.Max(11, ModManager.Instance.GetMaximumModules());
            int maxFrontFaceModuleCount = Math.Max(5, ModManager.Instance.GetMaximumModulesFrontFace());
            int requiredModuleCount = 0;
            int requiredFrontFaceModuleCount = 0;
            foreach (GeneratorSetting generatorSetting in missionDetails.GeneratorSettings.Values)
            {
                if (generatorSetting.ComponentPools != null)
                {
                    foreach (ComponentPool pool in generatorSetting.ComponentPools)
                    {
                        foreach (string modType in pool.ModTypes)
                        {
                            if (!ModManager.Instance.HasBombComponent(modType))
                            {
                                missingModTypes.Add(modType);
                            }
                        }
                    }
                }
                int moduleCount = generatorSetting.GetComponentCount();
                if (generatorSetting.FrontFaceOnly)
                    requiredFrontFaceModuleCount = Math.Max(requiredFrontFaceModuleCount, moduleCount);
                else
                    requiredModuleCount = Math.Max(requiredModuleCount, moduleCount);
            }
            missingModTypes.Sort();

            float maxTime;
            int totalModules;
            int totalStrikes;
            missionDetails.GetMissionInfo(out maxTime, out totalModules, out totalStrikes);

            if (textDescription != null)
            {
                if (missingModTypes.Count > 0)
                {
                    canStart = false;
                    Localization.SetTerm("BombBinder/error_missingModules", textDescription.gameObject);
                    Localization.SetParameter("MISSING_MODULES_LIST", string.Join("\n", missingModTypes.ToArray()), textDescription.gameObject);
                }
                else if (requiredModuleCount > maxModuleCount)
                {
                    canStart = false;
                    Localization.SetTerm("BombBinder/error_needABiggerBomb", textDescription.gameObject);
                    Localization.SetParameter("MAX_MODULE_COUNT", maxModuleCount.ToString(), textDescription.gameObject);
                }
                else if (requiredFrontFaceModuleCount > maxFrontFaceModuleCount)
                {
                    canStart = false;
                    Localization.SetTerm("BombBinder/error_needABiggerBomb", textDescription.gameObject);
                    Localization.SetParameter("MAX_MODULE_COUNT", maxModuleCount.ToString(), textDescription.gameObject);
                }
                else if (missionDetails.BombCount > maxBombCount)
                {
                    canStart = false;
                    textDescription.text = "A room that can support more bombs is required.\n\nCurrent rooms only support up to " + maxBombCount + " bombs.";
                }
                else
                {
                    canStart = true;
                    Localization.SetTerm(descriptionTerm, textDescription.gameObject);
                }
            }

            Localization.SetTerm("BombBinder/txtModuleCount", textModuleCount.gameObject);
            Localization.SetParameter("MODULE_COUNT", totalModules.ToString(), textModuleCount.gameObject);
            if (missionDetails.BombCount > 1)
            {
                textTime.text = string.Format("{0}:{1:00}", (int)maxTime / 60, maxTime % 60);
                Localization.SetTerm("BombBinder/txtStrikeCount", textStrikes.gameObject);
                Localization.SetParameter("STRIKE_COUNT", totalStrikes.ToString(), textStrikes.gameObject);
                textBombs.text = missionDetails.BombCount + " Bombs";
                textBombs.gameObject.SetActive(true);
            }

            return canStart;
        }
    }
}
