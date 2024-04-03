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
using Simulator.NET.WinUI.Control;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace Simulator.NET.WinUI.View
{
    public partial class MainView : ObservableObject, IPlayableControl, IRenderControlProvider
    {
        SwapBufferEngine<LifeGameItem> lifegameEngine;
        public IEnumerable<GraphicsDevice> Devices => GraphicsDevice.EnumerateDevices();

        private GraphicsDevice device;
        
        public GraphicsDevice SelectedDevice {
            get 
            {
                return device;
            } 
            set 
            {
                if (SetProperty(ref device, value))
                {
                    InitEngine();
                }
;            } 
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeviceIdle))]
        private bool isDeviceBusy = false;
        public bool IsDeviceIdle => !isDeviceBusy;
        [ObservableProperty]
        private int gridWidth = 4096;
        [ObservableProperty]
        private int gridHeight = 4096;
        [ObservableProperty]
        int frameCount = 0;

        CancellationTokenSource cts;
        CanvasBitmap bitmap;
        byte[] displayBuffer;
        LifeGameRender render;

        public MainView()
        {
            SelectedDevice = GraphicsDevice.GetDefault();
        }

        private LifeGamResultControl resultControl = new();

        public string DisplayName => "Lifegame Result Content";

        public UserControl UIContent => resultControl;

        [RelayCommand]
        public void Play()
        {
            FrameCount = 0;
            IsDeviceBusy = true;
            doStop();
            cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var token = cts.Token;
                while (!token.IsCancellationRequested)
                {
                    lifegameEngine.Step();
                    Task.Delay(1).Wait();
                }
            }, cts.Token);

        }

        [RelayCommand(CanExecute = "IsDeviceIdle")]
        public void Step()
        {
            IsDeviceBusy = true;
            lifegameEngine.Step();
            IsDeviceBusy = false;
        }

        private void InitEngine()
        {
            doStop();
            cts = null;
            var raw = Random.Shared.GetItems<LifeGameItem>(new LifeGameItem[] { new LifeGameItem() { Value = 1 }, new LifeGameItem() { Value = 0 } }, GridWidth * GridHeight);
            lifegameEngine = new SwapBufferEngine<LifeGameItem>(SelectedDevice, new Size(GridWidth, GridHeight), raw, new LifeGameProcessor());
            lifegameEngine.PostProcessors.Add(resultControl);
            FrameCount = 0;
        }

        [RelayCommand]
        public void Reset()
        {
            doStop();
            InitEngine();
            cts = new CancellationTokenSource();
            
        }
        [RelayCommand]
        public void Stop()
        {
            doStop();
            IsDeviceBusy = false;
        }

        private void doStop()
        {
            cts?.Cancel();
            cts = null;
        }
        
    }
}
