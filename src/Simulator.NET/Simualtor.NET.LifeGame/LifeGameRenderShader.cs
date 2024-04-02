using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simualtor.NET.LifeGame
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct LifeGameRenderShader(ReadWriteBuffer<LifeGameItem> buffer,ReadWriteBuffer<float4> output, int width, float4 aliveCellColor,float4 deadCellColor) : IComputeShader
    {
        public void Execute()
        {
            int idx = ThreadIds.Y * width + ThreadIds.X;
            float4 result = new Float4(1, 0, 0, 0);
            var item = buffer[idx];
            //if (item.ContinuLifeCount <= 5)
            //{
            //    result[3] = Hlsl.Min(buffer[idx].LifeCount / 255f, 1f);
            //}

            result = buffer[idx].Value == 1 ? aliveCellColor : deadCellColor;
            output[idx] = result;
        }
    }
}
