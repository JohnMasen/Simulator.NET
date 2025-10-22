using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.GrayScott.WinUI
{
    public class GrayScottRender<TPixel>(Action<Memory<TPixel>> renderCallback, TPixel background) : TextureRenderBase<GrayScottItem, TPixel> where TPixel : unmanaged, IPixel<TPixel, float4>
    {
        public override void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<GrayScottItem> data)
        {
            ctx.Fill(Texture,background);
            ctx.Barrier(Texture);
            ctx.For(BackBufferSize.Width, BackBufferSize.Height, new GrayScottRenderShader(data, Texture));
        }

        protected override void OnRender(Memory<TPixel> buffer)
        {
            renderCallback(buffer);
        }
    }
}
