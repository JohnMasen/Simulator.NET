using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NuralCellTest.Core;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NurallCellTest.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        List<(string Name, float Value)> values = new();
        public int TotalSteps { get; set; } = 1000;
        public bool EnableBrackets { get;
            set
            {
                field = value;
                RenderChart();
            } } = true;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NuralCell cell = new NuralCell();
            cell.BaseVoltage = -0.2f;
            cell.ActivationVoltage = 1.2f;
            cell.Threashold = 0.4f;
            cell.ActivationDelay = 3;
            cell.ThreasholdHigh = 0.7f;
            cell.RaiseStepper = _ => 0.5f;
            cell.FallStepper = _ => -0.5f;
            cell.RestoreStepper = _ => 0.002f;
            values.Clear();
            for (int i = 0; i < TotalSteps; i++)
            {
                //if (cell.CellStatus!=lastStatus)
                //{
                //    currentValues = new List<float>();
                //    values.Add((cell.CellStatus.ToString(), currentValues));
                //    lastStatus = cell.CellStatus;
                //}
                if (cell.CellStatus==NuraCellStatusEnum.Idle)
                {
                    cell.InputVoltage =Random.Shared.NextSingle();
                }
                else
                {
                    cell.InputVoltage = 0f;
                }
                    cell.Step();
                values.Add((cell.CellStatus.ToString(), cell.Voltage));
            }

            RenderChart();
        }

        private void RenderChart()
        {
            if (!values.Any())
            {
                return;
            }
            WinUIPlot1.Plot.Clear();
            WinUIPlot1.Plot.Axes.SetLimits(0, TotalSteps, -1, 2);
            WinUIPlot1.Plot.Add.Signal(values.Select(x => x.Value).ToList());
            if (EnableBrackets)
            {
                int idx = 0;
                string lastStatus = values.First().Name;
                int lastPos = 0;
                float lastValue = values.First().Value;
                foreach (var item in values)
                {
                    if (lastStatus != item.Name)
                    {
                        WinUIPlot1.Plot.Add.Bracket(lastPos, lastValue, idx, item.Value, lastStatus);
                        lastStatus = item.Name;
                        lastPos = idx;
                        lastValue = item.Value;
                    }
                    idx++;
                }
            }

            WinUIPlot1.Refresh();
        }
    }
}
