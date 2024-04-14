﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Core
{
    public interface ISession<T>:ISession
    {
        public T Config { get; init; }
    }
}
