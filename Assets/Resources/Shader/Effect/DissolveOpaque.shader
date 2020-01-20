// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Effect/DissolveOpaque"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Intensity( "_Intensity", float ) = 1
		_Noise( "_Noise", 2D ) = "gray" {}
		_NoiseScale( "_NoiseScale", float ) = 2
		_Color1( "_Color1", Color ) = ( 1, 0.8, 0.4, 1 )
		_Color2( "_Color2", Color ) = ( 1, 0.5, 0, 1 )
		_Range1( "_Range1", float ) = 0.05
		_Range2( "_Range2", float ) = 0.1
		_Range3( "_Range3", float ) = 0.05
		_Range4( "_Range4", float ) = 0.1
		_Start( "_Start", float ) = 0.2
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half start : TEXCOORD1;
			};

			sampler2D _MainTex;
			half _Intensity;
			sampler2D _Noise;
			half _NoiseScale;
			fixed3 _Color1;
			fixed3 _Color2;
			half _Range1;
			half _Range2;
			half _Range3;
			half _Range4;
			half _Start;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.start = v.color.a;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half n = tex2D( _Noise, i.uv * _NoiseScale ).g;
				half4 c = tex2D( _MainTex, i.uv );
				c.rgb *= _Intensity;
				n -= _Start + i.start;
				clip( n );
				half r3 = _Range1 + _Range2 + _Range3;
				if ( n < _Range1 )
				{
					c.rgb = _Color1;
				}
				else if ( n < _Range1 + _Range2 )
				{
					c.rgb = lerp( _Color1, _Color2, ( n - _Range1 ) / _Range2 );
				}
				else if ( n < r3 )
				{
					c.rgb = _Color2;
				}
				else if ( n < r3 + _Range4 )
				{
					c.rgb = lerp( _Color2, c.rgb, ( n - r3 ) / _Range4 );
				}
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Simple/Texture"
}
