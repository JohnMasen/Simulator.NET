using ComputeSharp;
using Simulator.NET.Core;
using System.Buffers;
using System.Drawing;
using System.Numerics;

namespace Simulator.NET.Worms
{
    public class WormProcessor : ITransformProcessor<WormItem>
    {
        
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
        }

        
    }
}
