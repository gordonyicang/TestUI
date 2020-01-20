// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Effect/FlowMask2LayerAdd"
{
	Properties
	{
		_Tex1( "_Tex1", 2D ) = "gray" {}
		_Scale1( "_Scale1", float ) = 2
		_SpeedU1( "_SpeedU1", float ) = 0
		_SpeedV1( "_SpeedV1", float ) = 0.5
		_Intensity1( "_Intensity1", float ) = 0.5

		_Tex2( "_Tex2", 2D ) = "gray" {}
		_Scale2( "_Scale2", float ) = 2
		_SpeedU2( "_SpeedU2", float ) = 0
		_SpeedV2( "_SpeedV2", float ) = -0.5
		_Intensity2( "_Intensity2", float ) = 0.5

		_MaskTex( "_MaskTex", 2D ) = "gray" {}
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
			// Fog {Mode Off}
			Cull Back
			ZTest LEqual
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv1 : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				half2 uv3 : TEXCOORD2;
			};
			
			sampler2D _Tex1;
			half _Scale1;
			half _SpeedU1;
			half _SpeedV1;
			half _Intensity1;
			sampler2D _Tex2;
			half _Scale2;
			half _SpeedU2;
			half _SpeedV2;
			half _Intensity2;
			sampler2D _MaskTex;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv1 = v.texcoord.xy * _Scale1 + half2( _SpeedU1, _SpeedV1 ) * _Time.y;
				o.uv2 = v.texcoord.xy * _Scale2 + half2( _SpeedU2, _SpeedV2 ) * _Time.y;
				o.uv3 = v.texcoord.xy;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 tex1 = tex2D( _Tex1, i.uv1 );
				half4 tex2 = tex2D( _Tex2, i.uv2 );
				fixed4 c = tex1 * _Intensity1 + tex2 * _Intensity2;
				c *= tex2D( _MaskTex, i.uv3 ).a;
				return c;
			}
			ENDCG
		}
	}
	
	Fallback "KYU3D/Effect/Add"
}
