using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Simulator.NET.WinUI.View;
using Simulator.NET.WinUI.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Simulator.NET.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainView ViewModel { get; private set; }
        public MainWindow(MainView view)
        {
            this.InitializeComponent();
            this.ViewModel = view;
            //ViewModel=App.Current.Services.GetService<MainView>();
        }

        

        //private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        //{
        //    ViewModel.OnDraw(sender, args);
        //}

        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    myButton.Content = "Clicked";
        //}
    }
}
