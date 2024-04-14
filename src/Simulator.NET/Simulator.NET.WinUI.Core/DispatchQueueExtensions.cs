using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.Core
{
    public static class DispatchQueueExtensions
    {
        public static void RunAndWait(this DispatcherQueue queue,Action action)
        {
            
            //queue.HasThreadAccess
            if (queue.HasThreadAccess)
            {
                action();
            }
            else
            {
                ManualResetEventSlim mre = new ManualResetEventSlim(false);
                queue.TryEnqueue(() =>
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        mre.Set();
                    }
                });
                mre.Wait();
            }
            
        }
    }
}
