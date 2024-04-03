using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.Core
{
    public interface IPlayableControl
    {
        void Play();
        void Stop();
        void Reset();
    }
}
