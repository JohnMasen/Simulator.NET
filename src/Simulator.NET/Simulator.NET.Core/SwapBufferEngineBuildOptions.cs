using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public class SwapBufferEngineBuildOptions<T> where T:unmanaged
    {
        public List<IPostProcessor<T>> PostProcessors { get;  } = new();
        public GraphicsDevice Device { get; set; } = GraphicsDevice.GetDefault();
        public ITransformProcessor<T> Processor { get; set; }
        public Size BufferSize { get; set; }
        public Memory<T> Data { get; set; }

    }
}
