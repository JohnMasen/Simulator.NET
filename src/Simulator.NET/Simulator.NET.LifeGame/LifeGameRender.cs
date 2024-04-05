using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.LifeGame
{
    //public class LifeGameRender():IPostProcessor<LifeGameItem>
    //{
    //    //private Memory<float4> buffer = new(new float4[size.Width * size.Height]);
    //    private readonly int alive = 0b_1111_1111_1111_1111;
    //    private readonly int dead = 0b_0000_0000_0000_1111;
    //    ReadWriteBuffer<int> texture;
    //    public ReadBackBuffer<int> output;
    //    public event EventHandler<Memory<int>> FrameArrived;
    //    private Size size;
    //    //public Memory<float4> Output => output.Memory;

    //    public void AfterProcess(GraphicsDevice device)
    //    {
    //        texture.CopyTo(output);
    //        FrameArrived?.Invoke(this, output.Memory);
    //    }

    //    public void BeforeProcess(GraphicsDevice device)
    //    {
            
    //    }

    //    public void Init(GraphicsDevice device,Size size)
    //    {
    //        texture = device.AllocateReadWriteBuffer<int>(size.Width * size.Height);
    //        output =  device.AllocateReadBackBuffer<int>(size.Width * size.Height);
    //    }

    //    public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<LifeGameItem> data)
    //    {
    //        var shader = new LifeGameRenderShader(data, texture, size.Width, alive, dead);
    //        ctx.For(size.Width, size.Height, shader);
    //        ctx.Barrier(texture);
            
    //    }

        
    //}
}
