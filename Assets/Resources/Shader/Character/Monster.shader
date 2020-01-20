Shader "KYU3D/Character/Monster"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = -1
		_LightY( "_LightY", Range( -1, 1 ) ) = 1
		_LightZ( "_LightZ", Range( -1, 1 ) ) = 0

		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1
		_DiffuseIntensity( "_DiffuseIntensity", float ) = 1

		_Mask( "_Specular(G)", 2D ) = "white" {}
		_SpecularIntensity( "_SpecularIntensity", float ) = 2.5
		_Shininess( "_Shininess", float ) = 10
		_Alpha("_Alpha",Range(0.1,1)) = 1

	}
	
	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Geometry+50"
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
				UNITY_FOG_COORDS(2)
			};

			half _LightX;
			half _LightY;
			half _LightZ;
			sampler2D _MainTex;
			half _AmbientIntensity;
			half _DiffuseIntensity;
			sampler2D _Mask;
			half _SpecularIntensity;
			half _Shininess;
			half _Alpha;

			v2f vert( appdata_full v )
			{
				v2f o;
				half3 l = normalize( half3( _LightX, _LightY, _LightZ ) );
				half3 n = normalize( mul( v.normal, (half3x3)unity_WorldToObject ) );
				half3 e = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.light.x = max( dot( l, n ), 0 ) * _DiffuseIntensity;
				o.light.y = pow( max( dot( normalize( e + l ), n ), 0 ), _Shininess ) * _SpecularIntensity;
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 s = tex2D( _Mask, i.uv );
				c.rgb *= _Ambient * _AmbientIntensity + _CharDiffuse * i.light.x + _CharSpecular * s.g * i.light.y;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.a = _Alpha;
				return c;
			}

			ENDCG
		}
	}

	Fallback "KYU3D/Simple/Texture"
}
