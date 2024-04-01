using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public interface IPostProcessor<T> :IProcessor where T:unmanaged
    {
        void Process(ref readonly ComputeContext ctx,ReadWriteBuffer<T> data);
    }
}
