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
    public class ResourceReader
    {
        private Stream stream;

        public ResourceReader(Stream stream)
        {
            this.stream = stream;
        }

        public ResourceCollection ReadResources(CultureInfo cultureInfo)
        {
            byte[] languageCodeBytes = Encoding.ASCII.GetBytes(cultureInfo.TwoLetterISOLanguageName);
            byte[] regionCodeBytes = new byte[2] { 0, 0 };
            if (!cultureInfo.IsNeutralCulture)
            {
                regionCodeBytes = Encoding.ASCII.GetBytes(new RegionInfo(cultureInfo.LCID).TwoLetterISORegionName);
            }

            ResourceCollection resourceCollection = new ResourceCollection();

            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                int defaultResourceListStartOffset = binaryReader.ReadInt32();

                int? neutralResourceListStartOffset = null;
                int? regionResourceListStartOffset = null;

                //Find the culture specific resources start offset (if present)
                int resourceListsCount = binaryReader.ReadInt32();
                for (int i = 0; i < resourceListsCount; i++)
                {
                    byte[] currentLanguageCodeBytes = binaryReader.ReadBytes(2);
                    byte[] currentRegionCodeBytes = binaryReader.ReadBytes(2);
                    int currentResourceListStartOffset = binaryReader.ReadInt32();

                    if (currentLanguageCodeBytes[0] == languageCodeBytes[0] && currentLanguageCodeBytes[1] == languageCodeBytes[1])
                    {
                        if (currentRegionCodeBytes[0] == 0 && currentRegionCodeBytes[1] == 0)
                        {
                            neutralResourceListStartOffset = currentResourceListStartOffset;

                            if (regionResourceListStartOffset != null || cultureInfo.IsNeutralCulture)
                                break;
                        }
                        else if (currentRegionCodeBytes[0] == regionCodeBytes[0] && currentRegionCodeBytes[1] == regionCodeBytes[1])
                        {
                            regionResourceListStartOffset = currentResourceListStartOffset;

                            if (neutralResourceListStartOffset != null)
                                break;
                        }
                    }
                }

                stream.Seek(defaultResourceListStartOffset, SeekOrigin.Begin);
                readResourceList(binaryReader, resourceCollection);

                if (neutralResourceListStartOffset != null)
                {
                    stream.Seek((int)neutralResourceListStartOffset, SeekOrigin.Begin);
                    readResourceList(binaryReader, resourceCollection);
                }

                if (regionResourceListStartOffset != null)
                {
                    stream.Seek((int)regionResourceListStartOffset, SeekOrigin.Begin);
                    readResourceList(binaryReader, resourceCollection);
                }
            }

            return resourceCollection;
        }

        private void readResourceList(BinaryReader binaryReader, ResourceCollection resourceCollection)
        {
            int resourceCount = binaryReader.ReadInt32();
            for (int i = 0; i < resourceCount; i++)
            {
                byte resourceType = binaryReader.ReadByte();

                int resourceIdByteCount = binaryReader.ReadInt32();
                string resourceId = Encoding.UTF8.GetString(binaryReader.ReadBytes(resourceIdByteCount));

                switch ((ResourceType)resourceType)
                {
                    case ResourceType.String:
                        int stringByteCount = binaryReader.ReadInt32();
                        resourceCollection.Strings[resourceId] = Encoding.UTF8.GetString(binaryReader.ReadBytes(stringByteCount));
                        break;
                    case ResourceType.PluralString:
                        string[] strings = new string[6];
                        for (int j = 0; j < 6; j++)
                        {
                            int stringLength = binaryReader.ReadInt32();
                            if (stringLength > 0)
                                strings[j] = Encoding.UTF8.GetString(binaryReader.ReadBytes(stringLength));
                        }
                        resourceCollection.PluralStrings[resourceId] = new PluralString(strings[0], strings[1], strings[2], strings[3], strings[4], strings[5]);
                        break;
                    default:
                        throw new Exception("Unknown resource type: " + resourceType + " at offset 0x" + binaryReader.BaseStream.Position.ToString("X8"));
                }
            }
        }
    }
}
