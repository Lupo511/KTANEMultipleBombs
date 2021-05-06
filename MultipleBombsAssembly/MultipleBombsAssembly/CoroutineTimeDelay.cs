using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class CoroutineTimeDelay : ICoroutineYieldable
    {
        private float secondsDelay;
        private float timeElapsed;

        public bool IsFinished
        {
            get
            {
                return timeElapsed >= secondsDelay;
            }
        }

        public CoroutineTimeDelay(float secondsDelay)
        {
            this.secondsDelay = secondsDelay;
        }

        public void Update()
        {
            timeElapsed += Time.deltaTime;
        }
    }
}
