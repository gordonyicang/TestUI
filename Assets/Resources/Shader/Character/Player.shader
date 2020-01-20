﻿Shader "KYU3D/Character/Player"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = -1
		_LightY( "_LightY", Range( -1, 1 ) ) = 1
		_LightZ( "_LightZ", Range( -1, 1 ) ) = 0

		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1
		_DiffuseIntensity( "_DiffuseIntensity", float ) = 1

		_SpecularTex( "_SpecularTex(G)", 2D ) = "white" {}
		_Specular( "_Specular", Color ) = ( 1, 1, 1, 1 )
		_SpecularColorRatio( "_SpecularColorRatio", Range( 0, 1 ) ) = 0.5
		_SpecularIntensity( "_SpecularIntensity", float ) = 2.5
		_Shininess( "_Shininess", float ) = 15

		_RimColor( "_RimColor", Color ) = ( 0.2, 0.6, 1, 1 )
		_RimIntensity( "_RimIntensity", Float ) = 3

		_Alpha("_Alpha",Range(0.1,1)) = 1
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Geometry+60"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// Fog {Mode Off}
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
				half2 light : TEXCOORD1;
				half3 view : TEXCOORD2;
				half3 normal : TEXCOORD3;
				half3 rimLight : TEXCOORD4;
				UNITY_FOG_COORDS(5)
			};

			half _LightX;
			half _LightY;
			half _LightZ;
			sampler2D _MainTex;
			half _AmbientIntensity;
			half _DiffuseIntensity;
			sampler2D _SpecularTex;
			half3 _Specular;
			half _SpecularColorRatio;
			half _SpecularIntensity;
			half _Shininess;
			half3 _RimColor;
			half _RimIntensity;
			half _Alpha;
			
			v2f vert( appdata_full v )
			{
				v2f o;
				half3 l = normalize( half3( _LightX, _LightY, _LightZ ) );
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize(UnityObjectToWorldNormal(v.normal));
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.light.x = max( dot( l, o.normal ), 0 ) * _DiffuseIntensity;
				o.light.y = pow( max( dot( normalize( l + o.view ), o.normal ), 0 ), _Shininess ) * _SpecularIntensity;
				o.rimLight = normalize( dot( l, UNITY_MATRIX_V[2].xyz ) * UNITY_MATRIX_V[2].xyz + l );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 frag( v2f i ) : COLOR
			{
				half4 tex = tex2D( _MainTex, i.uv );
				half4 st = tex2D( _SpecularTex, i.uv );
				fixed4 c;
				c.rgb = tex.rgb * ( _Ambient * _AmbientIntensity + _CharDiffuse * i.light.x )
						+ _CharSpecular * lerp( _Specular, tex.rgb, _SpecularColorRatio) * st.g * i.light.y;
				half r = 1 - dot( i.view, i.normal );
				r *= r;
				r *= r;
				r *= max( dot( i.rimLight, i.normal ), 0 ) * _RimIntensity;
				half3 rc = 1 - ( 1 - c.rgb ) * ( 1 - _RimColor );
				c.rgb = lerp(c.rgb, rc, r);
				UNITY_APPLY_FOG(i.fogCoord, c);
				// c.a = tex.a;
				c.a = _Alpha;
				return c;
			}
			ENDCG
		}
	}
	Fallback "KYU3D/Simple/Texture"
}
