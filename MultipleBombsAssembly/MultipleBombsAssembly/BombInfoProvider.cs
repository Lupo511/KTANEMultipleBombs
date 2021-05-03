using Assets.Scripts.Missions;
using Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class BombInfoProvider
    {
        private int bombCount = 1;
        private Dictionary<Bomb, BombEvents.BombSolvedEvent> bombSolvedEvents;

        public BombInfoProvider(int bombCount)
        {
            this.bombCount = bombCount;
            bombSolvedEvents = new Dictionary<Bomb, BombEvents.BombSolvedEvent>();
        }

        public void RedirectBombInfo(KMBombInfo bombInfo, Bomb bomb)
        {
            ModBombInfo modBombInfo = bombInfo.GetComponent<ModBombInfo>();

            DelegateUtils.RemoveAdd(ref bombInfo.TimeHandler, modBombInfo.GetTime, () => GetTime(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.FormattedTimeHandler, modBombInfo.GetFormattedTime, () => GetFormattedTime(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.StrikesHandler, modBombInfo.GetStrikes, () => GetStrikes(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.ModuleNamesHandler, modBombInfo.GetModuleNames, () => GetModuleNames(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.SolvableModuleNamesHandler, modBombInfo.GetSolvableModuleNames, () => GetSolvableModuleNames(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.SolvedModuleNamesHandler, modBombInfo.GetSolvedModuleNames, () => GetSolvedModuleNames(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.ModuleIDsHandler, modBombInfo.GetModuleTypes, () => GetModuleTypes(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.SolvableModuleIDsHandler, modBombInfo.GetSolvableModuleTypes, () => GetSolvableModuleTypes(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.SolvedModuleIDsHandler, modBombInfo.GetSolvedModuleTypes, () => GetSolvedModuleTypes(bomb));
            DelegateUtils.RemoveAdd(ref bombInfo.WidgetQueryResponsesHandler, modBombInfo.GetWidgetQueryResponses, (string queryKey, string queryInfo) => GetWidgetQueryResponses(bomb, queryKey, queryInfo));
            DelegateUtils.ReplaceFromTarget(ref bombInfo.IsBombPresentHandler, modBombInfo, () => IsBombPresent(bomb));

            BombEvents.BombSolvedEvent modBombInfoSolvedEvent = DelegateUtils.RemoveFromTarget(ref BombEvents.OnBombSolved, modBombInfo);
            if (bombSolvedEvents.ContainsKey(bomb))
                bombSolvedEvents[bomb] += modBombInfoSolvedEvent;
            else
                bombSolvedEvents.Add(bomb, modBombInfoSolvedEvent);
        }

        internal float GetTime(Bomb bomb)
        {
            if (bomb == null)
                return 0f;
            return bomb.GetTimer().TimeRemaining;
        }

        internal string GetFormattedTime(Bomb bomb)
        {
            if (bomb == null)
                return "";
            return bomb.GetTimer().GetFormattedTime(bomb.GetTimer().TimeRemaining, true);
        }

        internal int GetStrikes(Bomb bomb)
        {
            if (bomb == null)
                return 0;
            return bomb.NumStrikes;
        }

        internal List<string> GetModuleNames(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.ComponentType != ComponentTypeEnum.Empty && component.ComponentType != ComponentTypeEnum.Timer)
                {
                    modules.Add(component.GetModuleDisplayName());
                }
            }
            return modules;
        }

        internal List<string> GetSolvableModuleNames(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.IsSolvable)
                {
                    modules.Add(component.GetModuleDisplayName());
                }
            }
            return modules;
        }

        internal List<string> GetSolvedModuleNames(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.IsSolvable && component.IsSolved)
                {
                    modules.Add(component.GetModuleDisplayName());
                }
            }
            return modules;
        }

        internal List<string> GetModuleTypes(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.ComponentType != ComponentTypeEnum.Empty && component.ComponentType != ComponentTypeEnum.Timer)
                {
                    modules.Add(GetBombComponentModuleType(component));
                }
            }
            return modules;
        }

        internal List<string> GetSolvableModuleTypes(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.IsSolvable)
                {
                    modules.Add(GetBombComponentModuleType(component));
                }
            }
            return modules;
        }

        internal List<string> GetSolvedModuleTypes(Bomb bomb)
        {
            List<string> modules = new List<string>();
            if (bomb == null)
                return modules;
            foreach (BombComponent component in bomb.BombComponents)
            {
                if (component.IsSolvable && component.IsSolved)
                {
                    modules.Add(GetBombComponentModuleType(component));
                }
            }
            return modules;
        }

        internal List<string> GetWidgetQueryResponses(Bomb bomb, string queryKey, string queryInfo)
        {
            List<string> responses = new List<string>();
            if (bomb == null)
                return responses;
            foreach (Widget widget in bomb.WidgetManager.GetWidgets())
            {
                string queryResponse = widget.GetQueryResponse(queryKey, queryInfo);
                if (queryResponse != null && queryResponse != "")
                    responses.Add(queryResponse);
            }
            if (queryKey == "MultipleBombs")
            {
                Dictionary<string, int> response = new Dictionary<string, int>();
                response.Add("bombCount", bombCount);
                responses.Add(JsonConvert.SerializeObject(response));
            }
            return responses;
        }

        internal bool IsBombPresent(Bomb bomb)
        {
            return bomb != null;
        }

        private string GetBombComponentModuleType(BombComponent component)
        {
            if (component.ComponentType == ComponentTypeEnum.Mod)
            {
                ModBombComponent modComponent = component.GetComponent<ModBombComponent>();
                if (modComponent != null)
                {
                    return modComponent.GetModComponentType();
                }
            }
            else if (component.ComponentType == ComponentTypeEnum.NeedyMod)
            {
                ModNeedyComponent modNeedyComponent = component.GetComponent<ModNeedyComponent>();
                if (modNeedyComponent != null)
                {
                    return modNeedyComponent.GetModComponentType();
                }
            }
            else
            {
                return component.ComponentType.ToString();
            }
            return "Unknown";
        }

        public void BombSolved(Bomb bomb)
        {
            if (bombSolvedEvents.ContainsKey(bomb))
                bombSolvedEvents[bomb]();
        }
    }
}
