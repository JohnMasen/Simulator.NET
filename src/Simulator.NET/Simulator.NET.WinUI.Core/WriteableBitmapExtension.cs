using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.WinUI.Core
{
    public static class WriteableBitmapExtension
    {
        public static void CopyFromSpan<T>(this WriteableBitmap bitmap, Span<T> buffer) where T:struct
        {
            bitmap.PixelBuffer.AsStream().Write(MemoryMarshal.AsBytes(buffer));
        }
    }
}
