
Shader "KYU3D/Character/WeaponFlowEnvi"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = 0
		_LightY( "_LightY", Range( -1, 1 ) ) = 0.7
		_LightZ( "_LightZ", Range( -1, 1 ) ) = 1

		_MainTex( "_MainTex", 2D ) = "gray" {}
		_AmbientIntensity( "_AmbientIntensity", float ) = 1
		_DiffuseIntensity( "_DiffuseIntensity", float ) = 1

		_Mask( "Specular(G), Flow(B), Reflect(R)", 2D ) = "white" {}

		_SpecularIntensity( "_SpecularIntensity", float ) = 5
		_Shininess( "_Shininess", float ) = 30

		_FlowTex1( "_FlowTex1", 2D ) = "white" {}
		_FlowTexScale1( "_FlowTexScale1", float ) = 4
		_Emissive1( "_Emissive1", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_EmissiveFlowIntensity1( "_EmissiveFlowIntensity1", float ) = 1
		_FlowSpeedU1( "_FlowSpeedU1", float ) = 0
		_FlowSpeedV1( "_FlowSpeedV1", float ) = 0.5

		_Reflect( "_Reflect", CUBE ) = "" {}
		_ReflectColor( "_ReflectColor", Color ) = ( 1, 1, 1, 1 )
		_ReflectIntensity( "_ReflectIntensity", float ) = 1
		_Deflect( "_Deflect", float ) = 0.4
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Geometry+75"
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
				half3 normal2 : TEXCOORD3;
				half3 view : TEXCOORD4;
				half2 uvFlow : TEXCOORD5;
				UNITY_FOG_COORDS(6)
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
			fixed3 _Emissive1;
			sampler2D _FlowTex1;
			half _FlowTexScale1;
			half _FlowSpeedU1;
			half _FlowSpeedV1;
			half _EmissiveFlowIntensity1;
			samplerCUBE _Reflect;
			half3 _ReflectColor;
			half _ReflectIntensity;
			half _Deflect;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize( mul( v.normal, (half3x3)unity_WorldToObject ) );
				float a = _Deflect * _Time.y;
				float s = sin( a );
				float c = cos( a );
				o.normal2.x = o.normal.x * c - o.normal.z * s;
				o.normal2.y = o.normal.y;
				o.normal2.z = o.normal.z * c + o.normal.x * s;
				o.light.xyz = normalize( half3( _LightX, _LightY, _LightZ ) );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.light.w = max( dot( o.light.xyz, o.normal ), 0 ) * _DiffuseIntensity;
				o.uvFlow.xy = o.uv * _FlowTexScale1 + half2( _FlowSpeedU1, _FlowSpeedV1 ) * _Time.y;
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half4 m = tex2D( _Mask, i.uv );
				half4 f = tex2D( _FlowTex1, i.uvFlow );
				half3 v = normalize( i.view );
				half3 n = normalize( i.normal2 );
				half3 r = dot( v, n ) * 2 * n - v;
				half4 env = texCUBE( _Reflect, r );
				half s = pow( max( 0, dot( normalize( i.normal ), normalize( v + i.light.xyz ) ) ), _Shininess ) * _SpecularIntensity;
				c.rgb *= _Ambient * _AmbientIntensity + i.light.w * _CharDiffuse + m.g * s * _CharSpecular;
				c.rgb += _Emissive1 * f.rgb * _EmissiveFlowIntensity1 * m.b + env * m.r * _ReflectIntensity * _ReflectColor;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Character/Weapon"
}
