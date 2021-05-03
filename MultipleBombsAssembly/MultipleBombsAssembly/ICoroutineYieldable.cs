using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public interface ICoroutineYieldable
    {
        bool IsFinished { get; }

        void Update();
    }
}
