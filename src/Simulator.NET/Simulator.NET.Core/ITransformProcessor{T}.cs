using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public interface ITransformProcessor<T>:IProcessor where T:unmanaged
    {
        void Process(ref readonly ComputeContext context, ReadWriteBuffer<T> input, ReadWriteBuffer<T> output);
    }
}
