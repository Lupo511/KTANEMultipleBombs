﻿using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class FreeplayDeviceManager : MonoBehaviour
    {
        private MultipleBombs multipleBombs;
        private MultipleBombsFreeplaySettings freeplaySettings;
        private TextMeshPro bombsValue;
        private static float? vanillaMaxSecondsToSolve;
        private int maxModBombModules;

        public void Initialize(MultipleBombs multipleBombs)
        {
            this.multipleBombs = multipleBombs;

            freeplaySettings = multipleBombs.LastFreeplaySettings;

            if (vanillaMaxSecondsToSolve == null)
                vanillaMaxSecondsToSolve = FreeplayDevice.MAX_SECONDS_TO_SOLVE;
            maxModBombModules = ModManager.Instance.GetMaximumModules();
        }

        public IEnumerator Start()
        {
            Debug.Log("[MultipleBombs]Adding FreePlay option");

            FreeplayDevice freeplayDevice = GetComponent<FreeplayDevice>();

            //Add "Bombs" option to the freeplay device by cloning and modifying the Modules option
            GameObject modulesObject = freeplayDevice.ModuleCountIncrement.transform.parent.gameObject;
            GameObject bombsObject = Instantiate(modulesObject, modulesObject.transform.position, modulesObject.transform.rotation, modulesObject.transform.parent);
            bombsObject.name = "BombCountSettings";
            freeplayDevice.ObjectsToDisableOnLidClose.Add(bombsObject);

            bombsObject.transform.localPosition += new Vector3(0, 0f, -0.025f);

            TextMeshPro bombsLabel = bombsObject.transform.Find("ModuleCountLabel").GetComponent<TextMeshPro>();
            bombsLabel.gameObject.name = "BombCountLabel";
            Destroy(bombsLabel.GetComponent<Localize>());
            bombsLabel.text = "Bombs";

            bombsValue = bombsObject.transform.Find("ModuleCountValue").GetComponent<TextMeshPro>();
            bombsValue.gameObject.name = "BombCountValue";
            bombsValue.text = freeplaySettings.BombCount.ToString();

            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.name = "BombCountBackground";
            Renderer backgroundRenderer = background.GetComponent<Renderer>();
            backgroundRenderer.material.shader = Shader.Find("Unlit/Color");
            backgroundRenderer.material.color = Color.black;
            background.transform.localScale = new Vector3(0.048f, 0.023f, 0.005f); //Accurate Y would be 0.025 but this looks better
            background.transform.parent = bombsObject.transform;
            background.transform.localPosition = bombsValue.gameObject.transform.localPosition + new Vector3(0.00025f, -0.0027f, 0);
            background.transform.localEulerAngles = bombsValue.gameObject.transform.localEulerAngles;

            GameObject incrementButton = bombsObject.transform.Find("Modules_INCR_btn").gameObject;
            incrementButton.name = "Bombs_INCR_btn";
            GameObject decrementButton = bombsObject.transform.Find("Modules_DECR_btn").gameObject;
            decrementButton.name = "Bombs_DECR_btn";
            Selectable deviceSelectable = freeplayDevice.GetComponent<Selectable>();
            Selectable incrementButtonSelectable = incrementButton.GetComponent<Selectable>();
            Selectable decrementButtonSelectable = decrementButton.GetComponent<Selectable>();
            List<Selectable> children = deviceSelectable.Children.ToList();
            children.Insert(2, incrementButtonSelectable);
            children.Insert(2, decrementButtonSelectable);
            deviceSelectable.Children = children.ToArray();
            deviceSelectable.Init();

            GameObject modulesLedShell = modulesObject.transform.parent.Find("LEDs/Modules_LED_shell").gameObject;
            GameObject bombsLedShell = Instantiate(modulesLedShell, modulesLedShell.transform.position, modulesLedShell.transform.rotation, modulesLedShell.transform.parent);
            bombsLedShell.name = "Bombs_LED_shell";
            bombsLedShell.transform.localPosition += new Vector3(0, 0, -0.028f);

            LED bombsLed = bombsObject.transform.Find("ModuleCountLED").GetComponent<LED>();
            bombsLed.gameObject.name = "BombCountLed";
            bombsLed.transform.localPosition += new Vector3(0, 0, 0.003f);
            bombsLed.transform.Find("Modules_LED_On").gameObject.name = "Bombs_LED_On";
            bombsLed.transform.Find("Modules_LED_Off").gameObject.name = "Bombs_LED_Off";
            bombsObject.transform.Find("Modules_INCR_btn_highlight").name = "Bombs_INCR_btn_highlight";
            bombsObject.transform.Find("Modules_DECR_btn_highlight").name = "Bombs_DECR_btn_highlight";

            //Call the Awake callback
            bombsObject.SetActive(true);
            bombsObject.SetActive(false);

            if (KTInputManager.Instance.IsMotionControlMode())
            {
                incrementButtonSelectable.ActivateMotionControls();
                decrementButtonSelectable.ActivateMotionControls();
            }

            incrementButton.GetComponent<KeypadButton>().OnPush = new PushEvent(() =>
            {
                if (freeplaySettings.BombCount >= multipleBombs.GetCurrentMaximumBombCount())
                    return;
                freeplaySettings.BombCount++;
                bombsValue.text = freeplaySettings.BombCount.ToString();
                updateFreeplayDeviceDifficulty(freeplayDevice);
            });
            decrementButton.GetComponent<KeypadButton>().OnPush = new PushEvent(() =>
            {
                if (freeplaySettings.BombCount <= 1)
                    return;
                freeplaySettings.BombCount--;
                bombsValue.text = freeplaySettings.BombCount.ToString();
                updateFreeplayDeviceDifficulty(freeplayDevice);
            });
            //string textColor = "#" + valueText.color.r.ToString("x2") + valueText.color.g.ToString("x2") + valueText.color.b.ToString("x2");
            incrementButtonSelectable.OnHighlight = new Action(() =>
            {
                freeplayDevice.Screen.CurrentState = FreeplayScreen.State.Start;
                bombsLed.SetState(true);
                freeplayDevice.Screen.ScreenText.text = "BOMBS:\n\nNumber of bombs\nto defuse\n\n<size=20><#00ff00>Multiple Bombs Mod</color></size>";
            });
            decrementButtonSelectable.OnHighlight = new Action(() =>
            {
                freeplayDevice.Screen.CurrentState = FreeplayScreen.State.Start;
                bombsLed.SetState(true);
                freeplayDevice.Screen.ScreenText.text = "BOMBS:\n\nNumber of bombs\nto defuse\n\n<size=20><#00ff00>Multiple Bombs Mod</color></size>";
            });

            //Add action to disable the bombs led on the other buttons OnHighlight
            Action disableBomsLed = new Action(() => bombsLed.SetState(false));
            freeplayDevice.ModuleCountDecrement.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.ModuleCountIncrement.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.TimeDecrement.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.TimeIncrement.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.NeedyToggle.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.HardcoreToggle.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.ModsOnly.GetComponent<Selectable>().OnHighlight += disableBomsLed;
            freeplayDevice.StartButton.GetComponent<Selectable>().OnHighlight += disableBomsLed;

            //Resize selectable areas to be more comfortable
            incrementButtonSelectable.SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.015f, -0.015f, -0.015f);
            decrementButtonSelectable.SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.015f, -0.015f, -0.015f);
            freeplayDevice.ModuleCountIncrement.GetComponent<Selectable>().SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.012f, -0.012f, -0.012f);
            freeplayDevice.ModuleCountDecrement.GetComponent<Selectable>().SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.012f, -0.012f, -0.012f);
            freeplayDevice.TimeIncrement.GetComponent<Selectable>().SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.01f, -0.01f, -0.01f);
            freeplayDevice.TimeDecrement.GetComponent<Selectable>().SelectableArea.GetComponent<BoxCollider>().size += new Vector3(-0.01f, -0.01f, -0.01f);

            //Change modules screen text to specify that the module count is per bomb
            Action setCustomModulesText = new Action(() =>
            {
                freeplayDevice.Screen.CurrentState = FreeplayScreen.State.Modules;
                freeplayDevice.Screen.ScreenText.text = "MODULES:\n\nNumber of modules\nper bomb";
            });
            Selectable moduleCountDecrementSelectable = freeplayDevice.ModuleCountDecrement.GetComponent<Selectable>();
            Action moduleCountDecrementAction = (Action)findFreeplayDeviceEventTarget(moduleCountDecrementSelectable.OnHighlight, freeplayDevice);
            if (moduleCountDecrementAction != null)
            {
                moduleCountDecrementSelectable.OnHighlight -= moduleCountDecrementAction;
                moduleCountDecrementSelectable.OnHighlight += setCustomModulesText;
            }
            moduleCountDecrementSelectable.OnHighlight += disableBomsLed;
            Selectable moduleCountIncrementSelectable = freeplayDevice.ModuleCountIncrement.GetComponent<Selectable>();
            Action moduleCountIncrementAction = (Action)findFreeplayDeviceEventTarget(moduleCountIncrementSelectable.OnHighlight, freeplayDevice);
            if (moduleCountIncrementAction != null)
            {
                moduleCountIncrementSelectable.OnHighlight -= moduleCountIncrementAction;
                moduleCountIncrementSelectable.OnHighlight += setCustomModulesText;
            }

            //We need to wait the next frame to patch event handlers after they've been assigned
            yield return null;

            //Patch button push event handlers to calculate custom difficulty
            patchFreeplayButtonPushDifficulty(freeplayDevice.ModuleCountDecrement, freeplayDevice);
            patchFreeplayButtonPushDifficulty(freeplayDevice.ModuleCountIncrement, freeplayDevice);
            patchFreeplayButtonPushDifficulty(freeplayDevice.TimeDecrement, freeplayDevice);
            patchFreeplayButtonPushDifficulty(freeplayDevice.TimeIncrement, freeplayDevice);
            patchFreeplayToggleDifficulty(freeplayDevice.NeedyToggle, freeplayDevice);
            patchFreeplayToggleDifficulty(freeplayDevice.HardcoreToggle, freeplayDevice);

            Debug.Log("[MultipleBombs]FreePlay option added");
        }

        public void Update()
        {
            //Check max bomb count limit
            int maxBombCount = multipleBombs.GetCurrentMaximumBombCount();
            if (freeplaySettings.BombCount > maxBombCount)
                freeplaySettings.BombCount = maxBombCount;

            //Update Freeplay max time
            //First we multiply the vanilla max time by the max bomb count
            float newMaxTime = vanillaMaxSecondsToSolve.Value * maxBombCount;
            //Then we also add 60 seconds per additional max module per bomb
            int maxModules = maxModBombModules;
            if (GameplayState.GameplayRoomPrefabOverride != null && GameplayState.GameplayRoomPrefabOverride.GetComponent<GameplayRoom>().BombPrefabOverride != null)
            {
                maxModules = Math.Max(GameplayState.GameplayRoomPrefabOverride.GetComponent<GameplayRoom>().BombPrefabOverride.GetComponent<Bomb>().GetMaxModules(), FreeplayDevice.MAX_MODULE_COUNT);
                maxModules = Math.Max(maxModBombModules, maxModules);
            }
            if (maxModules > FreeplayDevice.MAX_MODULE_COUNT)
            {
                newMaxTime += (maxModules - FreeplayDevice.MAX_MODULE_COUNT) * 60 *
                              (maxBombCount - 1);
            }
            //Finally update the max seconds to solve field
            FreeplayDevice.MAX_SECONDS_TO_SOLVE = newMaxTime;
        }

        public void OnDestroy()
        {
            multipleBombs.LastFreeplaySettings = freeplaySettings;
        }

        private void updateFreeplayDeviceDifficulty(FreeplayDevice device)
        {
            if (device.CurrentSettings.IsHardCore)
            {
                device.DifficultyIndicator.Configure(device.CurrentSettings.Time, device.CurrentSettings.ModuleCount * freeplaySettings.BombCount, device.CurrentSettings.HasNeedy, true);
                if (device.CurrentSettings.HasNeedy)
                    device.DifficultyIndicator.Difficulty += freeplaySettings.BombCount - 1;
            }
            else
            {
                device.DifficultyIndicator.Configure(device.CurrentSettings.Time, device.CurrentSettings.ModuleCount, device.CurrentSettings.HasNeedy, false);
                device.DifficultyIndicator.Difficulty *= freeplaySettings.BombCount;
            }
        }

        private void patchFreeplayButtonPushDifficulty(KeypadButton button, FreeplayDevice device)
        {
            PushEvent original = (PushEvent)findFreeplayDeviceEventTarget(button.OnPush, device);
            button.OnPush -= original;
            button.OnPush += new PushEvent(() =>
            {
                if (original != null)
                    original();
                updateFreeplayDeviceDifficulty(device);
            });
        }

        private void patchFreeplayToggleDifficulty(ToggleSwitch toggle, FreeplayDevice device)
        {
            ToggleEvent original = (ToggleEvent)findFreeplayDeviceEventTarget(toggle.OnToggle, device);
            toggle.OnToggle -= original;
            toggle.OnToggle += new ToggleEvent((bool toggleState) =>
            {
                if (original != null)
                    original(toggleState);
                updateFreeplayDeviceDifficulty(device);
            });
        }

        private Delegate findFreeplayDeviceEventTarget(Delegate source, FreeplayDevice device)
        {
            foreach (Delegate del in source.GetInvocationList())
            {
                if (ReferenceEquals(del.Target, device))
                {
                    return del;
                }
            }
            return null;
        }

        public int FreeplayBombCount
        {
            get
            {
                return freeplaySettings.BombCount;
            }
            set
            {
                freeplaySettings.BombCount = value;
                bombsValue.text = freeplaySettings.BombCount.ToString();
            }
        }
    }
}