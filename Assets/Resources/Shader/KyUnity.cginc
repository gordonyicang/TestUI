// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D


#ifndef KYUNITYINC
#define KYUNITYINC

#include "UnityCG.cginc"
#include "UnityUI.cginc"
#include "Lighting.cginc"
#include "TerrainEngine.cginc"
// #include "UnityStandardInput.cginc"

#define PI 3.141592653589793
#define PI2 6.283185307179586
#define PI05 1.570796326794897
#define CUT clip(c.a-0.5)

// sampler2D unity_Lightmap;
// float4 unity_LightmapST;
samplerCUBE _NormalCube;
sampler2D _Shadow;
half3 _Ambient;
half3 _ObjDiffuse;
half3 _ObjSpecular;
half3 _CharDiffuse;
half3 _CharSpecular;
half3 _HarmoneyColor;
half _FogIntensity;
half _FogIncrease;
half _FogStart;
half3 _FogColor1;
half3 _ColorDelta;
half _Color1Y;
half _ColorYDelta;

inline half2 LightMapUV( half4 lmuv )
{
	return lmuv.xy * unity_LightmapST.xy + unity_LightmapST.zw;
}

inline half3 LightMapColor( half2 lmuv )
{
	// return half3(1,1,1);
	half4 m = UNITY_SAMPLE_TEX2D( unity_Lightmap, lmuv );
	// half3 bakedColor = DecodeLightmap(m);
	// return bakedColor;
#if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL)
	return m.rgb * 2;
#else
	return m.rgb * m.a * unity_Lightmap_HDR.x;
#endif
}

inline half4 FogColor( half viewZ, half4 Pos )
{
	half4 o;
	half f = saturate( ( viewZ - _FogStart ) * _FogIncrease );
	f *= f;
	o.a = f * ( 2 - f );
	o.rgb = _ColorDelta * saturate( ( _Color1Y - dot( unity_ObjectToWorld[1], Pos ) ) * _ColorYDelta ) + _FogColor1;
	return o;
}

inline half3 MixFog( half3 color, half4 fog )
{
	return lerp( color, fog.rgb, fog.a );
}

inline half GrayScale( half3 color )
{
	return dot( color, half3( 0.299, 0.587, 0.114 ) );
}

inline half3 GetHarmoneyColor( half3 color )
{
	return _HarmoneyColor * GrayScale( color );
}


#endif
