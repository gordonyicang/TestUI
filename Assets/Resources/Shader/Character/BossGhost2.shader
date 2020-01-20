// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "KYU3D/Character/BossGhost2"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1

		_Mask( "Opaque(G), Flow1(B), Flow2(R)", 2D ) = "gray" {}
		
		_FlowTex1( "_FlowTex1", 2D ) = "grey" {}
		_FlowTexScale1( "_FlowTexScale1", float ) = 3
		_Emissive1( "_Emissive1", Color ) = ( 0.0, 0.0, 1.0, 1.0 )
		_EmissiveFlowIntensity1( "_EmissiveFlowIntensity1", float ) = 1
		_FlowSpeedU1( "_FlowSpeedU1", float ) = -0.2
		_FlowSpeedV1( "_FlowSpeedV1", float ) = 0.0

		_FlowTex2( "_FlowTex2", 2D ) = "grey" {}
		_FlowTexScale2( "_FlowTexScale2", float ) = 1.5
		_Emissive2( "_Emissive2", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_EmissiveFlowIntensity2( "_EmissiveFlowIntensity2", float ) = 1
		_FlowSpeedU2( "_FlowSpeedU2", float ) = 0.0
		_FlowSpeedV2( "_FlowSpeedV2", float ) = 0.2

		_EmissiveOffset( "_EmissiveOffset", float ) = 0.15
		_EmissivePow( "_EmissivePow", float ) = 2
		
		_Noise( "_Noise", 2D ) = "gray" {}
		_NoiseScale( "_NoiseScale", float ) = 2
		_NoiseIntensity( "_NoiseIntensity", float ) = 0.2
		_NoiseSpeedU( "_NoiseSpeedU", float ) = 0.2
		_NoiseSpeedV( "_NoiseSpeedV", float ) = 0.2
		
		_RimColor( "_RimColor", Color ) = ( 0.3, 0.7, 1, 1 )
		_RimIntensity( "_RimIntensity", float ) = 1.5

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
				float2 uv : TEXCOORD0;
				float4 uvFlow : TEXCOORD1;
				float2 uvNoise : TEXCOORD2;
				half3 normal : TEXCOORD3;
				half3 view : TEXCOORD4;
				UNITY_FOG_COORDS(5)
				half fogParam:TEXCOORD6;
			};
			
			sampler2D _MainTex;
			half _AmbientIntensity;
			sampler2D _Mask;
			half3 _Emissive1;
			sampler2D _FlowTex1;
			half _FlowTexScale1;
			half _FlowSpeedU1;
			half _FlowSpeedV1;
			half _EmissiveFlowIntensity1;
			half3 _Emissive2;
			sampler2D _FlowTex2;
			half _FlowTexScale2;
			half _FlowSpeedU2;
			half _FlowSpeedV2;
			half _EmissiveFlowIntensity2;
			sampler2D _Noise;
			half _NoiseScale;
			half _NoiseIntensity;
			half _NoiseSpeedU;
			half _NoiseSpeedV;
			half3 _RimColor;
			half _RimIntensity;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.uvFlow.xy = o.uv * _FlowTexScale1 + float2( _FlowSpeedU1, _FlowSpeedV1 ) * _Time.y;
				o.uvFlow.zw = o.uv * _FlowTexScale2 + float2( _FlowSpeedU2, _FlowSpeedV2 ) * _Time.y;
				o.uvNoise = o.uv * _NoiseScale + float2(_NoiseSpeedU, -_NoiseSpeedV ) * _Time.y;
				UNITY_TRANSFER_FOG(o,o.pos);
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 m = tex2D( _Mask, i.uv );
				half n = ( tex2D( _Noise, i.uvNoise ).g - 0.5 ) * _NoiseIntensity * 0.5;
				half4 f1 = tex2D( _FlowTex1, i.uvFlow.xy - n );
				half4 f2 = tex2D( _FlowTex2, i.uvFlow.zw + n );
				half r = 1 - dot( normalize( i.normal ), normalize( i.view ) );
				c.rgb = c.rgb * _Ambient * _AmbientIntensity + _Emissive1 * f1.rgb * _EmissiveFlowIntensity1 * m.b +
							_Emissive2 * f2.rgb * _EmissiveFlowIntensity2 * m.r + _RimColor * _RimIntensity * r * ( 1 - m.g );
				c.a = _Opaque * ( m.g + r * _RimIntensity );
				UNITY_APPLY_FOG(i.fogCoord, c);
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
				float2 uv : TEXCOORD0;
				float4 uvFlow : TEXCOORD1;
				float2 uvNoise : TEXCOORD2;
				half3 normal : TEXCOORD3;
				half3 view : TEXCOORD4;
			};
			
			sampler2D _Mask;
			half3 _Emissive1;
			sampler2D _FlowTex1;
			half _FlowTexScale1;
			half _FlowSpeedU1;
			half _FlowSpeedV1;
			half _EmissiveFlowIntensity1;
			half3 _Emissive2;
			sampler2D _FlowTex2;
			half _FlowTexScale2;
			half _FlowSpeedU2;
			half _FlowSpeedV2;
			half _EmissiveFlowIntensity2;
			sampler2D _Noise;
			half _NoiseScale;
			half _NoiseIntensity;
			half _NoiseSpeedU;
			half _NoiseSpeedV;
			half _EmissiveOffset;
			half _EmissivePow;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4 p = v.vertex;
				p.xyz += v.normal * _EmissiveOffset;
				o.pos = UnityObjectToClipPos( p );
				o.uv = v.texcoord.xy;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.uvFlow.xy = o.uv * _FlowTexScale1 + float2( _FlowSpeedU1, _FlowSpeedV1 ) * _Time.y;
				o.uvFlow.zw = o.uv * _FlowTexScale2 + float2( _FlowSpeedU2, _FlowSpeedV2 ) * _Time.y;
				o.uvNoise = o.uv * _NoiseScale + float2(_NoiseSpeedV, _NoiseSpeedU ) * _Time.y;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 m = tex2D( _Mask, i.uv );
				half n = ( tex2D( _Noise, i.uvNoise ).g - 0.5 ) * _NoiseIntensity;
				half4 f1 = tex2D( _FlowTex1, i.uvFlow.xy + n );
				half4 f2 = tex2D( _FlowTex2, i.uvFlow.zw - n );
				half r = pow( abs( dot( normalize( i.normal ), normalize( i.view ) ) ), _EmissivePow );
				c.rgb = ( _Emissive1 * f1.rgb * _EmissiveFlowIntensity1 * m.b + _Emissive2 * f2.rgb * _EmissiveFlowIntensity2 * m.r ) * r * _Opaque;
				c.a = 1;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Character/Boss"
}
