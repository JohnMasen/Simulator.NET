using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.Worms.WinUI
{
    public class WormRender<TPixel>(Action<Memory<TPixel>> renderCallback,TPixel background, float4 bodyColor,float4 headColor ,int frameSkip=0) : TextureRenderBase<WormItem, TPixel> where TPixel : unmanaged, IPixel<TPixel, float4>
    {
        private int frameToSkip = 0;
        private bool renderResult = false;
        public override void BeforeProcess(GraphicsDevice device)
        {
            base.BeforeProcess(device);
        }
        public override void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<WormItem> data)
        {
            if (frameToSkip==0)
            {
                ctx.Fill(Texture, background);
                ctx.Barrier(Texture);
                ctx.For(data.Length, new WormRenderShader(data, Texture, bodyColor,headColor));
                frameToSkip = frameSkip;
                renderResult = true;
            }
            else
            {
                frameToSkip--;
                renderResult = false;
            }
            
        }

        protected override void OnRender(Memory<TPixel> buffer)
        {
            if (renderResult==false)
            {
                return;
            }
            renderCallback(buffer);
        }
    }
}
