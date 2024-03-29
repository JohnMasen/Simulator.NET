using ComputeSharp;
using ComputeSharp.Descriptors;
using ComputeSharp.Resources;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComputeSharpTest1.Core
{
    public class TransformEngine<TData, TShader>
        where TData : unmanaged
        where TShader : struct, IComputeShader, IComputeShaderDescriptor<TShader>
    {
        GraphicsDevice device;
        //TShader[] shaders;
        //TShader shader1;
        //TShader shader2;
        ReadWriteBuffer<TData> buffer1;
        ReadWriteBuffer<TData> buffer2;
        private int x, y;
        bool isSwap = false;
        public delegate TShader CreateShaderDelegate(ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target);
        private List<ITransformPostProcessor<TData>> PostProcessors { get; }
        private CreateShaderDelegate createShader;
        public TransformEngine(GraphicsDevice device, int xCount, int yCount, Memory<TData> data, CreateShaderDelegate createShaderCallback, IEnumerable<ITransformPostProcessor<TData>> postProcessors=null)
        {
            Debug.WriteLine($"Using {device}");
            //shaders = new TShader[2];
            this.device = device;
            buffer1 = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffer2 = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffer2.CopyFrom(data.Span);
            buffer1.CopyFrom(buffer2);
            //shaders[0] = createShaderCallback(buffer1, buffer2);
            //shaders[1] = createShaderCallback(buffer2, buffer1);
            Type rType = typeof(TShader);
            x = xCount;
            y = yCount;
            if (postProcessors != null)
            {
                PostProcessors = [.. postProcessors];
                PostProcessors.ForEach(item => item.Init());
            }
            
            createShader = createShaderCallback;

        }

        public void Step()
        {
            int index = isSwap ? 1 : 0;
            var source=isSwap?buffer2 : buffer1;
            var target = isSwap ? buffer1 : buffer2;
            var shader = createShader(source, target);
            using (var ctx=device.CreateComputeContext())
            {
            }
            device.For(x, y, shader);

            //source.CopyFrom(target);

            //if (PostProcessors == null)
            //{
                //device.For(x, y, shaders[index]);
            //}
            //else
            //{
            //    using (var ctx = device.CreateComputeContext())
            //    {
            //        buffer1.CopyFrom()
            //        ctx.For(x, y, shaders[index]);
            //        var postShaders=isSwap ? postShaders2 : postShaders1;
            //        foreach (var item in postShaders)
            //        {
            //            ctx.For(x, y, item);
            //        }
            //    }
            //}



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

        public ReadWriteBuffer<TData> GetOutputBuffer() => isSwap ? buffer1 : buffer2;

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
