using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ComputeSharp;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using Simulator.NET.Core;
using Simulator.NET.LifeGame;
using Simulator.NET.WinUI.ViewModel;
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

namespace Simulator.NET.WinUI.View
{
    public partial class MainView : ObservableObject, IPlayableControl, IResultRenderControl
    {
        SwapBufferEngine<LifeGameItem> lifegameEngine;
        public IEnumerable<GraphicsDevice> Devices => GraphicsDevice.EnumerateDevices();

        [ObservableProperty]
        private GraphicsDevice selectedDevice = GraphicsDevice.GetDefault();

        [ObservableProperty]
        private bool canChangeSettings = true;
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

        public LifeGamResultControl Render { get; } = new();

        [RelayCommand]
        public void Play()
        {
            if (lifegameEngine==null)
            {
                InitEngine();
            }
            FrameCount = 0;
            CanChangeSettings = !CanChangeSettings;
        }

        [RelayCommand(CanExecute ="CanChangeSettings")]
        public void Step()
        {

        }

        private void InitEngine()
        {
            var raw = Random.Shared.GetItems<LifeGameItem>(new LifeGameItem[] { new LifeGameItem() { Value = 1 }, new LifeGameItem() { Value = 0 } }, GridWidth * GridHeight);
            lifegameEngine = new SwapBufferEngine<LifeGameItem>(SelectedDevice, new Size(GridWidth, GridHeight), raw, new LifeGameProcessor());
            lifegameEngine.PostProcessors.Add(Render);
            FrameCount = 0;
        }

        [RelayCommand]
        public void Reset()
        {
            InitEngine();
        }
        [RelayCommand]
        public void Stop()
        {
            
        }
        public void OnDraw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            //CanvasSolidColorBrush aliveBrush = new CanvasSolidColorBrush(args.DrawingSession.Device, Color.FromArgb(255, 0, 0, 0));
            //float unitWidth = (float)(sender.ActualWidth / XCOUNT);
            //float unitHeight = (float)(sender.ActualHeight / YCOUNT);
            CanvasTextFormat format = new CanvasTextFormat();
            format.HorizontalAlignment = CanvasHorizontalAlignment.Center;
            format.VerticalAlignment = CanvasVerticalAlignment.Center;

            if (bitmap == null)
            {
                displayBuffer = new byte[GridWidth * GridHeight * 16]; //w*h*pixel_in_bytes
                bitmap = CanvasBitmap.CreateFromBytes(sender, displayBuffer, GridWidth, GridHeight, Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float);
            }

            var sw = Stopwatch.StartNew();
            var data = render.output;
            sw.Stop();
            MemoryMarshal.AsBytes(data.Span).CopyTo(displayBuffer);

            
            sw.Stop();
            bitmap.SetPixelBytes(displayBuffer);
            args.DrawingSession.DrawImage(bitmap, new Windows.Foundation.Rect(0, 0, sender.ActualWidth, sender.ActualHeight));


            //txtRenderTime.Text = $"{sw.ElapsedMilliseconds}ms";
        }
    }
}
