using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace MultipleBombsAssembly
{
    //This does not have the exact behaviour of Unity WaitForSeconds but it should be good enough for our needs
    public class CoroutineTimeDelay : ICoroutineYieldable
    {
        private int isElapsed;

        public bool IsFinished
        {
            get
            {
                return Interlocked.CompareExchange(ref isElapsed, 1, 1) == 1;
            }
        }

        public CoroutineTimeDelay(float secondsDelay)
        {
            System.Timers.Timer timer = new System.Timers.Timer(secondsDelay * 1000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Interlocked.Exchange(ref isElapsed, 1);
        }
    }
}
