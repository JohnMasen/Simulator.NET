using ComputeSharp;
using ComputeSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.GrayScott.WinUI
{
    [GeneratedComputeShaderDescriptor]
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    public readonly partial struct GrayScottRenderShader(ReadWriteBuffer<GrayScottItem> data, IReadWriteNormalizedTexture2D<float4> texture, IReadOnlyNormalizedTexture2D<float4> colorMap) : IComputeShader
    {
        public void Execute()
        {
            //float4 result = float4.Zero;
            int idx = ThreadIds.Y * DispatchSize.X + ThreadIds.X;
            //result.A = 1f;

            int2 mapPos = 0;
            mapPos.X = (int)Hlsl.Lerp(0f, colorMap.Width, data[idx].U);
            mapPos.Y = (int)Hlsl.Lerp(0f, colorMap.Height, data[idx].V);
            //result.B = data[idx].U;
            //result.G = data[idx].V;
            texture[ThreadIds.XY] = colorMap[mapPos];
        }
    }
}
