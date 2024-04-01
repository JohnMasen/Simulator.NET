using ComputeSharp.Descriptors;
using ComputeSharp.Resources;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public class SwapBufferEngine<TData>
        where TData : unmanaged
    {
        GraphicsDevice device;
        (ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target) buffers;
        private Size size;
        public List<IPostProcessor<TData>> PostProcessors { get; } = new();
        private ITransformProcessor<TData> transformProcessor;
        public SwapBufferEngine(GraphicsDevice device, Size bufferSize, Memory<TData> data, ITransformProcessor<TData> processor)
        {
            this.device = device;
            buffers.source = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target.CopyFrom(data.Span);
            buffers.source.CopyFrom(buffers.target);
            size = bufferSize;
            transformProcessor=processor;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void swapBuffer()
        {
            buffers = (buffers.target, buffers.source);
        }

        public void Step()
        {
            using (var ctx = device.CreateComputeContext())
            {
                transformProcessor.Process(in ctx, buffers.source, buffers.target);
                foreach (var item in PostProcessors)
                {
                    item.Process(in ctx, buffers.target);
                }
            }
            swapBuffer();
        }
        public async Task StepAsync()
        {
            //var shader = createShader(buffers.source, buffers.target);
            await using (var ctx = device.CreateComputeContext())
            {
                transformProcessor.Process(in ctx, buffers.source, buffers.target);
                foreach (var item in PostProcessors)
                {
                    item.Process(in ctx, buffers.target);
                }
            }
            swapBuffer();
        }

        public void GetOutput(Memory<TData> outputBuffer)
        {
            buffers.target.CopyTo(outputBuffer.Span);
        }

        public ReadWriteBuffer<TData> GetOutputBuffer() => buffers.target;

        public void WithOutput(Action<Memory<TData>> outputProcessingCallback)
        {
            using var tmp = MemoryPool<TData>.Shared.Rent(buffers.target.Length);
            buffers.target.CopyTo(tmp.Memory.Span);
            outputProcessingCallback(tmp.Memory.Slice(buffers.target.Length));
        }
    }
}
