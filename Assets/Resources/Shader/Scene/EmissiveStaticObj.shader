// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Scene/EmissiveStaticObj"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_MainIntensity( "_MainIntensity", float ) = 0.7
		_EmissiveTex( "_EmissiveTex", 2D ) = "green" {}
		_EmissiveIntensity( "_EmissiveIntensity", float ) = 2
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Geometry-45"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			fixed _MainIntensity;
			sampler2D _EmissiveTex;
			fixed _EmissiveIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				UNITY_TRANSFER_FOG(o,o.pos);
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				fixed4 e = tex2D( _EmissiveTex, i.uv );
				fixed3 lm = LightMapColor( i.lmuv );
				c.rgb = c.rgb * lm * _MainIntensity + e.rgb * _EmissiveIntensity;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Scene/StaticObjLow"
}
