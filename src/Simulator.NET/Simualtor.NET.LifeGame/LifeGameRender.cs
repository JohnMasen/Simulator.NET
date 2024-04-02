using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core
{
    public class LifeGameRender(GraphicsDevice device, Size size)
    {
        //private Memory<float4> buffer = new(new float4[size.Width * size.Height]);
        private readonly float4 alive = new Float4(0, 0, 0, 1);
        private readonly float4 dead = new Float4(1, 1, 1, 1);
        ReadWriteBuffer<float4> texture = device.AllocateReadWriteBuffer<float4>(size.Width* size.Height);
        ReadBackBuffer<float4> output = device.AllocateReadBackBuffer<float4>(size.Width*size.Height);
        public Memory<float4> Render(ReadWriteBuffer<LifeGameItem> data)
        {
            var shader = new LifeGameRenderShader(data,texture, size.Width, alive, dead);
            device.For(size.Width,size.Height,shader);
            texture.CopyTo(output);
            return output.Memory;
            //texture.CopyTo(buffer.Span);
        }
    }
}
