using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core
{
    public interface ITransformPostProcessor<TData> where TData:unmanaged
    {
        void Init();
        void Process(ReadWriteBuffer<TData> data);
    }
}
