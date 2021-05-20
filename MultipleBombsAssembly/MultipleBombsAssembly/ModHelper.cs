using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MultipleBombsAssembly
{
    public class ModHelper
    {
        private static FieldInfo loadedModsField = typeof(ModManager).GetField("loadedMods", BindingFlags.Instance | BindingFlags.NonPublic);
        private string modId;
        private Mod mod;

        public ModHelper(string modId)
        {
            this.modId = modId;
        }

        public Mod Mod
        {
            get
            {
                if (mod == null)
                    foreach (Mod gameMod in ((Dictionary<string, Mod>)loadedModsField.GetValue(ModManager.Instance)).Values)
                        if (gameMod.ModID == modId)
                            mod = gameMod;

                return mod;
            }
        }
    }
}
