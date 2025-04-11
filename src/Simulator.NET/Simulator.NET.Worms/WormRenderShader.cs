using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Worms
{
    [GeneratedComputeShaderDescriptor]
    [ThreadGroupSize(DefaultThreadGroupSizes.X)]
    public readonly partial struct WormRenderShader(ReadWriteBuffer<WormItem> buffer,IReadWriteNormalizedTexture2D<float4> texture,float4 bodyColor,float4 headColor) : IComputeShader
    {
        public void Execute()
        {
            WormItem item = buffer[ThreadIds.X];
            texture[item.HeadPosition] = headColor;
            texture[item.BodyCells[0]] = bodyColor;
            texture[item.BodyCells[1]] = bodyColor;
            texture[item.BodyCells[2]] = bodyColor;

        }

        //private bool isSame(int2 v1,int2 v2)
        //{
        //    return v1.X == v2.X && v1.Y == v2.Y;
        //}
    }
}
