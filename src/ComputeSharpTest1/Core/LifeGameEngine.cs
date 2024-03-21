using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using ComputeSharp.D2D1;
using ComputeSharp.Resources;
using Windows.System;

namespace ComputeSharpTest1.Core;
public class LifeGameEngine
{
    //private Memory<int> buffer1;
    //private Memory<int> buffer2;
    private readonly ReadWriteBuffer<LiveGameDebugItem> rwbuffer1;
    private readonly ReadWriteBuffer<LiveGameDebugItem> rwbuffer2;
    private readonly LifeGameTest[] shaders;
    private readonly int xCount, yCount;
    bool isSwap = false;

    public LifeGameEngine(Memory<int> data,int xcount,int ycount)
    {
        //buffer1 = new Memory<int>(new int[data.Length]);
        //buffer2 = new Memory<int>(new int[data.Length]);
        //data.CopyTo(buffer1);
        var d = GraphicsDevice.GetDefault();
        xCount = xcount;
        yCount = ycount;
        rwbuffer1 = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<LiveGameDebugItem>(data.Length);
        rwbuffer2 = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<LiveGameDebugItem>(data.Length);
        using var tmpMo = MemoryPool<LiveGameDebugItem>.Shared.Rent(data.Length);
        var tmpM =tmpMo.Memory.Slice(0,data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            tmpM.Span[i] = new LiveGameDebugItem() { Value = data.Span[i] };
        }
        rwbuffer1.CopyFrom(tmpM.Span);
        shaders = new LifeGameTest[2];
        shaders[0] = new LifeGameTest(rwbuffer1, rwbuffer2, xcount, ycount);
        shaders[1] = new LifeGameTest(rwbuffer2, rwbuffer1, xcount, ycount);
    }

    public void Step()
    {
        int index=isSwap ? 1 : 0;
        GraphicsDevice.GetDefault().For(xCount, yCount, shaders[index]);
        isSwap = !isSwap;
    }

    public void GetOutput(Memory<LiveGameDebugItem> data)
    {
        if (!isSwap)
        {
            rwbuffer1.CopyTo(data.Span);
        }
        else
        {
            rwbuffer2.CopyTo(data.Span);
        }
    }

    public ReadWriteBuffer<LiveGameDebugItem> GetOutput()
    {
        return isSwap ? rwbuffer2 : rwbuffer1;
    }





}
