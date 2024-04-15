using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ComputeSharp;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using Simulator.NET.Core;
using Simulator.NET.LifeGame;
using Simulator.NET.WinUI.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Simulator.NET.LifeGame.WinUI.Control;
using Microsoft.UI.Dispatching;

namespace Simulator.NET.WinUI.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        //SwapBufferEngine<LifeGameItem> lifegameEngine;
        //SwapBufferSession<LifeGameItem> session;
        //public IEnumerable<GraphicsDevice> Devices => GraphicsDevice.EnumerateDevices();
        //[ObservableProperty]
        //private GraphicsDevice selectedDevice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeviceIdle))]
        private bool isDeviceBusy = false;
        public bool IsDeviceIdle => !IsDeviceBusy;
        //[ObservableProperty]
        //private int gridWidth = 4096;
        //[ObservableProperty]
        //private int gridHeight = 4096;
        //[ObservableProperty]
        //int frameCount = 0;

        private DispatcherQueue queue;

        [ObservableProperty]
        private ISimulationProvider[] simulationProviders;

        public ISimulationProvider CurrentProvider
        {
            get => currentProvider;
            set
            {
                var lastValue = currentProvider;
                if (SetProperty(ref currentProvider, value, nameof(CurrentProvider)))
                {
                    currentProvider.SimulationController.OnRunningStatusChanged += OnRunningStatusChangedHandler;
                    if (lastValue != null)
                    {
                        lastValue.SimulationController.OnRunningStatusChanged -= OnRunningStatusChangedHandler;
                    }
                }
            }
        }

        private void OnRunningStatusChangedHandler(object _, bool value)
        {
            queue.RunAndWait(() =>
            {
                IsDeviceBusy = value;
            });
        }
        private ISimulationProvider currentProvider;

        CancellationTokenSource cts;
        private bool isInitRequired = true;
        public MainViewModel(IEnumerable<ISimulationProvider> providers)
        {
            SimulationProviders = providers.ToArray();
            if (SimulationProviders?.Length > 0)
            {
                CurrentProvider = SimulationProviders[0];
            }
            queue = DispatcherQueue.GetForCurrentThread();
            //SelectedDevice = GraphicsDevice.GetDefault();
        }



    }
}
