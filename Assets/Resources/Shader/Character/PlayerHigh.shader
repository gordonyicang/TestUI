
Shader "KYU3D/Character/PlayerHigh"
{
	Properties
	{
		[NoScaleOffset]_MainTex( "_MainTex", 2D ) = "gray" {}
		[NoScaleOffset]_Mask( "_Mask Specular(G)Reflect(R)Skin(B)", 2D ) = "gray" {}
		[Space( 20 )]
		_FillColor( "_FillColor", Color ) = ( 0.6, 0.7, 0.8, 1 )
		_DiffuseWeaken( "_DiffuseWeaken", Range( 0, 1 ) ) = 0.4
		[Space( 20 )]
		_RimColor( "_RimColor", Color ) = ( 0.6, 0.8, 0.9, 1 )
		_RimIntensity( "_RimIntensity", float ) = 1.5
		_RimRight( "_RimRight", Range( -1, 1 ) ) = 0.65
		_RimUp( "_RimUp", Range( -1, 1 ) ) = 0.45
		[Space( 20 )]
		_Shininess( "_Shininess", float ) = 10
		_SpecularColorRatio( "_SpecularColorRatio", Range( 0, 1 ) ) = 0
		_SpecularColor( "_SpecularColor", Color ) = ( 1.0, 0.5, 0.0, 1 )
		[Space( 20 )]
		_SkinSoft( "_SkinSoft", float ) = 0.5
		[Space( 20 )]
		[NoScaleOffset]_Reflect( "_Reflect", CUBE ) = "" {}
		_ReflectIntensity( "_ReflectIntensity", float ) = 0.5
	}

	SubShader
	{
		LOD 400

		Tags
		{
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			// Fog {Mode Off}

			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 view : TEXCOORD2;
				float3 rim : TEXCOORD3;
			};
			
			sampler2D _MainTex;
			sampler2D _Mask;
			float3 _FillColor;
			float _DiffuseWeaken;
			float _SpecularColorRatio;
			float _SkinSoft;
			float3 _RimColor;
			float _RimIntensity;
			float _RimRight;
			float _RimUp;
			float3 _SpecularColor;
			samplerCUBE _Reflect;
			float _ReflectIntensity;
			
			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize( mul( v.normal, (float3x3)unity_WorldToObject ) );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.rim = mul( normalize( float3( _RimRight, _RimUp, -1 ) ), (float3x3)UNITY_MATRIX_V );
				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				float4 c;
				float4 t = tex2D( _MainTex, i.uv );
				float4 m = tex2D( _Mask, i.uv );
				float3 n = normalize( i.normal );
				float3 v = normalize( i.view );
				float vn = dot( v, n );
				float r = 1 - max( vn, 0 );
				float nr = max( 0, dot( n, i.rim ) );
				float4 e = texCUBE( _Reflect, vn * n * 2 - v );
				float3 sc = ( ( _SpecularColor - t.rgb ) * _SpecularColorRatio + t.rgb );
				float rl =  _ReflectIntensity * r * r * m.r;
				float3 rc = _RimColor * _RimIntensity;
				float3 ro = 2 * ( t.rgb + rc - rc * t.rgb - 0.5 );
				
				c.rgb = ( _LightColor0.rgb - _FillColor ) * saturate( dot( n, _WorldSpaceLightPos0.xyz ) + 0.1 ) + _FillColor;
				c.rgb += m.b > 0.8 ? _SkinSoft - _SkinSoft * c.rgb : 0;
				c.rgb *= t.rgb - sc * m.g * _DiffuseWeaken;
				c.rgb += ( ro - c.rgb ) * nr;
				c.rgb += min( e.rgb * rl, 0.5 );
				c.a = t.a;
				
				return c;
			}

			ENDCG
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardAdd"
			}
			// Fog {Mode Off}
			ZTest LEqual
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_fwdadd
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 view : TEXCOORD2;
				float3 light : TEXCOORD3;
			};
			
			sampler2D _MainTex;
			sampler2D _Mask;
			float3 _FillColor;
			float _DiffuseWeaken;
			float _Shininess;
			float3 _SpecularColor;
			float _SpecularColorRatio;
			float _SkinSoft;
			
			v2f vert( appdata_full v )
			{
				v2f o;
				float3 wp = mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize( mul( v.normal, (float3x3)unity_WorldToObject ) );
				o.view = _WorldSpaceCameraPos - wp;
				o.light = _WorldSpaceLightPos0.xyz - wp;
				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				float4 c;
				float4 t = tex2D( _MainTex, i.uv );
				float4 m = tex2D( _Mask, i.uv );
				float3 n = normalize( i.normal );
				float3 v = normalize( i.view );
				float3 f = dot( v, n ) * n * 2 - v;
				float3 l = normalize( i.light );
				float s = 2 * pow( max( dot( l, f ), 0 ), m.r * _Shininess + 0.5 ) * m.g / ( 0.5 + dot( i.light, i.light ) );
				c.rgb = _LightColor0.rgb * ( _SpecularColorRatio * ( _SpecularColor - t.rgb ) + t.rgb ) * s;
				c.a = t.a;
				
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Character/Player"
}
