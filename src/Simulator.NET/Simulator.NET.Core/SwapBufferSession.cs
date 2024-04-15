using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public class SwapBufferSession<TData> : ISession where TData:unmanaged
    {
        (ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target) buffers;
        protected GraphicsDevice Device;
        protected Size BufferSize;
        protected ITransformProcessor<TData> Processor;
        protected List<IPostProcessor<TData>> PostProcessors = new List<IPostProcessor<TData>>();
        private ReadBackBuffer<TData>? readBack;
        private object syncLock = new object();
        public SwapBufferSession(GraphicsDevice device, Size bufferSize,ReadOnlySpan<TData> inputData,ITransformProcessor<TData> processor,IEnumerable<IPostProcessor<TData>> postProcessors) 
        {
            Device = device;
            BufferSize = bufferSize;
            Processor = processor;
            Processor.Init(Device,BufferSize);
            buffers.source=Device.AllocateReadWriteBuffer(inputData);
            buffers.target = Device.AllocateReadWriteBuffer(buffers.source); //copy the initial data to output as default output
            if (PostProcessors!=null)
            {
                foreach (var p in postProcessors)
                {
                    AttachPostProcessor(p);
                }
            }
            stepInternal(true);
        }
        public void Step()
        {
            stepInternal();
        }

        private void stepInternal(bool skipTransform=false)
        {
            foreach (IProcessor item in PostProcessors)
            {
                var x = item.IsEnabled;
            }
            var enabledPostProcessors = from p in PostProcessors
                                        where p.IsEnabled == true
                                        select p;
            lock (syncLock)
            {
                
                BeforeStep();
                if (!skipTransform)
                {
                    Processor.BeforeProcess(Device);
                }

                foreach (var p in enabledPostProcessors)
                {
                    p.BeforeProcess(Device);
                }
                using (var ctx = Device.CreateComputeContext())
                {
                    if (!skipTransform)
                    {
                        OnStep(in ctx);
                    }
                    foreach (var p in enabledPostProcessors)
                    {
                        p.Process(in ctx, buffers.target);
                    }
                }

                foreach (var p in enabledPostProcessors)
                {
                    p.AfterProcess(Device);
                }
                if (!skipTransform)
                {
                    Processor.AfterProcess(Device);
                }
                AfterStep();
                swapBuffer();
            }
        }

        public void AttachPostProcessor(IPostProcessor<TData> postProcessor)
        {
            PostProcessors.Add(postProcessor);
            postProcessor.Init(Device, BufferSize);
        }

        protected virtual void OnStep(ref readonly ComputeContext ctx)
        {
            Processor.Process(ctx, buffers.source, buffers.target);
        }
        protected virtual void BeforeStep()
        {

        }
        protected virtual void AfterStep()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void swapBuffer()
        {
            buffers = (buffers.target, buffers.source);
        }
        

        public void WithOutput(Action<Memory<TData>> action)
        {
            lock (syncLock)
            {
                if (readBack is null)
                {
                    readBack=Device.AllocateReadBackBuffer<TData>(BufferSize.Width*BufferSize.Height,AllocationMode.Default);
                }
                buffers.target.CopyTo(readBack);
                //using var memoryHandle = MemoryPool<TData>.Shared.Rent(BufferSize.Width * BufferSize.Height);
                //buffers.target.CopyTo(memoryHandle.Memory.Span);
                action(readBack.Memory);
            }
        }
    }
}
