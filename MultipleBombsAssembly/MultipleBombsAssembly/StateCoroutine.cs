using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class StateCoroutine
    {
        private bool completed;
        private IEnumerator<ICoroutineYieldable> enumerator;

        public StateCoroutine(IEnumerator<ICoroutineYieldable> enumerator)
        {
            this.enumerator = enumerator;
            if (!enumerator.MoveNext())
                completed = true;
        }

        public bool CoroutineUpdate()
        {
            if (completed)
                return true;

            if (enumerator.Current != null)
                enumerator.Current.Update();

            if (enumerator.Current == null || enumerator.Current.IsFinished)
                completed = !enumerator.MoveNext();

            return completed;
        }
    }
}
