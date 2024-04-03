using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public class SwapBufferEngineBuilder
    {
        public static SwapBufferEngine<T> Create<T>(Action<SwapBufferEngineBuildOptions<T>> createCallback)where T:unmanaged
        {
            SwapBufferEngineBuildOptions<T> opt = new();
            createCallback(opt);
            //check options

            return new SwapBufferEngine<T>(opt.Device, opt.BufferSize, opt.Data, opt.Processor, opt.PostProcessors);
        }
        
    }
}
