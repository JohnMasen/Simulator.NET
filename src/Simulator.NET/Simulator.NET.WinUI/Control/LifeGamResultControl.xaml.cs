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
        private readonly float4 alive = new Float4(0, 0, 0, 1);
        private readonly float4 dead = new Float4(1, 1, 1, 1);
        ReadWriteBuffer<float4> texture;
        public ReadBackBuffer<float4> output;
        #endregion

        private Size backBufferSize;
        AutoResetEvent aseResize = new(false);
        public LifeGameRender Render { get; set; }



        public LifeGamResultControl()
        {
            this.InitializeComponent();
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
            ctx.Barrier(data);
            ctx.For(backBufferSize.Width, backBufferSize.Height, shader);
            //ctx.Barrier(texture);
        }

        public void Init(GraphicsDevice device,Size size)
        {
            
            DispatcherQueue.TryEnqueue(() =>
            {
                canvas1.Width = size.Width;
                canvas1.Height = size.Height;
                aseResize.Set();
            });
            aseResize.WaitOne();
            
            backBufferSize = size;
            backBuffer = new byte[size.Width * size.Height * Marshal.SizeOf<float4>()];
            bitmap = CanvasBitmap.CreateFromBytes(canvas1, backBuffer, size.Width, size.Height, Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float); ;
            texture = device.AllocateReadWriteBuffer<float4>(size.Width * size.Height);
            output = device.AllocateReadBackBuffer<float4>(size.Width * size.Height);
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            //throw new NotImplementedException();
        }

        public void AfterProcess(GraphicsDevice device)
        {   
            var sw = Stopwatch.StartNew();
            //output.Span.CopyTo()
            texture.CopyTo(output);
            sw.Stop();
            MemoryMarshal.AsBytes(output.Span).CopyTo(backBuffer);
            bitmap.SetPixelBytes(backBuffer);
            canvas1.Invalidate();
            
            Debug.WriteLine($"after process={sw.ElapsedMilliseconds}");
        }
    }
}
