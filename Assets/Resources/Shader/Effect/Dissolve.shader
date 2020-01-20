
Shader "KYU3D/Effect/Dissolve"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Noise( "_Noise", 2D ) = "gray" {}
		_NoiseScale( "_NoiseScale", float ) = 2
		_Color1( "_Color1", Color ) = ( 1, 0.8, 0.4, 1 )
		_Color2( "_Color2", Color ) = ( 1, 0.5, 0, 1 )
		_Range1( "_Range1", float ) = 0.05
		_Range2( "_Range2", float ) = 0.1
		_Range3( "_Range3", float ) = 0.05
		_Range4( "_Range4", float ) = 0.1
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
			Cull Off
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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
				half2 uv1 : TEXCOORD1;
				half start : TEXCOORD2;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			sampler2D _Noise;
			half _NoiseScale;
			fixed3 _Color1;
			fixed3 _Color2;
			half _Range1;
			half _Range2;
			half _Range3;
			half _Range4;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv1 = v.texcoord1.xy;
				o.start = 1.5 - v.color.a * 2;
				return o;
			}
			
			fixed4 frag( v2f i ) : COLOR
			{
				half n = tex2D( _Noise, i.uv1 * _NoiseScale ).g;
				half4 c = tex2D( _MainTex, i.uv );
				n -= i.start;
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
