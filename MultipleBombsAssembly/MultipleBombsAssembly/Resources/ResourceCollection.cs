using MultipleBombsAssembly.Internationalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly.Resources
{
    public class ResourceCollection
    {
        public Dictionary<string, string> Strings { get; }
        public Dictionary<string, PluralString> PluralStrings { get; }

        public ResourceCollection()
        {
            Strings = new Dictionary<string, string>();
            PluralStrings = new Dictionary<string, PluralString>();
        }
    }
}
