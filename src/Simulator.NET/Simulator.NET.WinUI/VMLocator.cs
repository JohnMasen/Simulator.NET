using Simulator.NET.WinUI.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI
{
    internal  class VMLocator
    {
        internal  MainView MainView  => new MainView();

        public static VMLocator Instance { get; private set; } = new VMLocator();
    }
}
