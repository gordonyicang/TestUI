// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KYU3D/Scene/PlantBlendWithAlpha" 
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AlphaTex( "_AlphaTex( R )", 2D ) = "gray" {}
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		Tags{ 
				"Queue" = "Geometry-15" 
				"IgnoreProjector" = "true"
			}
		
		Pass
		{
			LOD 200

			Lighting Off
			//Fog { Mode Off }
			Cull Off
			ZTest LEqual
			Zwrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex  vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"
			
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			half _LightmapIntensity;

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv_MainTex : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv_MainTex = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ):SV_Target
			{
				fixed4 c = tex2D( _MainTex, i.uv_MainTex );
				fixed3 lm;
				lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.a = tex2D( _AlphaTex, i.uv_MainTex ).r;
				return c;
			}

			ENDCG
		}
	}

	FallBack "KYU3D/Simple/Transparent"
}
