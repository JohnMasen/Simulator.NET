using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.GrayScott.WinUI
{
    public class GrayScottRender<TPixel>(Action<Memory<TPixel>> renderCallback, Bgra32[] colorMapData,Size colorMapSize) : TextureRenderBase<GrayScottItem, TPixel> where TPixel : unmanaged, IPixel<TPixel, float4>
    {
        private ReadOnlyTexture2D<Bgra32, float4> map;
        public override void Init(GraphicsDevice device, Size size)
        {
            base.Init(device, size);
            map = device.AllocateReadOnlyTexture2D<Bgra32, float4>(colorMapData, colorMapSize.Width, colorMapSize.Height);
            //using var ctx= device.CreateComputeContext();

        }
        public override void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<GrayScottItem> data)
        {
            ctx.Fill(Texture, new float4(1,1,1,1));
            ctx.Barrier(Texture);
            ctx.For(BackBufferSize.Width, BackBufferSize.Height, new GrayScottRenderShader(data, Texture,map));
        }

        protected override void OnRender(Memory<TPixel> buffer)
        {
            renderCallback(buffer);
        }
    }
}
