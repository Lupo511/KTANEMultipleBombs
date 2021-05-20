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

        public ResourceManager(ModHelper modHelper, CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            this.modHelper = modHelper;
        }

        public void LoadResources()
        {
            using (FileStream fileStream = File.OpenRead(modHelper.Mod.GetModPath() + "/Resources.bin"))
            {
                resourceCollection = new ResourceReader(fileStream).ReadResources(cultureInfo);
            }
        }
    }
}
