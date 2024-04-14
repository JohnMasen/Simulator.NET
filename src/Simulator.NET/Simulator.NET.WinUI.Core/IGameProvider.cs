using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.Core
{
    public interface IGameProvider:IPlayableControl
    {
        public UserControl ResultControl { get; }
    }
}
