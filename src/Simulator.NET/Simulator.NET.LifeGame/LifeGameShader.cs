using ComputeSharp;

namespace Simulator.NET.LifeGame
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct LifeGameShader(ReadWriteBuffer<LifeGameItem> buffer1, ReadWriteBuffer<LifeGameItem> buffer2, int xCount, int yCount, int3x3 mask)
    : IComputeShader
    {


        public void Execute()
        {
            LifeGameItem result = new LifeGameItem();
            int count = 0;
            int xStart = ThreadIds.X == 0 ? 0 : -1;
            int xEnd = ThreadIds.X == xCount - 1 ? 0 : 1;
            int yStart = ThreadIds.Y == 0 ? 0 : -1;
            int yEnd = ThreadIds.Y == yCount - 1 ? 0 : 1;

            //int loopCount = 0;
            int idx = ThreadIds.Y * xCount + ThreadIds.X;
            int3x3 nValues = 0;
            //int3x3 mask = new Int3x3(1, 1, 1,
            //                            1, 0, 1,
            //                            1, 1, 1);
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    int dataIndex = (ThreadIds.Y + y) * xCount + ThreadIds.X + x;
                    nValues[y + 1][x + 1] = buffer1[dataIndex].Value;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                count += Hlsl.Dot(nValues[i], mask[i]);
            }
            var current = buffer1[idx];
            result.LifeCount = current.LifeCount;
            result.ContinuLifeCount = buffer2[idx].ContinuLifeCount;
            if (count == 3)
            {
                result.Reason = current.Value == 1 ? 0u : 1u;
                result.Value = 1;
                result.ContinuLifeCount += current.Value;
            }
            if (count == 2)
            {
                result.Value = current.Value;
                result.ContinuLifeCount += current.Value;
            }

            if (result.Value == 1)
            {
                result.LifeCount++;

            }
            buffer2[idx] = result;
        }
    }
}
