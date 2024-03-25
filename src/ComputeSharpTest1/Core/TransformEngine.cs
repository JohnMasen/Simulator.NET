using ComputeSharp;
using ComputeSharp.Descriptors;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComputeSharpTest1.Core
{
    public class TransformEngine<TData,TShader> where TData:unmanaged where TShader :struct, IComputeShader, IComputeShaderDescriptor<TShader>
    {
        GraphicsDevice device;
        TShader[] shaders;
        //TShader shader1;
        //TShader shader2;
        ReadWriteBuffer<TData> buffer1;
        ReadWriteBuffer<TData> buffer2;
        private int x,y;
        bool isSwap = false;
        public delegate TShader CreateShaderDelegate(ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target);
        public TransformEngine(GraphicsDevice device, int xCount,int yCount, Memory<TData> data,CreateShaderDelegate createShaderCallback)
        {
            shaders = new TShader[2];
            this.device = device;
            buffer1=device.AllocateReadWriteBuffer<TData>(data.Length);
            buffer2=device.AllocateReadWriteBuffer<TData>(data.Length);
            buffer2.CopyFrom(data.Span);
            buffer1.CopyFrom(buffer2);
            shaders[0] = createShaderCallback(buffer1, buffer2);
            shaders[1] = createShaderCallback(buffer2, buffer1);
            x = xCount;
            y = yCount;
        }

        public void Step()
        {
            int index=isSwap? 1 : 0;
            //source.CopyFrom(target);
            device.For(x, y, shaders[index]);
            isSwap = !isSwap;
        }

        public void GetOutput(Memory<TData> outputBuffer)
        {
            if (isSwap)
            {
                buffer1.CopyTo(outputBuffer.Span);
            }
            else
            {
                buffer2.CopyTo(outputBuffer.Span);
            }
        }

        public void WithOutput(Action<Memory<TData>> outputProcessingCallback)
        {
            using var tmp = MemoryPool<TData>.Shared.Rent(buffer1.Length);
            if (isSwap)
            {
                outputProcessingCallback(tmp.Memory.Slice(buffer1.Length));
            }
            else
            {
                outputProcessingCallback(tmp.Memory.Slice(buffer2.Length));
            }
            
        }
    }
}
