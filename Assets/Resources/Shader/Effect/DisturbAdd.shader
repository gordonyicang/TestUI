
Shader "KYU3D/Effect/DisturbAdd"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_Color( "_Color", Color ) = ( 1.0, 0.8, 0, 1 )
		_EmissiveIntensity( "_EmissiveIntensity", float ) = 1
		_MainDisturb( "_MainDisturb", float ) = 0.05
		_Disturb( "_Disturb", 2D ) = "gray" {}
		_DisturbScale( "_DisturbScale", float ) = 4
		_DisturbSpeedU( "_DisturbSpeedU", float ) = 0
		_DisturbSpeedV( "_DisturbSpeedV", float ) = 0.3
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
			// Fog {Mode Off}
			Cull Off
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha One

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
				fixed4 color : TEXCOORD1;
				float4 uvDst : TEXCOORD2;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			half4 _Color;
			half _EmissiveIntensity;
			half _MainDisturb;
			sampler2D _Disturb;
			float _DisturbScale;
			float _DisturbSpeedU;
			float _DisturbSpeedV;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.color = v.color * _Color * _EmissiveIntensity;
				o.uvDst.xy = o.uv * _DisturbScale - float2( _DisturbSpeedU, _DisturbSpeedV ) * _Time.y;
				o.uvDst.zw = o.uv * _DisturbScale + float2( _DisturbSpeedU, _DisturbSpeedV ) * _Time.y;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half d1 = tex2D( _Disturb, i.uvDst.xy ).g;
				half4 c = tex2D( _MainTex, i.uv + ( d1 - 0.5 ) * _MainDisturb );
				c *= i.color;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Effect/Add"
}
