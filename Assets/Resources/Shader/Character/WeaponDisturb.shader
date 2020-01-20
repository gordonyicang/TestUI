
Shader "KYU3D/Character/WeaponDisturb"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = -1
		_LightY( "_LightY", Range( -1, 1 ) ) = 1
		_LightZ( "_LightZ", Range( -1, 1 ) ) = 0

		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1
		_DiffuseIntensity( "_DiffuseIntensity", float ) = 1

		_Mask( "Specular(G), Disturb(B)", 2D ) = "white" {}
		_SpecularIntensity( "_SpecularIntensity", float ) = 3
		_Shininess( "_Shininess", float ) = 20
		
		_Disturb( "_Disturb", 2D ) = "gray" {}
		_DisturbScale( "_DisturbScale", float ) = 6
		_DisturbIntensity( "_DisturbIntensity", float ) = 0.1
		_DisturbSpeedU( "_DisturbSpeedU", float ) = 0
		_DisturbSpeedV( "_DisturbSpeedV", float ) = 0.1
		
		_Color1( "_Color1", Color ) = ( 1.0, 0.2, 0, 1 )
		_Color2( "_Color2", Color ) = ( 0.8, 0.7, 0, 1 )
		_EmissiveIntensity( "_EmissiveIntensity", float ) = 1

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
				half4 light : TEXCOORD1;
				half3 normal : TEXCOORD2;
				float4 uvDst : TEXCOORD3;
				UNITY_FOG_COORDS(4)
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
			sampler2D _Disturb;
			float _DisturbScale;
			float _DisturbIntensity;
			float _DisturbSpeedU;
			float _DisturbSpeedV;
			half3 _Color1;
			half3 _Color2;
			half _EmissiveIntensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				half3 l = normalize( half3( _LightX, _LightY, _LightZ ) );
				half3 e = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.light.xyz = normalize( e + l );
				o.normal = normalize( mul( v.normal, (half3x3)unity_WorldToObject ) );
				o.light.w = max( dot( l, o.normal ), 0 ) * _DiffuseIntensity;
				o.uvDst.xy = o.uv * _DisturbScale - float2( _DisturbSpeedU, _DisturbSpeedV ) * _Time.y;
				o.uvDst.zw = o.uv * _DisturbScale + float2( _DisturbSpeedU, _DisturbSpeedV ) * _Time.y;
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 m = tex2D( _Mask, i.uv );
				half d1 = tex2D( _Disturb, i.uvDst.xy ).g;
				half d2 = tex2D( _Disturb, i.uvDst.zw + d1 * _DisturbIntensity ).g;
				half4 c = tex2D( _MainTex, i.uv );
				half s = pow( max( 0, dot( i.normal, normalize( i.light.xyz ) ) ), _Shininess ) * _SpecularIntensity;
				c.rgb *= _Ambient * _AmbientIntensity + i.light.w * _CharDiffuse + m.g * s * _CharSpecular;
				c.rgb += lerp( _Color1, _Color2, d2 ) * _EmissiveIntensity * m.b;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Character/Weapon"
}
