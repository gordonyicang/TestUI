
Shader "KYU3D/Scene/Grass"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Cutoff( "_Cutoff", Range( 0, 1 ) ) = 0.5
		_HOffset( "_HOffset", float ) = 0.5
		_Frequency( "_Frequency", float ) = 2
		_Amplitude( "_Amplitude", float ) = 0.15
		_Difference( "_Difference", float ) = 2
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
	}

	SubShader
	{
		LOD 300

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
			ZTest LEqual
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile _ LIGHTMAP_ON
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			float _Cutoff;
			float _HOffset;
			float _Frequency;
			float _Amplitude;
			float _Difference;
			float _LightmapIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4 p = mul( unity_ObjectToWorld, v.vertex );
				float of = _HOffset / sqrt( UNITY_MATRIX_V[2][2] * UNITY_MATRIX_V[2][2] / ( UNITY_MATRIX_V[2][0] * UNITY_MATRIX_V[2][0] ) + 1 );
				float s = sign( 0.5 - v.texcoord.x );
				p.xz += _Amplitude * sin( _Frequency * _Time.y + p.xz * _Difference ) * v.texcoord.y;
				p.x -= ( _HOffset - abs( UNITY_MATRIX_V[2][2] / UNITY_MATRIX_V[2][0] ) * of ) * s;
				p.z -= of * sign( UNITY_MATRIX_V[2][0] ) * sign( UNITY_MATRIX_V[2][2] ) * s;
				p.xz -= normalize( UNITY_MATRIX_V[2].xz ) * UNITY_MATRIX_V[2][1] * _HOffset * v.texcoord.y;
				o.pos = mul( UNITY_MATRIX_VP, p );
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				UNITY_TRANSFER_FOG(o,o.pos);
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				clip( c.a - _Cutoff );
			#ifdef LIGHTMAP_ON
				fixed3 lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
			#else
				fixed3 lm = 1;
			#endif
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Scene/PlantFlutterCut"
}
