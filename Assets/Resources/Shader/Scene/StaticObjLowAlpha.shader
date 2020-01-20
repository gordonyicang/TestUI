Shader "KYU3D/Scene/StaticObjLowAlpha" 
{
	Properties 
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Alpha( "_Alpha", 2D ) = "gray" {}
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
		_AlphaIntensity( "_AlphaIntensity", float ) = 1
	}
	SubShader 
	{
		LOD 200
		
		Tags 
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True"
		}
		Pass 
		{
			Lighting Off 
//Fog { Mode Off }
			Cull Off 
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma vertex vert 
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_fastest_hint
			#include"../KyUnity.cginc"

			struct v2f 
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			sampler2D _Alpha;
			half _LightmapIntensity;
			half _AlphaIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : Color 
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				c.a = tex2D( _Alpha, i.uv ).r * _AlphaIntensity;
				fixed3 lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}		
	}
	FallBack "Diffuse"
}
