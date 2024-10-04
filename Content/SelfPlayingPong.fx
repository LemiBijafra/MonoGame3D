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
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD;
};

struct VSOutputTx
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};


float fTimer;

float Noise21 (float2 p, float ta, float tb) {
    return frac(sin(p.x * ta + p.y * tb) * 5678.);
}

#define OFFSET 1.3541
#define RADIUS 0.05
#define SIZE float2(0.02,0.3)
#define TIME_SCALE 0.5

#define NORM float2(iResolution.x/iResolution.y,1.0)

float frac(float c) {
	return c - floor(c);
}

float lerp(float a, float b, float t) {
	return a + (b-a) * t;
}

float pingpong(float t) {
	return abs(frac(t)-0.5)*2.0;
}

float2 ball(float t) {
    return float2(pingpong(t),pingpong(t*OFFSET));
}

float getPos(float t) {
    return clamp(lerp(ball(ceil(t)).y,ball(ceil(t+1.0)).y,frac(t)),SIZE.y*0.5,1.0-SIZE.y*0.5);
}

//	This is our Pixel Shader function.  It takes in the output from the vertex shader
//	and returns back a float4 containing the color data for the pixel.
float4 MainPS(VSOutputTx input) : COLOR
{
    float2 iResolution = float2(1024, 768);
#if 1
    float2 uv = input.TexCoord; // doesn't work
#else
    float2 uv = input.Position.xy / iResolution.xy * 2.;
#endif
    // Basic test if everything OK with UVs:
    // ->
    //if (uv.x < 0.5)
    //{
    //    return float4(0., 0., 1., 1);
    //}
    //return float4(0., 1., 0., 1);
    // <-
    
    float t = fTimer * TIME_SCALE;

 	float a = 0.;

	a += step(length((uv-ball(t))*NORM),RADIUS);
    
    a += step(uv.x,SIZE.x) * step(abs(uv.y-getPos(t+0.5)),SIZE.y*0.5);
    
    a += step(1.0-SIZE.x,uv.x) * step(abs(uv.y-getPos(t-1.0)),SIZE.y*0.5);

	return float4(float3(a, a, a), 1.0);
}

float4x4 WorldViewProjection;

VSOutputTx VSBasicTx(VSInputTx vin)
{
    VSOutputTx vout;
    
    //vout.Position = vin.Position;
    vout.Position = mul(vin.Position, WorldViewProjection);
    vout.TexCoord = vin.TexCoord;

    return vout;
}

technique Zhmuda
{
	pass P0
	{
        // Why doesn't this work?
        VertexShader = compile VS_SHADERMODEL VSBasicTx();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};