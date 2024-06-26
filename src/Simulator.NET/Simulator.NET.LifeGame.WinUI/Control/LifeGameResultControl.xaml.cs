using ABI.Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Simulator.NET.Core;
using Simulator.NET.LifeGame;
using Simulator.NET.WinUI.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Size = System.Drawing.Size;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.LifeGame.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class LifeGameResultControl : UserControl,IPostProcessor<LifeGameItem>
    {
        [ObservableProperty]
        WriteableBitmap bitmapOutput;

        private LifeGameRender<Bgra32> render;

        bool IProcessor.IsEnabled { get; set; } = true;

        public LifeGameResultControl()
        {
            this.InitializeComponent();
        }


        

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<LifeGameItem> data)
        {
            render.Process(in ctx, data);
        }

        public void Init(GraphicsDevice device,Size size)
        {
            BitmapOutput = new WriteableBitmap(size.Width, size.Height);
            DispatcherQueue.RunAndWait(() =>
            {
                imgOutput.Width = size.Width;
                imgOutput.Height = size.Height;
            });
            render = new LifeGameRender<Bgra32>(buffer =>
            {
                DispatcherQueue.RunAndWait(() =>
                {
                    BitmapOutput.CopyFromSpan(buffer.Span);
                    BitmapOutput.Invalidate();
                });
            },new(0,0,0,1),new(1,1,1,1));
            render.Init(device, size);
            
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            render.BeforeProcess(device);
        }

        public void AfterProcess(GraphicsDevice device)
        {
            render.AfterProcess(device);
        }
    }
}
