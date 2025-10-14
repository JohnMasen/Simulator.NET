using ComputeSharp;
using Simulator.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.GrayScott
{
    public class GrayScottProcessor(float f,float k) : ITransformProcessor<GrayScottItem>
    {
        private Size s;
        public bool IsEnabled { get; set; }

        public void AfterProcess(GraphicsDevice device)
        {
            
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            
        }

        public void Init(GraphicsDevice device, Size size)
        {
            s = size;
        }

        public void Process(ref readonly ComputeContext context, ReadWriteBuffer<GrayScottItem> input, ReadWriteBuffer<GrayScottItem> output)
        {
            GrayScottShader shader = new GrayScottShader(input, output, f, k);
            context.For(s.Width, s.Height, shader);
        }
    }
}
