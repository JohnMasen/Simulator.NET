using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp;
using Microsoft.UI.Xaml.Controls;
using Simulator.NET.Core;
using Simulator.NET.GrayScott.WinUI.Control;
using Simulator.NET.WinUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.GrayScott.WinUI
{
    public class GrayScottSimulationProvider : ObservableObject, ISimulationProvider
    {

        public UserControl ResultControl { get;private set; }

        public UserControl ConfigPanel { get; private set; }

        public ISimulationController SimulationController { get; private set; }
        public GrayScottSimulationProvider()
        {
            var config= new GrayScottConfigControl();
            var result=new GrayScottResultControl();
            ConfigPanel = config;
            ResultControl = result;
            //List<GrayScottItem> items = new List<GrayScottItem>()
            //{
            //    new GrayScottItem(){U=0,V=0.5f},
            //    new GrayScottItem(){U=0,V=0},
            //    new GrayScottItem(){U=0.5f,V=0}
            //};

            SimulationController = new SimulationController(() =>
            {
                //var raw = Random.Shared.GetItems<GrayScottItem>(items.ToArray().AsSpan(), config.DataGridHeight*config.DataGridWidth);
                return new SwapBufferSession<GrayScottItem>(GraphicsDevice.GetDefault(),
                    new System.Drawing.Size(config.DataGridWidth, config.DataGridHeight),
                    config.CreateStartupItems().Span,
                    new GrayScottProcessor(config.Feed, config.KillRate),
                    [result]
                    );
            });

        }
        public override string ToString()
        {
            return nameof(GrayScottSimulationProvider);
        }
    }
}
