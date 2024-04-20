using ComputeSharp;
using Simulator.NET.Core;
using System.Buffers;
using System.Drawing;
using System.Numerics;

namespace Simulator.NET.Worms
{
    public class WormProcessor(int itemCount,int2 gridSize,float navigation,float stability) : ITransformProcessor<WormItem>
    {
        public bool IsEnabled { get; set; } = true;
        public void AfterProcess(GraphicsDevice device)
        {
            //throw new NotImplementedException();
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            //throw new NotImplementedException();
        }

        public void Init(GraphicsDevice device, Size size)
        {
            //throw new NotImplementedException();
        }

        public void Process(ref readonly ComputeContext context, ReadWriteBuffer<WormItem> input, ReadWriteBuffer<WormItem> output)
        {
            //throw new NotImplementedException();
            float seed = Random.Shared.NextSingle();
            context.For(itemCount, new WormProcessorShader(input, output, gridSize, seed, navigation,stability));

        }

        
    }
}
