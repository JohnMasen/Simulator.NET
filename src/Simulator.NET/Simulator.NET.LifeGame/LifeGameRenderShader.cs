using ComputeSharp;
using ComputeSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.LifeGame
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct LifeGameRenderShader(ReadWriteBuffer<LifeGameItem> buffer, IReadWriteNormalizedTexture2D<float4> o, int width, float4 aliveCellColor, float4 deadCellColor) : IComputeShader
    {
        public void Execute()
        {
            int idx = ThreadIds.Y * width + ThreadIds.X;
            o[ThreadIds.XY] = buffer[idx].Value == 1 ? aliveCellColor : deadCellColor;
        }
    }
}
