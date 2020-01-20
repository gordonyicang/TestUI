// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Character/BossGhost"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Mask( "Opaque(G), FlowA(B), FlowB(R)", 2D ) = "gray" {}

		_LightX( "_LightX", Range( -1, 1 ) ) = -1
		_LightY( "_LightY", Range( -1, 1 ) ) = 1
		_LightZ( "_LightZ", Range( -1, 1 ) ) = 0

		_AmbientIntensity( "_AmbientIntensity", float ) = 1
		_DiffuseIntensity( "_DiffuseIntensity", float ) = 1
		
		_Noise( "_Noise", 2D ) = "gray" {}
		_NoiseScale( "_NoiseScale", float ) = 2
		_SpeedU( "_SpeedU", float ) = 0
		_SpeedV( "_SpeedV", float ) = -0.15
		_NoiseIntensity( "_NoiseIntensity", float ) = 0.1

		_EmissiveOffset( "_EmissiveOffset", float ) = 0.15
		_Frequency( "_Frequency", float ) = 1
		_EmissivePow( "_EmissivePow", float ) = 2

		_ColorA1( "_ColorA1", Color ) = ( 0.2, 0.3, 1, 1 )
		_ColorA2( "_ColorA2", Color ) = ( 0, 0.8, 0.8, 1 )
		_EmissiveIntensityA( "_EmissiveIntensityA", float ) = 1
		
		_ColorB1( "_ColorB1", Color ) = ( 0.9, 0.3, 0, 1 )
		_ColorB2( "_ColorB2", Color ) = ( 0.9, 0.8, 0, 1 )
		_EmissiveIntensityB( "_EmissiveIntensityB", float ) = 1

		[HideInInspector]_Opaque( "_Opaque", float ) = 1
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
			ColorMask 0
			ZWrite On
			ZTest Less

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
			};

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				return 1;
			}

			ENDCG
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half dfs : COLOR0;
				float4 uvFlow : TEXCOORD1;
				half3 normal : TEXCOORD2;
				half3 view : TEXCOORD3;
				UNITY_FOG_COORDS(4)
				
			};

			sampler2D _MainTex;
			sampler2D _Mask;
			half _LightX;
			half _LightY;
			half _LightZ;
			half _AmbientIntensity;
			half _DiffuseIntensity;
			sampler2D _Noise;
			half _NoiseScale;
			half _SpeedU;
			half _SpeedV;
			half _NoiseIntensity;
			half3 _ColorA1;
			half3 _ColorA2;
			half3 _ColorB1;
			half3 _ColorB2;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				half3 l = normalize( half3( _LightX, _LightY, _LightZ ) );
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.dfs = max( dot( l, o.normal ), 0 ) * _DiffuseIntensity;
				o.uvFlow.xy = o.uv * _NoiseScale + float2(_SpeedV, -_SpeedU ) * _Time.y;
				o.uvFlow.zw = o.uv * _NoiseScale + float2(_SpeedU, _SpeedV ) * _Time.y;
				UNITY_TRANSFER_FOG(o,o.pos);
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 m = tex2D( _Mask, i.uv );
				half n1 = tex2D( _Noise, i.uvFlow.zw ).g;
				half n2 = tex2D( _Noise, i.uvFlow.xy + n1 * _NoiseIntensity ).g;
				half r = 1 - dot( normalize( i.normal ), normalize( i.view ) );
				c.rgb = c.rgb * max( 1 - m.b - m.r, 0 ) + ( lerp( _ColorA1, _ColorA2, n2 ) * m.b + lerp( _ColorB1, _ColorB2, n2 ) * m.r ) * 0.8;
				c.rgb *=  _Ambient * _AmbientIntensity + _CharDiffuse * i.dfs;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.a = _Opaque * ( m.g + r * r * 0.5 );
				return c;
			}

			ENDCG
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Off
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
				half2 uv : TEXCOORD0;
				float4 uvFlow : TEXCOORD1;
				half3 normal : TEXCOORD2;
				half3 view : TEXCOORD3;
				half2 offset : TEXCOORD4;
			};
			
			sampler2D _Mask;
			sampler2D _Noise;
			half _NoiseScale;
			half _SpeedU;
			half _SpeedV;
			half _NoiseIntensity;
			half4 _ColorA1;
			half4 _ColorA2;
			half4 _ColorB1;
			half4 _ColorB2;
			half _EmissiveIntensityA;
			half _EmissiveIntensityB;
			half _EmissiveOffset;
			half _EmissivePow;
			float _Frequency;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4 p = v.vertex;
				half of = frac( _Time.y * _Frequency - ( v.texcoord.x + v.texcoord.y ) * 512 );
				p.xyz += v.normal * _EmissiveOffset * ( of + 0.25 );
				o.pos = UnityObjectToClipPos( p );
				o.uv = v.texcoord.xy;
				o.uvFlow.xy = o.uv * _NoiseScale + float2(_SpeedV, -_SpeedU ) * _Time.y;
				o.uvFlow.zw = o.uv * _NoiseScale + float2(_SpeedU, _SpeedV ) * _Time.y;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.offset.x = ( of + 1 ) * _NoiseIntensity;
				o.offset.y = 2 - abs( of * 4 - 2 );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 m = tex2D( _Mask, i.uv );
				half n1 = tex2D( _Noise, i.uvFlow.zw ).g;
				half n2 = tex2D( _Noise, i.uvFlow.xy + n1 * i.offset.x ).g;
				half r = pow( abs( dot( normalize( i.normal ), normalize( i.view ) ) ), _EmissivePow );
				c.rgb = ( lerp( _ColorA1, _ColorA2, n2 ) * m.b * _EmissiveIntensityA + lerp( _ColorB1, _ColorB2, n2 )
							* m.r * _EmissiveIntensityB ) * r * n1 * i.offset.y;
				c.rgb *= _Opaque;
				c.a = 1;
				return c;
			}

			ENDCG
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Off
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
				half2 uv : TEXCOORD0;
				float4 uvFlow : TEXCOORD1;
				half3 normal : TEXCOORD2;
				half3 view : TEXCOORD3;
				half2 offset : TEXCOORD4;
			};
			
			sampler2D _Mask;
			sampler2D _Noise;
			half _NoiseScale;
			half _SpeedU;
			half _SpeedV;
			half _NoiseIntensity;
			half4 _ColorA1;
			half4 _ColorA2;
			half4 _ColorB1;
			half4 _ColorB2;
			half _EmissiveIntensityA;
			half _EmissiveIntensityB;
			half _EmissiveOffset;
			half _EmissivePow;
			float _Frequency;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4 p = v.vertex;
				half of = frac( _Time.y * _Frequency - 0.5 - ( v.texcoord.x + v.texcoord.y ) * 512 );
				p.xyz += v.normal * _EmissiveOffset * ( of + 0.25 );
				o.pos = UnityObjectToClipPos( p );
				o.uv = v.texcoord.xy;
				o.uvFlow.xy = o.uv * _NoiseScale + float2(_SpeedV, -_SpeedU ) * _Time.y;
				o.uvFlow.zw = o.uv * _NoiseScale + float2(_SpeedU, _SpeedV ) * _Time.y;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.offset.x = ( of + 1 ) * _NoiseIntensity;
				o.offset.y = 2 - abs( of * 4 - 2 );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 m = tex2D( _Mask, i.uv );
				half n1 = tex2D( _Noise, i.uvFlow.zw ).g;
				half n2 = tex2D( _Noise, i.uvFlow.xy + n1 * i.offset.x ).g;
				half r = pow( abs( dot( normalize( i.normal ), normalize( i.view ) ) ), _EmissivePow );
				c.rgb = ( lerp( _ColorA1, _ColorA2, n2 ) * m.b * _EmissiveIntensityA + lerp( _ColorB1, _ColorB2, n2 )
							* m.r * _EmissiveIntensityB ) * r * n1 * i.offset.y;
				c.rgb *= _Opaque;
				c.a = 1;
				return c;
			}

			ENDCG
		}

	}
	
	Fallback "KYU3D/Character/Boss"
}
