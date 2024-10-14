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


float rand(float2 p){
	p+=.2127+p.x+.3713*p.y;
	float2 r=4.789*sin(789.123*(p));
	return frac(r.x*r.y);
}

float sn(float2 p){
	float2 i=floor(p-.5);
	float2 f=frac(p-.5);
	f = f*f*f*(f*(f*6.0-15.0)+10.0);
	float rt=lerp(rand(i),rand(i+float2(1.,0.)),f.x);
	float rb=lerp(rand(i+float2(0.,1.)),rand(i+float2(1.,1.)),f.x);
	return lerp(rt,rb,f.y);
}

float4 MainPS(VSOutputTx input) : COLOR
{
    float2 uv = input.TexCoord;   // doesn't work    
    //if (uv.x < 0.5)
    //{
    //    return float4(0., 0., 1., 1);
    //}
    //return float4(0., 1., 0., 1);
    
    // Mouse support
    //uv+=iMouse.xy/ iResolution.xy;
	
	float2 p=uv.xy*float2(3.,4.3);
	float f =
	.5*sn(p)
	+.25*sn(2.*p)
	+.125*sn(4.*p)
	+.0625*sn(8.*p)
	+.03125*sn(16.*p)+
	.015*sn(32.*p)
	;
	
    float newT = fTimer * 0.4 + sn(float2(fTimer * 1., fTimer * 1.)) * 0.1;
	p.x-=fTimer * .01;
	
	p.y*=1.3;
	float f2=
	.5*sn(p)
	+.25*sn(2.04*p+newT*1.1)
	-.125*sn(4.03*p-fTimer*0.3)
	+.0625*sn(8.02*p-fTimer*0.4)
	+.03125*sn(16.01*p+fTimer*0.5)+
	.018*sn(24.02*p);
	
	float f3=
	.5*sn(p)
	+.25*sn(2.04*p+newT*1.1)
	-.125*sn(4.03*p-fTimer*0.3)
	+.0625*sn(8.02*p-fTimer*0.5)
	+.03125*sn(16.01*p+fTimer*0.6)+
	.019*sn(18.02*p);
	
	float f4 = f2*smoothstep(0.0,1.,uv.y);
	
	float3 clouds = lerp(float3(-0.4,-0.4,-0.15),float3(1.4,1.4,1.3),f4*f);
    float lightning = sn((f3) + float2(pow(sn(float2(fTimer * 4.5, fTimer * 4.5)), 6.), pow(sn(float2(fTimer * 4.5, fTimer * 4.5)), 6.)));

	lightning *= smoothstep(0.0,1.,uv.y+0.5);

	lightning = smoothstep(0.76,1.,lightning);
	lightning=lightning*2.;
	
	float2 moonp = float2(0.5,0.8);
	float moon = smoothstep(0.95,0.956,1.-length(uv-moonp));
	float2 moonp2 = moonp + float2(0.015, 0);
	moon -= smoothstep(0.93,0.956,1.-length(uv-moonp2));
	moon = clamp(moon, 0., 1.);
	moon += 0.3*smoothstep(0.80,0.956,1.-length(uv-moonp));

	clouds+= pow(1.-length(uv-moonp),1.7)*0.3;

	clouds*=0.8;
	clouds += lightning + moon +0.2;

	float ground = smoothstep(0.07,0.075,f*(p.y-0.98)+0.01);
	
	float2 newUV = uv;
	newUV.x-=fTimer*0.3;
	newUV.y+=fTimer*3.;
	float strength = sin(fTimer*0.5+sn(newUV))*0.1+0.15;
	
	float rain = sn( float2(newUV.x*20.1, newUV.y*40.1+newUV.x*400.1-20.*strength ));
	float rain2 = sn( float2(newUV.x*45.+fTimer*0.5, newUV.y*30.1+newUV.x*200.1 ));	
	rain = strength-rain;
	rain+=smoothstep(0.2,0.5,f4+lightning+0.1)*rain;
	rain += pow(length(uv-moonp),1.)*0.1;
	rain+=rain2*(sin(strength)-0.4)*2.;
	rain = clamp(rain, 0.,0.5)*0.5;
	
	float tree = 0.;
	float2 treeUV = uv;

	{
		float atree = abs(atan2(treeUV.x*59.-85.5,3.-treeUV.y*25.+5.));
		atree +=rand(treeUV.xy*float2(0.001,1.))*atree;
		tree += clamp(atree, 0.,1.)*0.33;
	}
	{
		float atree = abs(atan2(treeUV.x*65.-65.5,3.-treeUV.y*19.+4.));
		atree +=rand(treeUV.xy*float2(0.001,1.))*atree;
		tree += clamp(atree, 0.,1.)*0.33;
	}
	{
		float atree = abs(atan2(treeUV.x*85.-91.5,3.-treeUV.y*21.+4.));
		atree +=rand(treeUV.xy*float2(0.001,1.))*atree;
		tree += clamp(atree, 0.,1.)*0.34;
	}
	tree = smoothstep(0.6,1.,tree);
	
	float3 painting = tree*ground*(clouds + rain)+clamp(rain*(strength-0.1),0.,1.);
	
	float r=1.-length(max(abs(uv*2.-1.)-.5,0.)); 
	painting*=r;
	
	return float4(painting, 1.);
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
technique StormyNight
{
	pass P0
	{
        // Why doesn't this work?
        VertexShader = compile VS_SHADERMODEL VSBasicTx();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};