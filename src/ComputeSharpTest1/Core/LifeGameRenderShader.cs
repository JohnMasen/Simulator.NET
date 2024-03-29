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
    public readonly partial struct LifeGameRenderShader(ReadWriteBuffer<LifeGameDItem> items,ReadWriteTexture2D<float4> outputTexture,int xCount) : IComputeShader
    {
        private readonly float4 white = new float4(1, 1, 1, 1);
        private readonly float4 black = new float4(0, 0, 0, 1);
        public void Execute()
        {
            outputTexture[ThreadIds.XY] = items[ThreadIds.X + ThreadIds.Y * xCount].Value == 1 ? black : white;
        }
    }
}
