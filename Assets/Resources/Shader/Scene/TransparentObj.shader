
Shader "KYU3D/Scene/TransparentObj"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Mask( "Opaque(G)", 2D ) = "gray" {}
		_Opaque( "_Opaque", float ) = 1
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Back
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile _ LIGHTMAP_ON
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			sampler2D _Mask;
			half _Opaque;
			half _LightmapIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				fixed4 m = tex2D( _Mask, i.uv );
			#ifdef LIGHTMAP_ON
				c.rgb *= LightMapColor( i.lmuv ) * _LightmapIntensity;
			#endif
				c.a = m.g * _Opaque;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Transparent"
}
