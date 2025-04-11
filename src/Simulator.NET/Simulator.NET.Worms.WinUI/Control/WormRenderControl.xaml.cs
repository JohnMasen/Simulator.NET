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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Foundation;
using Windows.Foundation.Collections;
using Simulator.NET.WinUI.Core;
using System.Drawing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.Worms.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class WormRenderControl : UserControl,IPostProcessor<WormItem>
    {
        [ObservableProperty]
        WriteableBitmap bitmapOutput;

        private WormRender<Bgra32> render;
        public float4 HeadColor;
        bool IProcessor.IsEnabled { get; set; } = true;

        public WormRenderControl()
        {
            this.InitializeComponent();
        }




        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<WormItem> data)
        {
            render.Process(in ctx, data);
        }

        public void Init(GraphicsDevice device, Size size)
        {
            BitmapOutput = new WriteableBitmap(size.Width, size.Height);
            DispatcherQueue.RunAndWait(() =>
            {
                imgOutput.Width = size.Width;
                imgOutput.Height = size.Height;
            });
            render = new WormRender<Bgra32>(buffer =>
            {
                DispatcherQueue.RunAndWait(() =>
                {
                    BitmapOutput.CopyFromSpan(buffer.Span);
                    BitmapOutput.Invalidate();
                });
            },new Bgra32(0,0,0,255),new float4(1,1,1,1),HeadColor);
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
