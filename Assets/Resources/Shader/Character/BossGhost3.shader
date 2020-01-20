
Shader "KYU3D/Character/BossGhost3"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1.5

		_Mask( "Opaque(G), Flow(R)", 2D ) = "gray" {}

		_FlowTex( "_FlowTex", 2D ) = "grey" {}
		_FlowTexScaleU( "_FlowTexScaleU", float ) = 2
		_FlowTexScaleV( "_FlowTexScaleV", float ) = 2
		_Emissive( "_Emissive", Color ) = ( 0.6, 0.8, 1.0, 1.0 )
		_EmissiveFlowIntensity( "_EmissiveFlowIntensity", float ) = 2
		_FlowSpeedU( "_FlowSpeedU", float ) = 0.2
		_FlowSpeedV( "_FlowSpeedV", float ) = 0.0

		_EmissiveOffset( "_EmissiveOffset", float ) = 0.15
		_EmissivePow( "_EmissivePow", float ) = 1.5
		
		_Noise( "_Noise", 2D ) = "gray" {}
		_NoiseScale( "_NoiseScale", float ) = 2
		_NoiseIntensity( "_NoiseIntensity", float ) = 0.2
		_NoiseSpeedU( "_NoiseSpeedU", float ) = 0.2
		_NoiseSpeedV( "_NoiseSpeedV", float ) = 0.2
		
		_RimColor( "_RimColor", Color ) = ( 0.3, 0.7, 1, 1 )
		_RimIntensity( "_RimIntensity", float ) = 1.5
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
				float2 uvFlow : TEXCOORD1;
				float2 uvNoise : TEXCOORD2;
				half3 normal : TEXCOORD3;
				half3 view : TEXCOORD4;
				UNITY_FOG_COORDS(5)
			};
			
			sampler2D _MainTex;
			half _AmbientIntensity;
			sampler2D _Mask;
			half3 _Emissive;
			sampler2D _FlowTex;
			half _FlowTexScaleU;
			half _FlowTexScaleV;
			half _FlowSpeedU;
			half _FlowSpeedV;
			half _EmissiveFlowIntensity;
			sampler2D _Noise;
			half _NoiseScale;
			half _NoiseIntensity;
			half _NoiseSpeedU;
			half _NoiseSpeedV;
			half3 _RimColor;
			half _RimIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.uvFlow = o.uv * float2( _FlowTexScaleU, _FlowTexScaleV ) - float2( _FlowSpeedU, _FlowSpeedV ) * _Time.y;
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
				half4 f = tex2D( _FlowTex, i.uvFlow + n );
				half r = 1 - dot( normalize( i.normal ), normalize( i.view ) );
				c.rgb = c.rgb * _Ambient * _AmbientIntensity + _Emissive * f.rgb * _EmissiveFlowIntensity * m.r + _RimColor * _RimIntensity * r * ( 1 - m.g );
				c.a = ( r * _RimIntensity + 1 ) * m.g;
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
				float2 uvFlow : TEXCOORD1;
				float2 uvNoise : TEXCOORD2;
				half3 normal : TEXCOORD3;
				half3 view : TEXCOORD4;
			};
			
			sampler2D _Mask;
			half3 _Emissive;
			sampler2D _FlowTex;
			half _FlowTexScaleU;
			half _FlowTexScaleV;
			half _FlowSpeedU;
			half _FlowSpeedV;
			half _EmissiveFlowIntensity;
			sampler2D _Noise;
			half _NoiseScale;
			half _NoiseIntensity;
			half _NoiseSpeedU;
			half _NoiseSpeedV;
			half _EmissiveOffset;
			half _EmissivePow;

			v2f vert( appdata_full v )
			{
				v2f o;
				float4 p = v.vertex;
				p.xyz += v.normal * _EmissiveOffset;
				o.pos = UnityObjectToClipPos( p );
				o.uv = v.texcoord.xy;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.uvFlow = o.uv * float2( _FlowTexScaleU, _FlowTexScaleV ) - float2( _FlowSpeedU, _FlowSpeedV ) * _Time.y;
				o.uvNoise = o.uv * _NoiseScale + float2(_NoiseSpeedV, _NoiseSpeedU ) * _Time.y;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 m = tex2D( _Mask, i.uv );
				half n = ( tex2D( _Noise, i.uvNoise ).g - 0.5 ) * _NoiseIntensity;
				half4 f = tex2D( _FlowTex, i.uvFlow - n );
				half r = pow( abs( dot( normalize( i.normal ), normalize( i.view ) ) ), _EmissivePow );
				c.rgb = _Emissive * f.rgb * _EmissiveFlowIntensity * m.r  * r;
				c.a = 1;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Character/Boss"
}
