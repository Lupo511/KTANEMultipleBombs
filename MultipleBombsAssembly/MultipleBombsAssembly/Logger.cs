using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Debug.Log("[MultipleBombs]" + message);
        }
    }
}
