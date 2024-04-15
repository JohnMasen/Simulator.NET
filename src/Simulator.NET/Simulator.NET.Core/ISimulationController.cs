using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public interface ISimulationController
    {
        public event EventHandler<string> OnStop;
        public event EventHandler<bool> OnRunningStatusChanged;
        public event EventHandler<ISession> OnStep;
        public ISession Session { get; }
        public bool IsRunning { get; }
        public void Step();
        public void Stop();
        public void Start();
        public void Reset();

    }
}
