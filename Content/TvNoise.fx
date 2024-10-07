#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VSInputTx
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;
};

struct VSOutputTx
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};


float fTimer;


float3 palette(float t)
{
    float3 a = float3(0.5, 0.5, 0.5);
    float3 b = float3(0.5, 0.5, 0.5);
    float3 c = float3(1.0, 1.0, 1.0);
    float3 d = float3(0.263, 0.416, 0.557);

    return a + b * cos(6.28318 * (c * t + d));
}


float4 MainPS(VSOutputTx input) : COLOR
{
    float2 uv = input.TexCoord;   // doesn't work    
    //if (uv.x < 0.5)
    //{
    //    return float4(0., 0., 1., 1);
    //}
    //return float4(0., 1., 0., 1);
    float time = fTimer * 0.1;

    // Generate noise based on 2D coordinates and time
    float noise = frac(sin(dot(uv + time, float2(12.9898, 78.233))) * 43758.5453);

    // Function to generate color based on noise


    // Apply the palette function to the noise value
    float3 col = palette(noise);

    // Output to screen
    return float4(col, 1.0);
}

float4x4 ModelViewProjMx;

VSOutputTx VSBasicTx(VSInputTx vin)
{
    VSOutputTx vout;
    
    //vout.Position = vin.Position;
    vout.Position = mul(vin.Position, ModelViewProjMx);
    vout.TexCoord = vin.TexCoord;

    return vout;
}

//	Here, I've renamed the technique to match what it is actually doing
technique Zhmuda
{
	pass P0
	{
        // Why doesn't this work?
        VertexShader = compile VS_SHADERMODEL VSBasicTx();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};