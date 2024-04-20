using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public abstract class TextureRenderBase<TData,TPixel>() : IPostProcessor<TData> where TData : unmanaged where TPixel:unmanaged, IPixel<TPixel,float4>
    {
        public virtual bool IsEnabled { get; set; } = true;

        protected ReadWriteTexture2D<TPixel, float4> Texture;
        protected ReadBackTexture2D<TPixel> ReadBack;
        protected Size BackBufferSize;
        private TPixel[] BackBuffer;

        public void AfterProcess(GraphicsDevice device)
        {
            Texture.CopyTo(ReadBack);
            ReadBack.View.CopyTo(BackBuffer);
            OnRender(BackBuffer);

        }
        protected abstract void OnRender(Memory<TPixel> buffer);

        public virtual void BeforeProcess(GraphicsDevice device)
        {
            
        }

        public virtual void Init(GraphicsDevice device, Size size)
        {
            if (!device.IsReadWriteTexture2DSupportedForType<TPixel>())
            {
                throw new InvalidOperationException($"[{typeof(TPixel).Name}] is not supported by device [{device.Name}]");
            }
            BackBufferSize = size;
            BackBuffer = new TPixel[size.Width * size.Height];
            Texture = device.AllocateReadWriteTexture2D<TPixel, float4>(size.Width, size.Height);
            ReadBack = device.AllocateReadBackTexture2D<TPixel>(size.Width, size.Height);
        }

        public abstract void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<TData> data);
    }
}
