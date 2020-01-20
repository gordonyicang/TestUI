Shader "KYU3D/Scene/PlantCut"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Cutoff( "_Cutoff", Range( 0, 1 ) ) = 0.5
		_AlbedoIntensity("_AlbedoIntensity",float) = 1
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Geometry-15"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// Fog {Mode Off}
			Cull Off
			ZTest LEqual
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
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
			fixed _Cutoff;
			half _LightmapIntensity;
			half _AlbedoIntensity;

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

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				clip( c.a - _Cutoff );
				c.rgb *= _AlbedoIntensity;
				fixed3 lm = 1;
				lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Cut"
}
