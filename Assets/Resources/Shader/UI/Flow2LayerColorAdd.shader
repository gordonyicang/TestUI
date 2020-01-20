// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/UI/Flow2LayerColorAdd"
{
	Properties
	{
		_Tex1( "_Tex1", 2D ) = "gray" {}
		_ScaleU1( "_ScaleU1", float ) = 2
		_ScaleV1( "_ScaleV1", float ) = 2
		_SpeedU1( "_SpeedU1", float ) = 0
		_SpeedV1( "_SpeedV1", float ) = 0.5
		_Intensity1( "_Intensity1", float ) = 0.5

		_Tex2( "_Tex2", 2D ) = "gray" {}
		_ScaleU2( "_ScaleU2", float ) = 2
		_ScaleV2( "_ScaleV2", float ) = 2
		_SpeedU2( "_SpeedU2", float ) = 0
		_SpeedV2( "_SpeedV2", float ) = -0.5
		_Intensity2( "_Intensity2", float ) = 0.5
		
		_Color( "_Color", Color ) = ( 1, 1, 1, 1 )
		_ColorIntensity( "_ColorIntensity", float ) = 1
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
			Fog {Mode Off}
			Cull Back
			ZTest LEqual
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				fixed4 color : TEXCOORD2;
			};
			
			sampler2D _Tex1;
			half _ScaleU1;
			half _ScaleV1;
			half _SpeedU1;
			half _SpeedV1;
			half _Intensity1;
			sampler2D _Tex2;
			half _ScaleU2;
			half _ScaleV2;
			half _SpeedU2;
			half _SpeedV2;
			half _Intensity2;
			half3 _Color;
			half _ColorIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv1 = v.texcoord.xy * float2( _ScaleU1, _ScaleV1 ) + float2( _SpeedU1, _SpeedV1 ) * _Time.y;
				o.uv2 = v.texcoord.xy * float2( _ScaleU2, _ScaleV2 ) + float2( _SpeedU2, _SpeedV2 ) * _Time.y;
				o.color = v.color * _ColorIntensity;
				o.color.rgb *= _Color;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 tex1 = tex2D( _Tex1, i.uv1 );
				half4 tex2 = tex2D( _Tex2, i.uv2 );
				fixed4 c;
				c.rgb = tex1.rgb * _Intensity1 + tex2.rgb * _Intensity2;
				c.a = 1;
				c *= i.color;
				c.rgb *= c.a;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Effect/Add"
}
