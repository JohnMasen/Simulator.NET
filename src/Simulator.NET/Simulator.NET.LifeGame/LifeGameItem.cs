using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.LifeGame
{
    public struct LifeGameItem
    {
        public int Value;
        public int LifeCount;
        public int ContinuLifeCount;
        /// <summary>
        /// 0=no change, 1=birth, 2=dead
        /// </summary>
        public uint Reason;
    }
}
