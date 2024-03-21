using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using ComputeSharp.D2D1;

namespace ComputeSharpTest1.Core;
[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct TestShader(int2 dispatchSize) : ID2D1PixelShader
{

    public float4 Execute()
    {
        int2 xy = (int2)D2D.GetScenePosition().XY;
        float2 uv = xy / (float2)dispatchSize;
        float4 color = 1;
        color.R = uv.X;
        color.G = uv.Y;
        int pos = xy.Y * dispatchSize.X + xy.X;
        //color = (float)data[pos].Value/255;
        return color;
    }
}
