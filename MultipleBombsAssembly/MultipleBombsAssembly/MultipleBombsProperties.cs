using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class MultipleBombsProperties : PropertiesBehaviour
    {
        internal MultipleBombs MultipleBombs { get; set; }

        public MultipleBombsProperties()
        {
            AddProperty("CurrentMaximumBombCount", () => MultipleBombsModManager.GetMaximumBombs(), null);
            AddProperty("CurrentFreePlayBombCount", () => { return MultipleBombs.CurrentFreeplayBombCount; }, (object value) => { MultipleBombs.CurrentFreeplayBombCount = (int)value; });
        }
    }
}
