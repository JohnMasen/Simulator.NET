using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public sealed class DebuggerProcessor<T>(int bufferSize,Action<ReadOnlyMemory<T>> DebugHandler) : IPostProcessor<T> where T:unmanaged
    {
        public bool IsEnabled { get; set; } = true;
        ReadBackBuffer<T> readBack;
        public void AfterProcess(GraphicsDevice device)
        {
            if (Debugger.IsAttached)
            {
                DebugHandler(readBack.Memory);
            }
        }

        public void BeforeProcess(GraphicsDevice device)
        {
            
        }

        public void Init(GraphicsDevice device, Size size)
        {
            readBack = device.AllocateReadBackBuffer<T>(bufferSize);
        }

        public void Process(ref readonly ComputeContext ctx, ReadWriteBuffer<T> data)
        {
            data.CopyTo(readBack);
        }
    }
}
