using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics.Tensors;
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
    TransformEngine<LifeGameItem, LifeGameShader> engine;
    private const int XCOUNT = 4096;
    private const int YCOUNT = 4096;
    Task runGame;
    CancellationTokenSource cts;
    bool isDebug = false;
    TimeSpan engineStepDuration;
    //CanvasCachedGeometry[,] cachedAliveCells;
    //CanvasCachedGeometry[,] cachedDeadCells;
    CanvasBitmap cache;
    byte[] displayBuffer;
    //LifeGameShaderRunner<LifeGameShader> shaderRunner;
    LifeGameRender render;
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
        refreshResult();
    }
    private void initEngine()
    {
        var raw = Random.Shared.GetItems<LifeGameItem>(new LifeGameItem[] { new LifeGameItem() { Value = 1 }, new LifeGameItem() { Value = 0 } }, XCOUNT * YCOUNT);
        engine = new TransformEngine<LifeGameItem, LifeGameShader>((GraphicsDevice)cmbDevices.SelectedItem, new System.Drawing.Size(XCOUNT, YCOUNT), raw, (_source, _target) =>
        {
            return new LifeGameShader(_source, _target, XCOUNT, YCOUNT);
        });
        render = new LifeGameRender((GraphicsDevice)cmbDevices.SelectedItem, new System.Drawing.Size(XCOUNT, YCOUNT));
        //shaderRunner = new LifeGameShaderRunner<LifeGameShader>((GraphicsDevice)cmbDevices.SelectedItem, engine, XCOUNT);
        //cs1.ShaderRunner = shaderRunner;
        //cs1.FrameRequestQueue = shaderRunner.FrameRequestQueue;
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        //myButton.Content = "Clicked";

        //int[] raw = Random.Shared.GetItems<int>(new int[] { 1, 0 }, xCount * yCount);
        engine.Step();
        refreshResult();
        //canvas1.Invalidate();
    }


    private void refreshResult()
    {
        canvas1.Invalidate();
        //shaderRunner.FrameRequestQueue.EnqueueFrame();
    }
    private void drawLifegameResult(CanvasControl sender, CanvasDrawEventArgs args)
    {
        CanvasSolidColorBrush aliveBrush = new CanvasSolidColorBrush(args.DrawingSession.Device, Color.FromArgb(255, 0, 0, 0));
        //float unitWidth = (float)(sender.ActualWidth / XCOUNT);
        //float unitHeight = (float)(sender.ActualHeight / YCOUNT);
        CanvasTextFormat format = new CanvasTextFormat();
        format.HorizontalAlignment = CanvasHorizontalAlignment.Center;
        format.VerticalAlignment = CanvasVerticalAlignment.Center;

        if (cache == null)
        {
            displayBuffer = new byte[XCOUNT * YCOUNT * 16]; //w*h*pixel_in_bytes
            cache = CanvasBitmap.CreateFromBytes(sender, displayBuffer, XCOUNT, YCOUNT, Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float);
        }

        var sw = Stopwatch.StartNew();
        var data = render.Render(engine.GetOutputBuffer());
        sw.Stop();
        MemoryMarshal.AsBytes(data.Span).CopyTo(displayBuffer);

        //using (var mo = MemoryPool<LifeGameItem>.Shared.Rent(XCOUNT * YCOUNT))
        //{
        //    var data = mo.Memory.Slice(0, XCOUNT * YCOUNT);
        //    engine.GetOutput(data);


        //    float4 white = new Float4(1, 1, 1, 1);
        //    float4 black = new Float4(0, 0, 0, 1);

        //    if (isDebug)
        //    {
        //        var p = Partitioner.Create(0, XCOUNT * YCOUNT);
        //        var r = Enumerable.Range(0, XCOUNT * YCOUNT);
        //        Parallel.ForEach(p, range =>
        //        {
        //            Span<float4> m = MemoryMarshal.Cast<byte, float4>(displayBuffer.AsSpan());
        //            for (int i = range.Item1; i < range.Item2; i++)
        //            {
        //                m[i] = data.Span[i].Value == 0 ? white : black;
        //            }
        //        });
        //    }
        //    else
        //    {
        //        Span<float4> m = MemoryMarshal.Cast<byte, float4>(displayBuffer.AsSpan());
        //        for (int i = 0; i < XCOUNT * YCOUNT; i++)
        //        {
        //            m[i] = data.Span[i].Value == 0 ? white : black;
        //        }
        //    }
        //}
        sw.Stop();
        cache.SetPixelBytes(displayBuffer);
        args.DrawingSession.DrawImage(cache, new Rect(0, 0, sender.ActualWidth, sender.ActualHeight));


        txtRenderTime.Text = $"{sw.ElapsedMilliseconds}ms";
    }
    void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        drawLifegameResult(sender, args);
        //drawTextPixelShader(sender, args);
    }

    //void drawTextPixelShader(CanvasControl sender, CanvasDrawEventArgs args)
    //{
    //    //var data = engine.GetOutput();
    //    engine.WithOutput(data =>
    //    {
    //        PixelShaderEffect<TestShader> shader = new PixelShaderEffect<TestShader>();
    //        shader.ConstantBuffer = new TestShader(new int2((int)sender.ActualWidth, (int)sender.ActualHeight));
    //        args.DrawingSession.DrawImage(shader);
    //    });

    //}
    //private string getMatrixValue(int3x3 value)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine($"{value.M11},{value.M12},{value.M13}");
    //    sb.AppendLine($"{value.M21},{value.M22},{value.M23}");
    //    sb.AppendLine($"{value.M31},{value.M32},{value.M33}");
    //    return sb.ToString();
    //}

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        initEngine();
        refreshResult();
    }

    private void showDebug_Click(object sender, RoutedEventArgs e)
    {
        isDebug = (sender as CheckBox).IsChecked.Value;
        refreshResult();
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
                        cts.Token.ThrowIfCancellationRequested();
                        txtEngineTime.Text = $"Engine:{engineStepDuration.Milliseconds} CallCount:{callCount++}";
                    }
                    catch (Exception)
                    {

                    }

                });

                refreshResult();
                Task.Delay(1).Wait();
            }
        });
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        cts?.Cancel();
    }
}
