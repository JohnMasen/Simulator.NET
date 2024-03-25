using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core
{
    public interface IKernelFactory<TKernel,TData> where TData:struct where TKernel:IComputeShader
    {
        //TKernel GetKernel()
    }
}
