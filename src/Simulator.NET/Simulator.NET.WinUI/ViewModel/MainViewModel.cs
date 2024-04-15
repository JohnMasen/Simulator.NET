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

namespace Simulator.NET.WinUI.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        //SwapBufferEngine<LifeGameItem> lifegameEngine;
        SwapBufferSession<LifeGameItem> session;
        public IEnumerable<GraphicsDevice> Devices => GraphicsDevice.EnumerateDevices();
        [ObservableProperty]
        private GraphicsDevice selectedDevice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeviceIdle))]
        private bool isDeviceBusy = false;
        public bool IsDeviceIdle => !IsDeviceBusy;
        [ObservableProperty]
        private int gridWidth = 4096;
        [ObservableProperty]
        private int gridHeight = 4096;
        [ObservableProperty]
        int frameCount = 0;

        [ObservableProperty]
        private ISimulationProvider[] simulationProviders;

        [ObservableProperty]
        private ISimulationProvider currentProvider;

        CancellationTokenSource cts;
        private bool isInitRequired = true;
        public MainViewModel(IEnumerable<ISimulationProvider> providers)
        {
            SimulationProviders = providers.ToArray();
            //SelectedDevice = GraphicsDevice.GetDefault();
        }

        //[ObservableProperty]
        //private WriteableBitmap outputBitmap;

        //private LifeGamResultControl resultControl = new();

        //public string DisplayName => "Lifegame Result Content";

        //public UserControl UIContent => resultControl;

        //[RelayCommand]
        //public void Play()
        //{
        //    tryInit();
        //    FrameCount = 0;
        //    IsDeviceBusy = true;
        //    doStop();
        //    cts = new CancellationTokenSource();
        //    Task.Run(() =>
        //    {
        //        var token = cts.Token;
        //        while (!token.IsCancellationRequested)
        //        {
        //            session.Step();
        //            Task.Delay(1).Wait();
        //            FrameCount++;
        //        }
        //    }, cts.Token);

        //}

        //[RelayCommand(CanExecute = "IsDeviceIdle")]
        //public void Step()
        //{
        //    tryInit();
        //    IsDeviceBusy = true;
        //    session.Step();
        //    IsDeviceBusy = false;
        //}

        //private void InitEngine()
        //{
        //    doStop();
        //    cts = null;
        //    var raw = Random.Shared.GetItems(new LifeGameItem[] { new LifeGameItem() { Value = 1 }, new LifeGameItem() { Value = 0 } }, GridWidth * GridHeight);
        //    session = new SwapBufferSession<LifeGameItem>(SelectedDevice,new(GridHeight,GridWidth), raw, new LifeGameProcessor(),[resultControl]);
            
        //    //lifegameEngine = SwapBufferEngineBuilder.Create<LifeGameItem>(opt =>
        //    //{
        //    //    opt.Device = SelectedDevice;
        //    //    opt.BufferSize = new Size(GridWidth, GridHeight);
        //    //    opt.Data = raw;
        //    //    opt.Processor = new LifeGameProcessor();
        //    //    opt.PostProcessors.Add(resultControl);
        //    //});
        //    //lifegameEngine = new SwapBufferEngine<LifeGameItem>(SelectedDevice, new Size(GridWidth, GridHeight), raw, new LifeGameProcessor(),new List<IPostProcessor<LifeGameItem>>() { resultControl});
        //    //lifegameEngine.PostProcessors.Add(resultControl);
        //    FrameCount = 0;
        //    //outputBitmap = new WriteableBitmap(GridWidth, GridHeight);
        //}

        //[RelayCommand]
        //public void Reset()
        //{
        //    doStop();
        //    InitEngine();
        //    cts = new CancellationTokenSource();
            
        //}
        //[RelayCommand]
        //public void Stop()
        //{
        //    doStop();
        //    IsDeviceBusy = false;
        //}

        //private void doStop()
        //{
        //    cts?.Cancel();
        //    cts = null;
        //}
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private void tryInit()
        //{
        //    if (isInitRequired)
        //    {
        //        InitEngine();
        //    }
        //    isInitRequired = false;
        //}
        
    }
}
