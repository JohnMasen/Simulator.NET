using System;
using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComputeSharp;
using ComputeSharp.D2D1;
using ComputeSharp.D2D1.WinUI;
using ComputeSharp.WinUI;
using ComputeSharpTest1.Core;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.DirectX;
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
    //LifeGameEngine engine;
    TransformEngine<LifeGameDItem, LifeGameShader> engine;
    private const int XCOUNT = 2000;
    private const int YCOUNT = 2000;
    Task runGame;
    CancellationTokenSource cts;
    bool isDebug = false;
    TimeSpan engineStepDuration;
    //CanvasCachedGeometry[,] cachedAliveCells;
    //CanvasCachedGeometry[,] cachedDeadCells;
    CanvasBitmap cache;
    byte[] displayBuffer;
    LifeGameRenderShader renderShader;
    public MainWindow()
    {
        this.InitializeComponent();
        //bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 100, 100);
        //var s = new SoftwareBitmapSource();
        //s.SetBitmapAsync(bitmap).AsTask().Wait();

        //engine = new LifeGameEngine(raw, XCOUNT, YCOUNT);

        foreach (var item in GraphicsDevice.EnumerateDevices())
        {
            cmbDevices.Items.Add(item);
        }
        canvas1.Width = XCOUNT;
        canvas1.Height = YCOUNT;
        cmbDevices.SelectedItem = GraphicsDevice.GetDefault();
        initEngine();

    }
    private void initEngine()
    {
        var raw = Random.Shared.GetItems<LifeGameDItem>(new LifeGameDItem[] { new LifeGameDItem() { Value = 1 }, new LifeGameDItem() { Value = 0 } }, XCOUNT * YCOUNT);
        engine = new TransformEngine<LifeGameDItem, LifeGameShader>((GraphicsDevice)cmbDevices.SelectedItem, XCOUNT, YCOUNT, raw, (_source, _target) =>
        {
            return new LifeGameShader(_source, _target, XCOUNT, YCOUNT);
        });
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
        if (cache == null)
        {
            displayBuffer = new byte[XCOUNT * YCOUNT * 16]; //w*h*pixel in bytes
            cache = CanvasBitmap.CreateFromBytes(sender, displayBuffer, XCOUNT, YCOUNT, Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float);
        }
        
        using (var mo = MemoryPool<LifeGameDItem>.Shared.Rent(XCOUNT * YCOUNT))
        {
            var data = mo.Memory.Slice(0, XCOUNT * YCOUNT);
            engine.GetOutput(data);
            sw.Start();
            Span<float4> m = MemoryMarshal.Cast<byte, float4>(displayBuffer.AsSpan());
            
            for (int i = 0; i < XCOUNT*YCOUNT; i++)
            {
                if (data.Span[i].Value == 0)
                {
                    m[i].R = 1f;
                    m[i].G = 1f;
                    m[i].B = 1f;
                }
                else
                {
                    m[i].R = 0;
                    m[i].G = 0;
                    m[i].B = 0;
                }
                m[i].W = 1f;
            }
            cache.SetPixelBytes(displayBuffer);
            
            args.DrawingSession.DrawImage(cache,new Rect(0,0,sender.ActualWidth,sender.ActualHeight));

            //Parallel.For(0, YCOUNT, y =>
            //{
            //    for (int x = 0; x < XCOUNT; x++)
            //    {
            //        int idx = y * XCOUNT + x;
            //        var v = data.Span[idx];
            //        if (isDebug)
            //        {
            //            //string debugInfo = $"{v.Value}({v.Count}:{v.LoopCount}[X:{v.XStart}~{v.XEnd} Y:{v.YStart}~{v.YEnd}])";
            //            //debugInfo = $"[{v.Value},{v.Count},{v.LastValue}]{getMatrixValue(v.calcItem)}";
            //            //args.DrawingSession.DrawRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
            //            //args.DrawingSession.DrawText(debugInfo, new Rect(x * unitWidth, y * unitHeight, unitWidth, unitHeight), aliveBrush, format);
            //        }
            //        else
            //        {
            //            if (data.Span[y * XCOUNT + x].Value == 1)
            //            {

            //                args.DrawingSession.FillRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
            //            }
            //            else
            //            {
            //                //args.DrawingSession.DrawRectangle(x * unitWidth, y * unitHeight, unitWidth, unitHeight, aliveBrush);
            //            }
            //        }
            //    }
            //});
            //for (int y = 0; y < YCOUNT; y++)
            //{

            //}
            sw.Stop();
            txtRenderTime.Text = $"{sw.ElapsedMilliseconds}ms";
        }
        

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
        //var data = engine.GetOutput();
        engine.WithOutput(data =>
        {
            PixelShaderEffect<TestShader> shader = new PixelShaderEffect<TestShader>();
            shader.ConstantBuffer = new TestShader(new int2((int)sender.ActualWidth, (int)sender.ActualHeight));
            args.DrawingSession.DrawImage(shader);
        });

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
        //var raw = Random.Shared.GetItems<int>(new int[] { 1, 0 }, XCOUNT * YCOUNT);
        //engine = new LifeGameEngine(raw, XCOUNT, YCOUNT);
        initEngine();
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
        int callCount = 0;
        runGame = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                Stopwatch sw = Stopwatch.StartNew();
                engine.Step();
                sw.Stop();
                engineStepDuration = sw.Elapsed;
                DispatcherQueue?.TryEnqueue(() =>
                {
                    try
                    {
                        txtEngineTime.Text = $"Engine:{engineStepDuration.Milliseconds} CallCount:{callCount++}";
                    }
                    catch (Exception)
                    {

                    }
                    
                });

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
