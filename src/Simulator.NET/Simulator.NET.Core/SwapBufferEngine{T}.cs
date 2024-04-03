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
        private bool isInitNeeded = true;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void swapBuffer()
        {
            buffers = (buffers.target, buffers.source);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void Step()
        {
            tryInit();
            transformProcessor.BeforeProcess(device);
            foreach (var item in PostProcessors)
            {
                item.BeforeProcess(device);
            }
            Stopwatch sw = Stopwatch.StartNew();
            using (var ctx = device.CreateComputeContext())
            {
                
                transformProcessor.Process(in ctx, buffers.source, buffers.target);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public async Task StepAsync()
        {
            //var shader = createShader(buffers.source, buffers.target);
            tryInit();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void GetOutput(Memory<TData> outputBuffer)
        {
            buffers.target.CopyTo(outputBuffer.Span);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public ReadWriteBuffer<TData> GetOutputBuffer() => buffers.target;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void WithOutput(Action<Memory<TData>> outputProcessingCallback)
        {
            using var tmp = MemoryPool<TData>.Shared.Rent(buffers.target.Length);
            buffers.target.CopyTo(tmp.Memory.Span);
            outputProcessingCallback(tmp.Memory.Slice(buffers.target.Length));
        }

        private void tryInit()
        {
            if (isInitNeeded)
            {
                transformProcessor.Init(device,size);
                foreach (var item in PostProcessors)
                {
                    item.Init(device, size);
                }
            }
            
            isInitNeeded = false;
        }
    }
}
