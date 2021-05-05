using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class StartNotifier : MonoBehaviour
    {
        public event Action OnStart;

        public void Start()
        {
            if (OnStart != null)
                OnStart();
        }
    }
}
