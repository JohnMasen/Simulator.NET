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
using System.Runtime.CompilerServices;
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
        (ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target) buffers;
        private Size size;
        public delegate TShader CreateShaderDelegate(ReadWriteBuffer<TData> source, ReadWriteBuffer<TData> target);
        private CreateShaderDelegate createShader;
        public TransformEngine(GraphicsDevice device, Size bufferSize, Memory<TData> data, CreateShaderDelegate createShaderCallback)
        {
            Debug.WriteLine($"Using {device}");
            //shaders = new TShader[2];
            this.device = device;
            buffers.source = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target = device.AllocateReadWriteBuffer<TData>(data.Length);
            buffers.target.CopyFrom(data.Span);
            buffers.source.CopyFrom(buffers.target);
            size = bufferSize;
            createShader = createShaderCallback;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void swapBuffer()
        {
            buffers = (buffers.target, buffers.source);
        }

        public void Step()
        {
            var shader = createShader(buffers.source,buffers.target);
            device.For(size.Width, size.Height, shader);
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
