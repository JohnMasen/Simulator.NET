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
        //private Size size;
        private IEnumerable<IPostProcessor<TData>> PostProcessors;
        private ITransformProcessor<TData> transformProcessor;
        public SwapBufferEngine(GraphicsDevice device, Size bufferSize, Memory<TData> data, ITransformProcessor<TData> processor, IEnumerable<IPostProcessor<TData>> postProcessors)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            ArgumentNullException.ThrowIfNull(processor, nameof(processor));
            transformProcessor = processor;
            PostProcessors = postProcessors ?? new List<IPostProcessor<TData>>();

            this.device = device;
            buffers.source = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target.CopyFrom(data.Span);
            buffers.source.CopyFrom(buffers.target);

            transformProcessor.Init(device, bufferSize);
            foreach (var item in PostProcessors)
            {
                item.Init(device, bufferSize);
            }
            Step(true);
            //size = bufferSize;
            //transformProcessor=processor;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void swapBuffer()
        {
            buffers = (buffers.target, buffers.source);
        }

        public void Step(bool skipTransform = false)
        {
            transformProcessor.BeforeProcess(device);
            foreach (var item in PostProcessors)
            {
                item.BeforeProcess(device);
            }
            Stopwatch sw = Stopwatch.StartNew();

            using (var ctx = device.CreateComputeContext())
            {
                if (!skipTransform)
                {
                    transformProcessor.Process(in ctx, buffers.source, buffers.target);
                    ctx.Barrier(buffers.target);
                }
                else
                {
                    buffers.source.CopyTo(buffers.target);//bypass process, simply copy source to target
                }
                foreach (var item in PostProcessors)
                {
                    item.Process(in ctx, buffers.target);
                }
            }

            sw.Stop();
            transformProcessor.AfterProcess(device);
            foreach (var item in PostProcessors)
            {
                item.AfterProcess(device);
            }
            swapBuffer();
            Debug.WriteLine($"process={sw.ElapsedMilliseconds} ms");
        }
        public async Task StepAsync(bool skipTransform=false)
        {
            transformProcessor.BeforeProcess(device);
            foreach (var item in PostProcessors)
            {
                item.BeforeProcess(device);
            }
            Stopwatch sw = Stopwatch.StartNew();

            await using (var ctx = device.CreateComputeContext())
            {
                if (!skipTransform)
                {
                    transformProcessor.Process(in ctx, buffers.source, buffers.target);
                    ctx.Barrier(buffers.target);
                }
                else
                {
                    buffers.source.CopyTo(buffers.target);//bypass process, simply copy source to target
                }
                foreach (var item in PostProcessors)
                {
                    item.Process(in ctx, buffers.target);
                    ctx.Barrier(buffers.target);
                }
            }

            sw.Stop();
            transformProcessor.AfterProcess(device);
            foreach (var item in PostProcessors)
            {
                item.AfterProcess(device);
            }
            swapBuffer();
            Debug.WriteLine($"process async={sw.ElapsedMilliseconds} ms");
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
