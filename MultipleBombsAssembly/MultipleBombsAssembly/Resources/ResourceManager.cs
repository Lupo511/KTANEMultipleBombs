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
        private CultureInfo cultureInfo;
        private ResourceCollection resourceCollection;

        public ResourceManager(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        public void LoadResources()
        {
            using (FileStream fileStream = File.OpenRead(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Resources.bin"))
            {
                resourceCollection = new ResourceReader(fileStream).ReadResources(cultureInfo);
            }
        }
    }
}
