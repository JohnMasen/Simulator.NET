using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.ViewModel
{
    internal interface IPlayableControl
    {
        void Play();
        void Stop();
        void Reset();
    }
}
