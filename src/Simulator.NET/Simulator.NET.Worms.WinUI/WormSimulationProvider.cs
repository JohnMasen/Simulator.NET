using ComputeSharp;
using ComputeSharp.Descriptors;
using ComputeSharp.Interop;
using Microsoft.UI.Xaml.Controls;
using Simulator.NET.Core;
using Simulator.NET.WinUI.Core;
using Simulator.NET.Worms.WinUI.Control;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Worms.WinUI
{
    public class WormSimulationProvider : ISimulationProvider
    {
        private WormRenderControl render;
        private SimulationController controller;
        private WormConfigControl config;

        public WormSimulationProvider()
        {
            render = new WormRenderControl();
            config = new();
            controller = new SimulationController(() =>
            {
                Size size = new Size(config.GridWidth, config.GridHeight);
                var raw = WormItem.CreateRandomWorms(config.WormsCount, size).Span;

                return new SwapBufferSession<WormItem>(
                    GraphicsDevice.GetDefault(),
                    size,
                    raw,
                    new WormProcessor(config.WormsCount, new(config.GridWidth, config.GridHeight),config.NavigationCapacity,config.Stability)
                    , [render
                    //, new DebuggerProcessor<WormItem>(config.WormsCount, items =>
                    //{
                    //    int count = 0;
                    //    foreach (var item in items.Span)
                    //    {
                    //        Debug.WriteLine($"[{count++}].TargetPosition={item.TargetPosition},Position={item.HeadPosition}, Randoms={item.Randoms}");
                    //    }
                    //})
                    ]
                    );
            });
        }
        public UserControl ResultControl => render;

        public UserControl ConfigPanel => config;

        public ISimulationController SimulationController => controller;
        public override string ToString() => "Worm Simulator";

    }
}
