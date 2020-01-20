// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "KYU3D/Scene/Sky"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "blue" {}
		_Cloud1( "_Cloud1", 2D ) = "gray" {}
		_Scale1( "_Scale1", float ) = 2
		_SpeedU1( "_SpeedU1", float ) = 0
		_SpeedV1( "_SpeedV1", float ) = 0.25
		_Cloud2( "_Cloud2", 2D ) = "gray" {}
		_Scale2( "_Scale2", float ) = 2
		_SpeedU2( "_SpeedU2", float ) = 0.25
		_SpeedV2( "_SpeedV2", float ) = 0
		_Color( "_Color", Color ) = ( 1, 1, 1, 1 )
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Background"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
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
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _Cloud1;
			half _Scale1;
			half _SpeedU1;
			half _SpeedV1;
			sampler2D _Cloud2;
			half _Scale2;
			half _SpeedU2;
			half _SpeedV2;
			half3 _Color;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4x4 w = unity_ObjectToWorld;
				w[0][3] = _WorldSpaceCameraPos[0];
				w[1][3] += _WorldSpaceCameraPos[1];
				w[2][3] = _WorldSpaceCameraPos[2];
				o.pos = mul( mul( UNITY_MATRIX_VP, w ), v.vertex );
				o.pos.z = o.pos.w;
				o.uv = v.texcoord.xy;
				o.uv1 = ( v.texcoord.xy + half2( _SpeedU1, _SpeedV1 ) * _Time.y ) * _Scale1;
				o.uv2 = ( v.texcoord.xy + half2( _SpeedU2, _SpeedV2 ) * _Time.y ) * _Scale2;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 c1 = tex2D( _Cloud1, i.uv1 );
				half4 c2 = tex2D( _Cloud2, i.uv2 );
				c.rgb += ( c1.rgb - c.rgb ) * c1.a + ( c2.rgb - c.rgb ) * ( c2.a - c2.a * c1.a );
				c.rgb *= _Color;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Texture"
}
