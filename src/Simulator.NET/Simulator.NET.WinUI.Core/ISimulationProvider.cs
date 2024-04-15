using Microsoft.UI.Xaml.Controls;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.Core
{
    public interface ISimulationProvider
    {
        public UserControl ResultControl { get; }

        public UserControl ConfigPanel { get; }

        public ISimulationController SimulationController { get; }
    }
}
