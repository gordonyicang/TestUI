Shader "KYU3D/Scene/StaticObjLow"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AlbedoIntensity("_AlbedoIntensity",float) = 1
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Geometry-50"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// // Fog {Mode Off}

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
				// half4 fogCoord:TEXCOORD2;
				// UNITY_FOG_COORDS(2)
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			half _LightmapIntensity;
			half _AlbedoIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				// // o.fog = FogColor( o.pos.w, v.vertex );

				UNITY_TRANSFER_FOG(o,o.pos);
				
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				c.rgb *= _AlbedoIntensity;
				
				fixed3 lm;
				lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
				c.rgb *= lm;
				// c.rgb *= min(i.tmp,1);
				// UNITY_APPLY_FOG(i.fogCoord, c);
				// apply fog
				

				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Texture"
}
