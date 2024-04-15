using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Simulator.NET.Core
{
    public class SimulationController(Func<ISession> createSessionCallback) : ISimulationController
    {
        public virtual ISession Session { get; private set; }
        private bool isRunning = false;

        public bool IsRunning => isRunning;

        public event EventHandler<string> OnStop;
        public event EventHandler<ISession> OnStep;
        private volatile bool initRequired = true;
        private CancellationTokenSource? cts;
        public void Start()
        {
            tryInit();

            Task.Run(() =>
            {
                isRunning = true;
                var token = cts!.Token;
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        Session.Step();
                        OnStep?.Invoke(this, Session);
                    }
                    isRunning = false;
                    OnStop?.Invoke(this, "Run Complete");
                }
                catch (TaskCanceledException)
                {
                    isRunning = false;
                    OnStop?.Invoke(this, "Run Canceled");
                }
                catch(Exception)
                {
                    throw;
                }
            });
        }

        public void Step()
        {
            isRunning = true;
            tryInit();
            Session.Step();
            OnStep?.Invoke(this, Session);
            OnStop?.Invoke(this,"Step Complete");
            isRunning = false;
        }

        public void Stop()
        {
            doStop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void tryInit()
        {
            if (initRequired) 
            {
                Session = createSessionCallback();
                cts = new CancellationTokenSource();
                initRequired = false;
            }
        }

        private void doStop()
        {
            cts?.Cancel();
            cts = null;
            initRequired = true;
        }

        public void Reset()
        {
            doStop();
            tryInit();

        }
    }
}
