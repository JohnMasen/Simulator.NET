using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Simulator.NET.LifeGame
{
    public class LifeGameRender<T>(Action<Memory<T>>? renderCallback,float4 aliveColor,float4 deadColor) : IPostProcessor<LifeGameItem> where T:unmanaged,IPixel<T,float4>
    {
        public bool IsEnabled { get; set; } = true;
        public LifeGameRender(Action<Memory<T>>? renderCallback):this(renderCallback, new float4(0,0,0,1),new float4(1,1,1,1))
        {
            
        }
        private ReadWriteTexture2D<T, float4> texture;
        private ReadBackTexture2D<T> readback;
        private Size backBufferSize;
        private T[] backBuffer;
        public void AfterProcess(GraphicsDevice device)
        {
            texture.CopyTo(readback);
            readback.View.CopyTo(backBuffer);
            renderCallback?.Invoke(backBuffer.AsMemory());
        }

        public void BeforeProcess(GraphicsDevice device)
        {

        }

        public void Init(GraphicsDevice device, Size size)
        {
            if (!device.IsReadWriteTexture2DSupportedForType<T>())
            {
                throw new InvalidOperationException($"[{typeof(T).Name}] is not supported by device [{device.Name}]");
            }
            backBufferSize = size;
            backBuffer = new T[size.Width * size.Height];
            texture = device.AllocateReadWriteTexture2D<T, float4>(size.Width, size.Height);
            readback = device.AllocateReadBackTexture2D<T>(size.Width, size.Height);
        }

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<LifeGameItem> data)
        {
            var shader = new LifeGameRenderShader(data, texture, backBufferSize.Width, aliveColor, deadColor);
            ctx.For(backBufferSize.Width, backBufferSize.Height, shader);

        }


    }
}
