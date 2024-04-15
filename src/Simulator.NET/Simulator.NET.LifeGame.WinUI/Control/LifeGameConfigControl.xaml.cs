using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.LifeGame.WinUI.Control
{
    [ObservableObject]
    public sealed partial class LifeGameConfigControl : UserControl
    {
        [ObservableProperty]
        private int dataGridWidth  = 4096;
        [ObservableProperty]
        private int dataGridHeight = 4096;

        [ObservableProperty]
        private GraphicsDevice selectedDevice;


        private ObservableCollection<GraphicsDevice> DeviceList { get; init; } 
        public LifeGameConfigControl()
        {
            this.InitializeComponent();
            DeviceList = new ObservableCollection<GraphicsDevice>(GraphicsDevice.EnumerateDevices());
            SelectedDevice = GraphicsDevice.GetDefault();
        }
    }
}
