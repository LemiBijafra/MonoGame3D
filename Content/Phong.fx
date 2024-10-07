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
    float3 Normal : TEXCOORD0;  // Runtime shader compilation issue if this is declared as `NORMAL`
    float2 TexCoord : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
};

// Effect properties
// ->
float4x4 ModelViewProjMx;
float4x4 ModelViewMx;
float3x3 NormMx;
float3 FlatColor;
float fTimer;
Texture2D DiffuseTexture;
float3 worldCameraPos;

// Point light:
float3 PointLightPos;
float3 PointLightColor;
float PointLightIntensity;
float PointLightDecayExp;

// General light:
//? float3 SpecularIntensity;
float3 SpecularColor;
float Shininess;

// <-


//	The sampler object we'll use to sample the texture
sampler2D TextureSampler = sampler_state
{
    Texture = <DiffuseTexture>;
};

VSOutputTx VSMain(VSInputTx vin)
{
    VSOutputTx vout;
    float4 temp = vin.Position;
    
    vout.Position = mul(temp, ModelViewProjMx);
    vout.WorldPos = mul(temp, ModelViewMx).xyz;
    vout.TexCoord = vin.TexCoord;
    vout.Normal = normalize((mul(vin.Normal, NormMx).xyz));
    return vout;
}

void AddPointLight(VSOutputTx vsOut, inout float3 overallColor)
{
    //!float3 vertNorm = normalize(gl_FrontFacing ? VARY_vertNorm : -VARY_vertNorm);
    const float3 lightDir = normalize(PointLightPos - vsOut.WorldPos);
    const float lightDist = length(PointLightPos - vsOut.WorldPos);
    // DIFFUSE COMPONENT
    const float diffuseAtten = max(0., dot(vsOut.Normal, lightDir));
    const float3 diffuse = diffuseAtten * overallColor * PointLightColor;
    
    // SPECULAR COMPONENT
    const float3 camDir = normalize(-vsOut.WorldPos);
    const float specularCoeff = sign(diffuseAtten) *  pow(max(0.0, dot(camDir, reflect(-lightDir, vsOut.Normal))), Shininess);
    const float3 specular = specularCoeff * SpecularColor * PointLightColor;
    const float decay = PointLightIntensity / (1. + pow(lightDist, PointLightDecayExp));
    overallColor = decay * (diffuse + specular);
}

float4 PSMain(VSOutputTx vsOut) : COLOR
{
    float2 uv = float2(vsOut.TexCoord.x, 1 - vsOut.TexCoord.y);
    float4 texColor = tex2D(TextureSampler, uv);
    // It is assumed that `FlatColor` is set to (0, 0, 0) if no contribution from it is desired:
    float3 overallColor = tex2D(TextureSampler, uv).xyz + FlatColor;
    AddPointLight(vsOut, overallColor);
    return float4(overallColor, 1);
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