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
    public class LifeGameRender<TPixel>(Action<Memory<TPixel>>? renderCallback,float4 aliveColor,float4 deadColor) 
        : TextureRenderBase<LifeGameItem,TPixel> 
        where TPixel:unmanaged,IPixel<TPixel,float4>
    {
        public LifeGameRender(Action<Memory<TPixel>>? renderCallback):this(renderCallback, new float4(0,0,0,1),new float4(1,1,1,1))
        {
            
        }
        

        public override void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<LifeGameItem> data)
        {
            var shader = new LifeGameRenderShader(data, Texture, BackBufferSize.Width, aliveColor, deadColor);
            ctx.For(BackBufferSize.Width, BackBufferSize.Height, shader);

        }

        protected override void OnRender(Memory<TPixel> buffer)
        {
            renderCallback(buffer);
        }

        
    }
}
