using System;
using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComputeSharp;
using ComputeSharp.D2D1;
using ComputeSharp.D2D1.WinUI;
using ComputeSharpTest1.Core;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI;
using Color = Windows.UI.Color;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ComputeSharpTest1;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    //SoftwareBitmap bitmap;
    LifeGameEngine engine;
    private const int XCOUNT = 1500;
    private const int YCOUNT = 1000;
    Task runGame;
    CancellationTokenSource cts;
    bool isDebug = false;
    TimeSpan engineStepDuration;
    CanvasCachedGeometry[,] cachedAliveCells;
    CanvasCachedGeometry[,] cachedDeadCells;
    public MainWindow()
    {
        this.InitializeComponent();
        //bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 100, 100);
        //var s = new SoftwareBitmapSource();
        //s.SetBitmapAsync(bitmap).AsTask().Wait();
        var raw = Random.Shared.GetItems<int>(new int[] { 1, 0 }, XCOUNT * YCOUNT);
        engine = new LifeGameEngine(raw, XCOUNT, YCOUNT);
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        //myButton.Content = "Clicked";

        //int[] raw = Random.Shared.GetItems<int>(new int[] { 1, 0 }, xCount * yCount);
        engine.Step();
        canvas1.Invalidate();
    }
    private void drawLifegameResult(CanvasControl sender, CanvasDrawEventArgs args)
    {
        CanvasSolidColorBrush aliveBrush = new CanvasSolidColorBrush(args.DrawingSession.Device, Color.FromArgb(255, 0, 0, 0));
        float unitWidth = (float)(sender.ActualWidth / XCOUNT);
        float unitHeight = (float)(sender.ActualHeight / YCOUNT);
        CanvasTextFormat format = new CanvasTextFormat();
        format.HorizontalAlignment = CanvasHorizontalAlignment.Center;
        format.VerticalAlignment = CanvasVerticalAlignment.Center;
        Stopwatch sw = new Stopwatch();

        using (var mo = MemoryPool<LiveGameDebugItem>.Shared.Rent(XCOUNT * YCOUNT))
        {
            var data = mo.Memory.Slice(0, XCOUNT * YCOUNT);
            engine.GetOutput(data);
            sw.Start();
            Parallel.For(0, YCOUNT, y =>
            {
                for (int x = 0; x < XCOUNT; x++)
                {
                    int idx = y * XCOUNT + x;
                    var v = data.Span[idx];
                    if (isDebug)
                    {
                        //string debugInfo = $"{v.Value}({v.Count}:{v.LoopCount}[X:{v.XStart}~{v.XEnd} Y:{v.YStart}~{v.YEnd}])";
                        //debugInfo = $"[{v.Value},{v.Count},{v.LastValue}]{getMatrixValue(v.calcItem)}";
                        //args.DrawingSession.DrawRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
                        //args.DrawingSession.DrawText(debugInfo, new Rect(x * unitWidth, y * unitHeight, unitWidth, unitHeight), aliveBrush, format);
                    }
                    else
                    {
                        if (data.Span[y * XCOUNT + x].Value == 1)
                        {

                            args.DrawingSession.FillRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
                        }
                        else
                        {
                            //args.DrawingSession.DrawRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
                        }
                    }
                }
            });
            //for (int y = 0; y < YCOUNT; y++)
            //{

            //}

        }
        sw.Stop();
        txtRenderTime.Text = $"{sw.ElapsedMilliseconds}ms";
        txtEngineTime.Text = $"Engine:{engineStepDuration.Milliseconds}";
        //args.DrawingSession.FillRectangle(new Rect(0, 0, sender.ActualWidth, sender.ActualHeight),);
        //args.DrawingSession.DrawEllipse(155, 115, 80, 30, Colors.Black, 3);
        //args.DrawingSession.DrawText("Hello, world!", 100, 100, Colors.Yellow);
        //args.DrawingSession.DrawRectangle()
    }
    void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        drawLifegameResult(sender, args);
        //drawTextPixelShader(sender, args);
    }

    void drawTextPixelShader(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var data = engine.GetOutput();
        PixelShaderEffect<TestShader> shader = new PixelShaderEffect<TestShader>();
        shader.ConstantBuffer = new TestShader(new int2((int)sender.ActualWidth,(int)sender.ActualHeight));
        args.DrawingSession.DrawImage(shader);
    }
    private string getMatrixValue(int3x3 value)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{value.M11},{value.M12},{value.M13}");
        sb.AppendLine($"{value.M21},{value.M22},{value.M23}");
        sb.AppendLine($"{value.M31},{value.M32},{value.M33}");
        return sb.ToString();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        var raw = Random.Shared.GetItems<int>(new int[] { 1, 0 }, XCOUNT * YCOUNT);
        engine = new LifeGameEngine(raw, XCOUNT, YCOUNT);
        canvas1.Invalidate();
    }

    private void showDebug_Click(object sender, RoutedEventArgs e)
    {
        isDebug = (sender as CheckBox).IsChecked.Value;
        canvas1.Invalidate();
    }

    private void Run_Click(object sender, RoutedEventArgs e)
    {
        cts = new CancellationTokenSource();
        runGame = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                Stopwatch sw = Stopwatch.StartNew();
                engine.Step();
                sw.Stop();
                engineStepDuration = sw.Elapsed;
                canvas1.Invalidate();
                Task.Delay(1).Wait();
            }
        });
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        cts?.Cancel();
    }
}
