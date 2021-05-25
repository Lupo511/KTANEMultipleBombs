using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly.Internationalization
{
    public class PluralString
    {
        private string[] strings;

        public PluralString()
        {
            strings = new string[6];
        }

        public PluralString(string zero, string one, string two, string few, string many, string other)
        {
            strings = new string[6] { zero, one, two, few, many, other };
        }

        public string GetString(PluralQuantity quantity)
        {
            return strings[(int)quantity];
        }
    }
}
