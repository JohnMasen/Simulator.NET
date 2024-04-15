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
    public class LifeGameProcessor() : ITransformProcessor<LifeGameItem>
    {
        private Size size;
        readonly int3x3 mask = new(1, 1, 1,
                         1, 0, 1,
                         1, 1, 1);
        public bool IsEnabled { get; set; } = true;
        public void AfterProcess(GraphicsDevice device)
        {
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            
        }

        public void Init(GraphicsDevice device,Size size)
        {
            this.size = size;
        }

        public void Process(ref readonly ComputeContext context, ReadWriteBuffer<LifeGameItem> input, ReadWriteBuffer<LifeGameItem> output)
        {
            LifeGameShader shader = new LifeGameShader(input, output, size.Width, size.Height, mask);
            context.For(size.Width, size.Height, shader);
        }
    }
}
