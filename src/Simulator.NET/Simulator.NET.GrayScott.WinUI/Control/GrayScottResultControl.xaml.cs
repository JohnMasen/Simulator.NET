using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp;
using ComputeSharp.Resources;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Simulator.NET.Core;
using Simulator.NET.GrayScott.WinUI;
using Simulator.NET.WinUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.GrayScott.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class GrayScottResultControl : UserControl, IPostProcessor<GrayScottItem>
    {
        [ObservableProperty]
        private WriteableBitmap bitmapOutput;
        [ObservableProperty]
        private int frameCount;

        private int frameSkip = 0;
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
            initAsync(device, size).Wait();
        }
        private async Task initAsync(GraphicsDevice device, System.Drawing.Size size)
        {
            BitmapOutput = new WriteableBitmap(size.Width, size.Height);
            DispatcherQueue.RunAndWait(() =>
            {
                imgOutput.Width = size.Width;
                imgOutput.Height = size.Height;
                FrameCount = 0;
            });
            var colorMap = await LoadTextureFromStream(device, new MemoryStream(Resource1.RenderColorMapping)).ConfigureAwait(false);
            render = new GrayScottRender<Bgra32>(buffer =>
            {
                DispatcherQueue.RunAndWait(() =>
                {
                    BitmapOutput.CopyFromSpan(buffer.Span);
                    BitmapOutput.Invalidate();
                });
            }, colorMap.buffer,colorMap.size);
            render.Init(device, size);
        }

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<GrayScottItem> data)
        {
            if (frameSkip==0 || FrameCount % frameSkip==0)
            {
                render.Process(in ctx, data);
                DispatcherQueue.RunAndWait(() =>
                {
                    FrameCount++;
                });
            }
            else
            {
                frameCount++;
            };
            
        }
        private async Task<(Bgra32[] buffer, System.Drawing.Size size)> LoadTextureFromStream(GraphicsDevice device, Stream s)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(s.AsRandomAccessStream()).AsTask().ConfigureAwait(false);
            BitmapTransform transform = new BitmapTransform();
            var pixels = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
            int w = (int)decoder.PixelWidth;
            int h = (int)decoder.PixelHeight;
            Bgra32[] output=new Bgra32[w * h];
            pixels.DetachPixelData().CopyTo(MemoryMarshal.Cast<Bgra32, byte>(output));
            return (output, new System.Drawing.Size(w, h));
        }

    }
}
