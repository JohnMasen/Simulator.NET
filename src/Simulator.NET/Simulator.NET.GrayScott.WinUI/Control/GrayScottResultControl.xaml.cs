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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Simulator.NET.GrayScott.WinUI;
using Simulator.NET.WinUI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.GrayScott.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class GrayScottResultControl : UserControl, IPostProcessor<GrayScottItem>
    {
        [ObservableProperty]
        WriteableBitmap bitmapOutput;

        private GrayScottRender<Bgra32> render;

        bool IProcessor.IsEnabled { get; set; } = true;
        
        public GrayScottResultControl()
        {
            InitializeComponent();
        }

        
        public void AfterProcess(GraphicsDevice device)
        {
            render.BeforeProcess(device);
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            render.AfterProcess(device);
        }

        public void Init(GraphicsDevice device, System.Drawing.Size size)
        {
            BitmapOutput = new WriteableBitmap(size.Width, size.Height);
            DispatcherQueue.RunAndWait(() =>
            {
                imgOutput.Width = size.Width;
                imgOutput.Height = size.Height;
            });
            render = new GrayScottRender<Bgra32>(buffer =>
            {
                DispatcherQueue.RunAndWait(()=>
                {
                    BitmapOutput.CopyFromSpan(buffer.Span);
                    BitmapOutput.Invalidate();
                });
            },new Bgra32(0,0,0,1));
            render.Init(device,size);
        }

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<GrayScottItem> data)
        {
            render.Process(in ctx, data);
        }
    }
}
