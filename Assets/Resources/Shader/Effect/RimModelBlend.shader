
Shader "KYU3D/Effect/RimModelBlend"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_MainColor( "_MainColor", Color ) = ( 1, 1, 1, 1 )
		_RimPow( "_RimPow", float ) = 3
		_RimColor( "_RimColor", Color ) = ( 1, 1, 1, 1 )
		_RimIntensity( "_RimIntensity", float ) = 2
		_Opaque( "_Opaque", Range( 0, 1 ) ) = 0.5
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// Fog {Mode Off}
			Cull Back
			ZTest LEqual
			ZWrite Off
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
				half3 normal : TEXCOORD1;
				half3 view : TEXCOORD2;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			half3 _MainColor;
			half _RimPow;
			half3 _RimColor;
			half _RimIntensity;
			half _Opaque;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.normal = normalize(UnityObjectToWorldNormal(v.normal));
				o.view = normalize( _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half r = pow( 1 - dot( i.normal, i.view ), _RimPow );
				c.rgb = lerp( c.rgb * _MainColor, _RimColor * _RimIntensity, r );
				c.a *= _Opaque * ( 0.5 + r );
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Effect/Blend"
}
