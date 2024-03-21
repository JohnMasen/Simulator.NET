using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using Windows.ApplicationModel.Contacts;

namespace ComputeSharpTest1.Core;
[ThreadGroupSize(DefaultThreadGroupSizes.XY)]
[GeneratedComputeShaderDescriptor]
public readonly partial struct LifeGameTest(ReadWriteBuffer<LiveGameDebugItem> buffer1, ReadWriteBuffer<LiveGameDebugItem> buffer2, int xCount, int yCount) : IComputeShader
{
    public void Execute()
    {

        int count = 0;
        int xStart = ThreadIds.X == 0 ? 0 : -1;
        int xEnd = ThreadIds.X == xCount - 1 ? 0 : 1;
        int yStart = ThreadIds.Y == 0 ? 0 : -1;
        int yEnd = ThreadIds.Y == yCount - 1 ? 0 : 1;

        //if (xStart<0)
        //{
        //    xStart = 0;
        //}
        //if (yStart<0)
        //{
        //    yStart = 0;
        //}
        int loopCount = 0;
        int idx = ThreadIds.Y * xCount + ThreadIds.X;
        //int3x3 nValues=0;
        for (int y = yStart; y <= yEnd; y++)
        {
            for (int x = xStart; x <= xEnd; x++)
            {
                int dataIndex = (ThreadIds.Y + y) * xCount + ThreadIds.X + x;
                int v=buffer1[dataIndex].Value;
                //nValues[y+1][x+1] = v;
                if (v ==1)
                {
                    count++;
                }
                loopCount++;
            }
        }
        count -= buffer1[idx].Value;

        int newValue = 0;
        if (count==3)
        {
            newValue = 1;
        }
        if (count==2)
        {
            newValue = buffer1[idx].Value;
        }
        //buffer2[idx].calcItem = nValues;
        buffer2[idx].Value = newValue;
        //buffer2[idx].LastValue = buffer1[idx].Value;
        //buffer2[idx].XStart = xStart;
        //buffer2[idx].YStart = yStart;
        //buffer2[idx].XEnd = xEnd;
        //buffer2[idx].YEnd = yEnd;
        //buffer2[idx].Count = count;
        //buffer2[idx].LoopCount = loopCount;
    }
}
