using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeSharpTest1.Core;

public struct LifeGameItem
{
    public int Value;
    public int LifeCount;
    public int ContinuLifeCount;
    /// <summary>
    /// 0=no change, 1=birth, 2=dead
    /// </summary>
    public uint Reason; 
    //public int XStart;
    //public int YStart;
    //public int XEnd;
    //public int YEnd;
    //public int Count;
    //public int LoopCount;
    //public int3x3 calcItem;
    //public int LastValue;

}
