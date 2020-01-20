// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Effect/Opaque"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Color( "_Color", Color ) = ( 1, 1, 1, 1 )
		_Intensity( "_Intensity", Range( 0.1, 2 )) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Geometry+500"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Off
			ZTest LEqual
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : TEXCOORD1;
			};

			sampler2D _MainTex;
			fixed _Intensity;
			fixed4 _Color;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.color = v.color * _Intensity * _Color;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				c *= i.color;
				return c;
			}

			ENDCG
		}
	}
	
    Fallback "KYU3D/Simple/Texture"
}
