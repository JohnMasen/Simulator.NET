using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ComputeSharp;
using ComputeSharp.Resources;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using Simulator.NET.WinUI.Core;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.GrayScott.WinUI.Control
{
    [ObservableObject]
    public sealed partial class GrayScottConfigControl : UserControl
    {
        [ObservableProperty]
        private int dataGridWidth = 100;
        [ObservableProperty]
        private int dataGridHeight = 100;
        [ObservableProperty]
        private float feed = 0.055f;
        [ObservableProperty]
        private float killRate = 0.062f;

        Memory<GrayScottItem>? startupItems = null;
        [ObservableProperty]
        private GraphicsDevice selectedDevice;


        private ImmutableList<GraphicsDevice> DeviceList { get; init; }
        public GrayScottConfigControl()
        {
            InitializeComponent();
            DeviceList = ImmutableList.Create([.. GraphicsDevice.EnumerateDevices()]);
            SelectedDevice = GraphicsDevice.GetDefault();
        }
        [RelayCommand]
        private async Task LoadFromSampleImageAsync()
        {
            using MemoryStream ms = new MemoryStream(Resource1.GrayScottSample);
            await internalLoadFromImageAsync(ms.AsRandomAccessStream());
        }

        [RelayCommand]
        private async Task LoadFromImageAsync()
        {
            var picker = new FileOpenPicker(PickerHelper.MainWindowID);
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                StorageFile sf = await StorageFile.GetFileFromPathAsync(file.Path);
                using (var s = await sf.OpenReadAsync())
                {
                    await internalLoadFromImageAsync(s);
                }
            }
        }

        private async Task internalLoadFromImageAsync(IRandomAccessStream s)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(s);
            BitmapTransform transform = new BitmapTransform();
            var pixels = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
            DataGridHeight = (int)decoder.PixelHeight;
            DataGridWidth = (int)decoder.PixelWidth;
            var pixelBuffer = pixels.DetachPixelData();

            //TODO: ugly implementation, should fix with Tensor
            startupItems = new Memory<GrayScottItem>(new GrayScottItem[pixelBuffer.Length]);
            Parallel.For(0, (int)(decoder.PixelHeight * decoder.PixelWidth), (int idx) =>
            {
                GrayScottItem tmp = new GrayScottItem();
                var pos = idx * 4;
                tmp.U = pixelBuffer[pos++] / 255f;//blue for U
                tmp.V = pixelBuffer[pos] / 255f;//green for V
                //pos += 2; //skip red and alpha channel
                startupItems.Value.Span[idx] = tmp;
            });
        }

        [RelayCommand]
        private void ResetDefault()
        {
            Feed = 0.055f;
            KillRate = 0.062f;
        }
        public Memory<GrayScottItem> CreateStartupItems()
        {
            if (startupItems is not null)
            {
                return startupItems.Value;
            }
            List<GrayScottItem> items = new List<GrayScottItem>()
            {
                new GrayScottItem(){U=1,V=1f},
                new GrayScottItem(){U=1,V=0},
                //new GrayScottItem(){U=1.5f,V=0}
            };
            return Random.Shared.GetItems<GrayScottItem>(items.ToArray().AsSpan(), DataGridHeight * DataGridWidth);
            //return new GrayScottItem[] {
            //    new GrayScottItem(){ U=1, V = 0 },new GrayScottItem(){ U=1, V = 0 },new GrayScottItem(){ U=1, V = 0 },
            //    new GrayScottItem(){ U=1, V = 0 },new GrayScottItem(){ U=1, V = 1 },new GrayScottItem(){ U=1, V = 0 },
            //    new GrayScottItem(){ U=1, V = 0 },new GrayScottItem(){ U=1, V = 0 },new GrayScottItem(){ U=1, V = 0 }
            //};
        }

        
    }
}
