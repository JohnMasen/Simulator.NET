using ComputeSharp;
using ComputeSharp;
using ComputeSharp.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Simulator.NET.GrayScott
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct GrayScottShader(ReadWriteBuffer<GrayScottItem> buffer1, ReadWriteBuffer<GrayScottItem> buffer2,float f, float k) : IComputeShader
    {
        
        public void Execute()
        {
            int idx =ThreadIds.Y * DispatchSize.X + ThreadIds.X;
            GrayScottItem result = buffer1[idx];
            float diffuseU = ConvU3x3(ThreadIds.XY,DispatchSize.XY);
            float diffuseV = ConvV3x3(ThreadIds.XY, DispatchSize.XY);
            result.U += diffuseU - result.U * result.V * result.V + (1 - result.U) * f;
            result.V += diffuseV + result.U * result.V * result.V - (f + k) * result.V;
            buffer2[idx] = result;
        }


        public float ConvU3x3(int2 pos,  int2 size)
        {
            return
                SampleOverflow(pos,new int2(-1,-1),size).U + SampleOverflow(pos, new int2(-1, 0), size).U + SampleOverflow(pos, new int2(-1, 1), size).U +
                SampleOverflow(pos, new int2(0, -1), size).U + SampleOverflow(pos, new int2(0, 1), size).U +
                SampleOverflow(pos, new int2(1, -1), size).U + SampleOverflow(pos, new int2(1, 0), size).U + SampleOverflow(pos, new int2(1,1), size).U
                - SampleOverflow(pos,int2.Zero,size).U * 8;

        }
        public float ConvV3x3(int2 pos, int2 size)
        {
            return
                SampleOverflow(pos, new int2(-1, -1), size).V + SampleOverflow(pos, new int2(-1, 0), size).V + SampleOverflow(pos, new int2(-1, 1), size).V +
                SampleOverflow(pos, new int2(0, -1), size).V + SampleOverflow(pos, new int2(0, 1), size).V +
                SampleOverflow(pos, new int2(1, -1), size).V + SampleOverflow(pos, new int2(1, 0), size).V + SampleOverflow(pos, new int2(1, 1), size).V
                - SampleOverflow(pos, int2.Zero, size).V * 8;

        }

        public GrayScottItem SampleOverflow(int2 pos,int2 offset,int2 size)
        {
            int idx = pos.X + offset.X;
            if (idx<0)
            {
                idx += size.X;
            }
            else if (idx> size.X)
            {
                idx -= size.X;
            }
            int idy = pos.Y + offset.Y;
            if (idy<0)
            {
                idy += size.Y;
            }
            else if(idy> size.Y)
            {
                idy -= size.Y;
            }
            return buffer1[idx * size.X + idy];
        }

        public GrayScottItem SampleClamp(int2 pos, int2 offset, int2 size)
        {
            return buffer1[Hlsl.Clamp(pos.X+offset.X, 0, size.X-1) * DispatchSize.X + Hlsl.Clamp(pos.Y+offset.Y, 0, DispatchSize.Y - 1)];
        }
    }
}
