using ABI.Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
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
using Simulator.NET.WinUI.Helper;
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

namespace Simulator.NET.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class LifeGamResultControl : UserControl,IPostProcessor<LifeGameItem>
    {
        #region Canvas Variables
        CanvasBitmap bitmap;
        byte[] backBuffer;
        #endregion

        #region LifeGameRender variables
        private readonly float4 alive = new(0,0,0,1);
        private readonly float4 dead = new(1,1,1,1);
        private ReadWriteTexture2D<Bgra32, float4> texture;
        //ReadWriteBuffer<int> texture;
        public ReadBackBuffer<int> output;
        #endregion

        private Size backBufferSize;
        AutoResetEvent aseResize = new(false);
        [ObservableProperty]
        WriteableBitmap bitmapOutput;
        //public LifeGameRender Render { get; set; }



        public LifeGamResultControl()
        {
            this.InitializeComponent();
            this.Unloaded += LifeGamResultControl_Unloaded;
        }

        private void LifeGamResultControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //canvas1.RemoveFromVisualTree();
            //canvas1 = null;
        }

        private void CanvasControl_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            if (output == null)//not initlized
            {
                Debug.WriteLine("skipped drawing");
                return;
            }
            //var sw = Stopwatch.StartNew();
            
            args.DrawingSession.DrawImage(bitmap, new Windows.Foundation.Rect(0, 0, sender.ActualWidth, sender.ActualHeight));
            
        }

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<LifeGameItem> data)
        {
            var shader = new LifeGameRenderShader(data, texture, backBufferSize.Width, alive, dead);
            //ctx.Barrier(data);
            ctx.For(backBufferSize.Width, backBufferSize.Height, shader);
            //ctx.Barrier(texture);
        }

        public void Init(GraphicsDevice device,Size size)
        {
            DispatcherQueue.RunAndWait(() =>
            {
                imgOutput.Width = size.Width;
                imgOutput.Height = size.Height;
            });
            backBufferSize = size;
            backBuffer = new byte[size.Width * size.Height * Marshal.SizeOf<int>()];
            //TODO:possible bug, CreateFromBytes may called before canvas device is initlized
            //bitmap = CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(), backBuffer, size.Width, size.Height, Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float); ;
            texture = device.AllocateReadWriteTexture2D<Bgra32,float4>(size.Width , size.Height);
            output = device.AllocateReadBackBuffer<int>(size.Width * size.Height);
            BitmapOutput = new WriteableBitmap(size.Width, size.Height);
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            //throw new NotImplementedException();
        }

        public void AfterProcess(GraphicsDevice device)
        {   
            var sw = Stopwatch.StartNew();
            //output.Span.CopyTo()
            texture.CopyTo(MemoryMarshal.Cast<byte, Bgra32>(backBuffer.AsMemory().Span));
            //texture.CopyTo(output);
            sw.Stop();
            //MemoryMarshal.AsBytes(output.Span).CopyTo(backBuffer);
            DispatcherQueue.RunAndWait(() =>
            {
                backBuffer.AsBuffer().CopyTo(BitmapOutput.PixelBuffer);
                BitmapOutput.Invalidate();
            });
            
            
            //bitmap.SetPixelBytes(backBuffer);
            //canvas1.Invalidate();
            
            Debug.WriteLine($"after process={sw.ElapsedMilliseconds}");
        }
    }
}
