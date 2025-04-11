using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.Worms.WinUI.Control
{
    [INotifyPropertyChanged]
    public sealed partial class WormConfigControl : UserControl
    {
        [ObservableProperty]
        private int wormsCount = 100;

        [ObservableProperty]
        private int gridWidth = 1000;
        [ObservableProperty]
        private int gridHeight = 1000;

        [ObservableProperty]
        private float navigationCapacityUI = 70f;
        [ObservableProperty]
        private float stability = 0.8f;


        public float NavigationCapacity => NavigationCapacityUI * 0.01f;

        [ObservableProperty]
        private Color headColorUI = Color.FromArgb(255, 255, 0, 0);

        public float4 HeadColor => new(HeadColorUI.R/255f, HeadColorUI.G/255f, HeadColorUI.B/255f, HeadColorUI.A/255f);
        public WormConfigControl()
        {
            this.InitializeComponent();
        }
    }
}
