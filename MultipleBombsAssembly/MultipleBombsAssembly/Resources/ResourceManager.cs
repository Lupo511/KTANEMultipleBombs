using MultipleBombsAssembly.Internationalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MultipleBombsAssembly.Resources
{
    public class ResourceManager
    {
        private ModHelper modHelper;
        private CultureInfo cultureInfo;
        private ResourceCollection resourceCollection;
        private IPluralRule pluralRule;

        public ResourceManager(ModHelper modHelper, CultureInfo cultureInfo)
        {
            this.modHelper = modHelper;
            this.cultureInfo = cultureInfo;
            pluralRule = PluralRuleProvider.GetPluralRuleFromLanguage(cultureInfo.TwoLetterISOLanguageName);
        }

        public void LoadResources()
        {
            using (FileStream fileStream = File.OpenRead(modHelper.Mod.GetModPath() + "/Resources.bin"))
            {
                resourceCollection = new ResourceReader(fileStream).ReadResources(cultureInfo);
            }
        }

        public string GetString(string resourceId)
        {
            try
            {
                return resourceCollection.Strings[resourceId];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException();
            }
        }

        public PluralString GetPluralString(string resourceId)
        {
            try
            {
                return resourceCollection.PluralStrings[resourceId];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException();
            }
        }

        public string GetPluralStringValue(string resourceId, double quantity)
        {
            try
            {
                return resourceCollection.PluralStrings[resourceId].GetString(pluralRule.GetQuantity(quantity));
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
