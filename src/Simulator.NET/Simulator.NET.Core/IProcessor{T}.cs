

using System.Drawing;

namespace Simulator.NET.Core
{
    public interface IProcessor
    {
        bool IsEnabled { get; set; }
        void Init(GraphicsDevice device,Size size);
        void BeforeProcess(GraphicsDevice device);
        void AfterProcess(GraphicsDevice device);
    }
}
