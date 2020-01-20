
Shader "KYU3D/Effect/Lightning"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Color( "_Color", Color ) = ( 1, 1, 1, 1 )
		_Intensity( "_Intensity", float ) = 1
		_Width( "_Width", float ) = 0.1
		_Scaling( "_Scaling", float ) = 2
		_Speed( "_Speed", float ) = 2
		_Direction( "_Direction", Vector ) = ( -2, 0, -2, 1 )
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
			Cull Off
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			half4 _Color;
			half _Intensity;
			half _Width;
			float _Scaling;
			float _Speed;
			float3 _Direction;

			v2f vert( appdata_full v )
			{
				v2f o;
				float3 p0 = float3( unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3] );
				float l = length( _Direction );
				float3 en = _Direction / l;
				float3 n = cross( en, UNITY_MATRIX_V[2] );
				p0 += v.vertex.x * n * _Width + v.vertex.y * _Direction;
				o.pos = mul( UNITY_MATRIX_VP, float4( p0, 1 ) );
				o.uv.x = v.vertex.x + 0.5;
				o.uv.y = v.vertex.y * _Scaling * l - _Speed * _Time.y;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				c *= _Color * _Intensity;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Transparent"
}
