using ComputeSharp;
using ComputeSharp.Descriptors;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core
{
    public class TransformShaderRunner<TData,TShader> (TransformEngine<TData, TShader> engine) : IShaderRunner where TData:unmanaged where TShader : struct, IComputeShader, IComputeShaderDescriptor<TShader>
    {
        public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object parameter)
        {
            return true;
        }
    }
}
