

using System.Drawing;

namespace Simulator.NET.Core
{
    public interface IProcessor
    {
        void Init(GraphicsDevice device,Size size);
        void BeforeProcess(GraphicsDevice device);
        void AfterProcess(GraphicsDevice device);
    }
}
