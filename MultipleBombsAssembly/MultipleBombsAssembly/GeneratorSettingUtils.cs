using Assets.Scripts.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class GeneratorSettingUtils
    {
        public static KMGeneratorSetting CreateModFromGeneratorSetting(GeneratorSetting generatorSetting)
        {
            KMGeneratorSetting modSetting = new KMGeneratorSetting();
            modSetting.FrontFaceOnly = generatorSetting.FrontFaceOnly;
            modSetting.NumStrikes = generatorSetting.NumStrikes;
            modSetting.TimeBeforeNeedyActivation = generatorSetting.TimeBeforeNeedyActivation;
            modSetting.TimeLimit = generatorSetting.TimeLimit;
            modSetting.OptionalWidgetCount = generatorSetting.OptionalWidgetCount;
            modSetting.ComponentPools = new List<KMComponentPool>();
            foreach (ComponentPool pool in generatorSetting.ComponentPools)
            {
                KMComponentPool modPool = new KMComponentPool();
                modPool.Count = pool.Count;
                modPool.SpecialComponentType = (KMComponentPool.SpecialComponentTypeEnum)pool.SpecialComponentType;
                modPool.AllowedSources = (KMComponentPool.ComponentSource)pool.AllowedSources;
                modPool.ComponentTypes = new List<KMComponentPool.ComponentTypeEnum>();
                if (pool.ComponentTypes != null)
                {
                    foreach (ComponentTypeEnum componentType in pool.ComponentTypes)
                    {
                        modPool.ComponentTypes.Add((KMComponentPool.ComponentTypeEnum)componentType);
                    }
                }
                modPool.ModTypes = new List<string>();
                if (pool.ModTypes != null)
                    modPool.ModTypes.AddRange(pool.ModTypes);
                modSetting.ComponentPools.Add(modPool);
            }
            return modSetting;
        }
    }
}
