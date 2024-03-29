using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct LifeGameRenderShader(ReadWriteBuffer<LifeGameItem> buffer,ReadWriteBuffer<float4> output, int width, float4 aliveCellColor,float4 deadCellColor) : IComputeShader
    {
        public void Execute()
        {
            int idx = ThreadIds.Y * width + ThreadIds.X;
            output[idx]=buffer[idx].Value == 1 ? aliveCellColor : deadCellColor;
        }
    }
}
