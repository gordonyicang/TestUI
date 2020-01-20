// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/PostProcess/Sample4Pix"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
	}

	SubShader
	{
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off

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
			};

			sampler2D _MainTex;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				return tex2D( _MainTex, i.uv );
			}

			ENDCG
		}
	}

	FallBack off
}
