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

// Effect properties
// ->
float4x4 WorldViewProjection;
float4 DiffuseColor;
float fTimer;
Texture2D DiffuseTexture;
// <-

//	The sampler object we'll use to sample the texture
sampler2D TextureSampler = sampler_state
{
    Texture = <DiffuseTexture>;
};

VSOutputTx VSMain(VSInputTx vin)
{
    VSOutputTx vout;
    
    vout.Position = mul(vin.Position, WorldViewProjection);
    vout.TexCoord = vin.TexCoord;

    return vout;
}

float4 PSMain(VSOutputTx input) : COLOR
{
    float2 uv = input.TexCoord;
    float4 texColor = tex2D(TextureSampler, float2(uv.x, 1. - uv.y));
    return texColor + DiffuseColor;
}

technique Phong
{
    pass P0
    {
        // Why doesn't this work?
        VertexShader = compile VS_SHADERMODEL VSMain();
        PixelShader = compile PS_SHADERMODEL PSMain();
    }
};