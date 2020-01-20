
Shader "KYU3D/Scene/WaterCube"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = 0.5
		_LightY( "_LightY", Range( -1, 1 ) ) = 1
		_LightZ( "_LightZ", Range( -1, 1 ) ) = -1
		_Specular( "_Specular", Color ) = ( 1, 1, 1, 1 )
		_Shininess( "_Shininess", float ) = 192
		_SpecularIntensity( "_SpecularIntensity", float ) = 0.75
		
		_Color1( "_Color1", Color ) = ( 0.1, 0.6, 0.3, 1 )
		_Color2( "_Color2", Color ) = ( 0.3, 0.7, 0.9, 1 )
		[HideInInspector]_ColorVaryK( "_ColorVaryK", float ) = 1
		[HideInInspector]_ColorVaryB( "_ColorVaryB", float ) = 0.2
		[HideInInspector]_TransparentDifference( "_TransparentDifference", float ) = 0.3
		_TransparentVaryK( "_TransparentVaryK", Range( 0.7, 1.1 ) ) = 0.8

		_Reflect( "_Reflect", CUBE ) = "" {}
		[HideInInspector]_ReflectVaryK( "_ReflectVaryK", float ) = 2.5
		[HideInInspector]_ReflectVaryB( "_ReflectVaryB", float ) = 0.25
		_FnlPow( "_FnlPow", float ) = 1.5
		
		_Flat( "_Flat", float ) = 5
		_Wave( "_Wave", 2D ) = "Bump" {}
		_ScaleX1( "_ScaleX1", float ) = 0.13
		_ScaleZ1( "_ScaleZ1", float ) = -0.13
		_SpeedX1( "_SpeedX1", float ) = 0.2
		_SpeedZ1( "_SpeedZ1", float ) = 0.6
		_ScaleX2( "_ScaleX2", float ) = -0.2
		_ScaleZ2( "_ScaleZ2", float ) = 0.2
		_SpeedX2( "_SpeedX2", float ) = -0.6
		_SpeedZ2( "_SpeedZ2", float ) = -0.2
		
		_LightmapWave( "_LightmapWave" , float ) = 0.01
		[HideInInspector]_LightmapVaryB( "_LightmapVaryB", float ) = 0.1
		[HideInInspector]_LightmapVaryK( "_LightmapVaryK", float ) = 0.75
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Transparent-20"
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
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float4 waveUV : TEXCOORD0;
				float3 view : TEXCOORD01;
				float3 light : TEXCOORD2;
				half depth : TEXCOORD3;
				half2 lmuv : TEXCOORD4;
				UNITY_FOG_COORDS(5)
			};

			half _LightX;
			half _LightY;
			half _LightZ;
			half3 _Specular;
			half _Shininess;
			half _SpecularIntensity;
			half3 _Color1;
			half3 _Color2;
			half _ColorVaryK;
			half _ColorVaryB;
			half _TransparentDifference;
			half _TransparentVaryK;
			samplerCUBE _Reflect;
			half _ReflectWave;
			half _ReflectVaryK;
			half _ReflectVaryB;
			half _FnlPow;
			half _LightmapVaryB;
			half _LightmapVaryK;
			half _LightmapWave;
			half _Flat;
			sampler2D _Wave;
			float _ScaleX1;
			float _ScaleZ1;
			float _SpeedX1;
			float _SpeedZ1;
			float _ScaleX2;
			float _ScaleZ2;
			float _SpeedX2;
			float _SpeedZ2;

			v2f vert( appdata_full v )
			{
				v2f o;

				float3 wp = mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.view = _WorldSpaceCameraPos - wp;
				o.light = normalize( float3( _LightX, _LightY, _LightZ ) );
				o.waveUV.x = ( _SpeedX1 * _Time.y - wp.x ) * _ScaleX1;
				o.waveUV.y = ( _SpeedZ1 * _Time.y - wp.z ) * _ScaleZ1;
				o.waveUV.z = ( _SpeedX2 * _Time.y - wp.x ) * _ScaleX2;
				o.waveUV.w = ( _SpeedZ2 * _Time.y - wp.z ) * _ScaleZ2;
				o.depth = v.color.a;
				o.lmuv = LightMapUV( v.texcoord1 );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);

				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 n1 = tex2D( _Wave, i.waveUV.xy );
				half4 n2 = tex2D( _Wave, i.waveUV.zw );
				half3 v = normalize( i.view );
				half3 n = normalize( ( n1.xyz + n2.xyz - 1 ) * 2 + half3( 0, 0, _Flat ) );
				n.xyz = n.yzx;
				half nv = dot( v, n );
				half3 lm = max( LightMapColor( i.lmuv + _LightmapWave * n.xz ) * _LightmapVaryK + _LightmapVaryB, 0 );
				half f = saturate( pow( ( 1 - nv ) * ( 1 - v.y ), _FnlPow ) * _ReflectVaryK + _ReflectVaryB );
				half3 wc = lerp( _Color2, _Color1, saturate( max( dot( i.light, n ), 0 ) * _ColorVaryK + _ColorVaryB ) ) * lm;
				half4 r = texCUBE( _Reflect, nv * n * 2 - v );
				half s = pow( max( dot( normalize( v + i.light ), n ), 0 ), _Shininess ) * _SpecularIntensity;

				half4 c;
				c.rgb = max( ( r.rgb - wc ) * f, 0 ) + wc + s * _Specular * lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.a = max( i.depth * _TransparentVaryK - nv * _TransparentDifference, s * i.depth );
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Scene/WaterTransparent"
}
