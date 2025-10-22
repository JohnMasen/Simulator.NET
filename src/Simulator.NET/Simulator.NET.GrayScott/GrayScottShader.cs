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
    public readonly partial struct GrayScottShader(ReadWriteBuffer<GrayScottItem> buffer1, ReadWriteBuffer<GrayScottItem> buffer2, float f, float k) : IComputeShader
    {

        public void Execute()
        {
            int idx = ThreadIds.Y * DispatchSize.X + ThreadIds.X;
            GrayScottItem result = buffer1[idx];
            //float diffuseU = ConvU3x3(ThreadIds.XY, DispatchSize.XY);
            //float diffuseV = ConvV3x3(ThreadIds.XY, DispatchSize.XY);
            float diffuseU = LaplaceU(ThreadIds.XY, DispatchSize.XY);
            float diffuseV = LaplaceV(ThreadIds.XY, DispatchSize.XY);
            ////debug
            //diffuseU = 0;
            //diffuseV = 0;
            ////end of debug
            float uv2 = result.U * result.V * result.V;
            result.U += diffuseU - uv2 + (1 - result.U) * f;
            result.V += diffuseV + uv2 - (f + k) * result.V;
            //debug
            //result.U += diffuseU;
            //result.V += diffuseV;
            //end of debug
            result.U = Hlsl.Clamp(result.U, -1, 1);
            result.V = Hlsl.Clamp(result.V, -1, 1);
            buffer2[idx] = result;
        }

        public float LaplaceU(int2 pos,int2 size)
        {
            return
                (DoSample(pos, new int2(-1, -1), size).U + 4f*DoSample(pos, new int2(-1, 0), size).U + DoSample(pos, new int2(-1, 1), size).U +
                4f*DoSample(pos, new int2(0, -1), size).U + 4f*DoSample(pos, new int2(0, 1), size).U +
                DoSample(pos, new int2(1, -1), size).U + 4f * DoSample(pos, new int2(1, 0), size).U + DoSample(pos, new int2(1, 1), size).U
                - 20f * DoSample(pos, int2.Zero, size).U)/6;
        }

        public float LaplaceV(int2 pos, int2 size)
        {
            return
                (DoSample(pos, new int2(-1, -1), size).V + 4f * DoSample(pos, new int2(-1, 0), size).V + DoSample(pos, new int2(-1, 1), size).V +
                4f * DoSample(pos, new int2(0, -1), size).V + 4f * DoSample(pos, new int2(0, 1), size).V +
                DoSample(pos, new int2(1, -1), size).V + 4f * DoSample(pos, new int2(1, 0), size).V + DoSample(pos, new int2(1, 1), size).V
                - 20f * DoSample(pos, int2.Zero, size).V) / 6;
        }

        public float ConvU3x3(int2 pos, int2 size)
        {
            return
                (DoSample(pos, new int2(-1, -1), size).U + DoSample(pos, new int2(-1, 0), size).U + DoSample(pos, new int2(-1, 1), size).U +
                DoSample(pos, new int2(0, -1), size).U + DoSample(pos, new int2(0, 1), size).U +
                DoSample(pos, new int2(1, -1), size).U + DoSample(pos, new int2(1, 0), size).U + DoSample(pos, new int2(1, 1), size).U
                - 8f * DoSample(pos, int2.Zero, size).U)/9f;

        }
        public float ConvV3x3(int2 pos, int2 size)
        {
            return
                (DoSample(pos, new int2(-1, -1), size).V + DoSample(pos, new int2(-1, 0), size).V + DoSample(pos, new int2(-1, 1), size).V +
                DoSample(pos, new int2(0, -1), size).V + DoSample(pos, new int2(0, 1), size).V +
                DoSample(pos, new int2(1, -1), size).V + DoSample(pos, new int2(1, 0), size).V + DoSample(pos, new int2(1, 1), size).V
                - 8f * DoSample(pos, int2.Zero, size).V)/9f;

        }
        public GrayScottItem DoSample(int2 pos, int2 offset, int2 size)
        {
            return SampleOverflow(pos, offset, size);
        }

        public GrayScottItem SampleOverflow(int2 pos, int2 offset, int2 size)
        {
            int idx = pos.X + offset.X;
            if (idx < 0)
            {
                idx += size.X;
            }
            else if (idx > size.X-1)
            {
                idx -= size.X;
            }
            int idy = pos.Y + offset.Y;
            if (idy < 0)
            {
                idy += size.Y;
            }
            else if (idy > size.Y-1)
            {
                idy -= size.Y;
            }
            return buffer1[idy * size.X + idx];
        }

        public GrayScottItem SampleClamp(int2 pos, int2 offset, int2 size)
        {
            return buffer1[Hlsl.Clamp(pos.Y + offset.Y, 0, size.Y - 1) * size.X + Hlsl.Clamp(pos.X + offset.X, 0, size.X - 1)];
        }
    }
}
