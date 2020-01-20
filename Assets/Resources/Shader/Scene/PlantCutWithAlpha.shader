Shader "KYU3D/Scene/PlantCutWithAlpha" 
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AlphaTex( "_AlphaTex( R )", 2D ) = "gray" {}
		_CutOff( "_CutOff", range( 0, 1 ) ) = 0.7
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Geometry-15"
			"IgnoreProjector" = "true"
		}

		Pass
		{
			Lighting Off
			//Fog { Mode Off }
			Cull Off
			ZTest LEqual
			ZWrite On

			CGPROGRAM 

			#pragma vertex vert 
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed _CutOff;
			half _LightmapIntensity;

			struct v2f 
			{
				float4 pos : SV_POSITION;
				half2 uv_MainTex : TEXCOORD0;
				half2 uv_Lightmap : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos =UnityObjectToClipPos( v.vertex );
				o.uv_MainTex = v.texcoord.xy;
				o.uv_Lightmap = LightMapUV( v.texcoord1 );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ):SV_Target
			{
				fixed alpha = tex2D( _AlphaTex, i.uv_MainTex ).r;
				clip( alpha - _CutOff );
				fixed4 c = tex2D( _MainTex, i.uv_MainTex );
				fixed3 lm;
				lm = LightMapColor( i.uv_Lightmap ) * _LightmapIntensity;
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}

	FallBack "KYU3D/Simple/Cut"
}
