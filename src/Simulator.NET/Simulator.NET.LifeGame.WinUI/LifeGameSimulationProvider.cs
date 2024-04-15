using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Simulator.NET.Core;
using Simulator.NET.LifeGame.WinUI.Control;
using Simulator.NET.WinUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.LifeGame.WinUI
{
    public class LifeGameSimulationProvider : ObservableObject, ISimulationProvider
    {
        public UserControl ResultControl { get; private set; }

        public UserControl ConfigPanel { get; private set; }

        public ISimulationController SimulationController { get; private set; }
        public LifeGameSimulationProvider()
        {
            LifeGameResultControl resultControl = new();
            LifeGameConfigControl configControl = new();

            ResultControl = resultControl;
            ConfigPanel = configControl;
            SimulationController = new SimulationController(() =>
            {
                var raw = Random.Shared.GetItems(new LifeGameItem[] { new LifeGameItem() { Value = 1 }, new LifeGameItem() { Value = 0 } }, configControl.DataGridWidth * configControl.DataGridHeight);
                return new SwapBufferSession<LifeGameItem>(configControl.SelectedDevice,
                    new System.Drawing.Size(configControl.DataGridWidth, configControl.DataGridHeight),
                    raw,
                    new LifeGameProcessor(),
                    [resultControl]);
            });
        }
        public override string ToString()
        {
            return nameof(LifeGameSimulationProvider);
        }
    }
}
