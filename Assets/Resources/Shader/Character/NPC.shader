Shader "KYU3D/Character/NPC"
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
		_SpecularIntensity( "_SpecularIntensity", float ) = 2
		_Shininess( "_Shininess", float ) = 10

		_RimColor( "_RimColor", Color ) = ( 0.4, 0.7, 1, 1 )
		_RimIntensity( "_RimIntensity", float ) = 2
		_RimRange( "_RimRange",  Range( 0, 10 ) ) = 2
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
				half3 normal : TEXCOORD2;
				half3 rimLight : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};

			half _LightX;
			half _LightY;
			half _LightZ;
			sampler2D _MainTex;
			half _AmbientIntensity;
			half _DiffuseIntensity;
			sampler2D _SpecularTex;
			half _SpecularIntensity;
			half _Shininess;
			half3 _RimColor;
			half _RimIntensity;
			half _RimRange;
			
			v2f vert( appdata_full v )
			{
				v2f o;
				half3 l = normalize( half3( _LightX, _LightY, _LightZ ) );
				half3 e = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.normal = normalize( mul( v.normal, (half3x3)unity_WorldToObject ) );
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.light.x = max( dot( l, o.normal ), 0 ) * _DiffuseIntensity;
				o.light.y = pow( max( dot( normalize( e + l ), o.normal ), 0 ), _Shininess ) * _SpecularIntensity;
				o.rimLight = normalize( UNITY_MATRIX_V[1].xyz * 0.5 - UNITY_MATRIX_V[2].xyz
								- sign( dot( l, UNITY_MATRIX_V[0] ) ) * UNITY_MATRIX_V[0].xyz * _RimRange );
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 s = tex2D( _SpecularTex, i.uv );
				half r = max( dot( i.rimLight, i.normal ), 0 ) * _RimIntensity;
				c.rgb *= _Ambient * _AmbientIntensity + _CharDiffuse * i.light.x + _CharSpecular * s.g * i.light.y;
				c.rgb += min( c.rgb * ( 2 - c.rgb ) * r * _RimColor, 1 );
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}

	Fallback "KYU3D/Simple/Texture"
}
