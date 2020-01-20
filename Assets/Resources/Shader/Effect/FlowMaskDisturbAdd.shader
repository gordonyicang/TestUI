
Shader "KYU3D/Effect/FlowMaskDisturbAdd" 
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_FlowSpeedU1( "_FlowSpeedU1", Float ) = 0
		_FlowSpeedV1( "_FlowSpeedV1", Float ) = 0.2
		_MaskTex( "_MaskTex", 2D ) = "gray" {}
		_FlowSpeedU2( "_FlowSpeedU2", Float ) = 0
		_FlowSpeedV2( "_FlowSpeedV2", Float ) = 0.2
		_Color( "_Color", Color ) = ( 1.0, 0.8, 0, 1 )
		_EmissiveIntensity( "_EmissiveIntensity", Float ) = 1
		_MainDisturb( "_MainDisturb", Float ) = 0.03
		_Disturb( "_Disturb", 2D ) = "gray" {}
		_DisturbScale( "_DisturbScale", Float ) = 4
		_DisturbSpeedU( "_DisturbSpeedU", Float ) = 0.5
		_DisturbSpeedV( "_DisturbSpeedV", Float ) = 0		
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
			Cull Off 
			ZTest LEqual
			ZWrite Off 
			Blend SrcAlpha One

			CGPROGRAM 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include"../KyUnity.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
				fixed4 color : TEXCOORD1;
				float2 uv_Disturb : TEXCOORD2;
				float2 uv_MaskTex : TEXCOORD3;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			float _FlowSpeedU1;
			float _FlowSpeedV1;
			sampler2D _MaskTex;
			half4 _MaskTex_ST;
			float _FlowSpeedU2;
			float _FlowSpeedV2;
			fixed4 _Color;
			half _EmissiveIntensity;
			half _MainDisturb;
			sampler2D _Disturb;
			float _DisturbSpeedU;
			float _DisturbSpeedV;
			float _DisturbScale;			

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv_MainTex = TRANSFORM_TEX( v.texcoord.xy, _MainTex );
				o.uv_MainTex += float2( _FlowSpeedU1, _FlowSpeedV1 ) * _Time.y;
				o.uv_MaskTex = TRANSFORM_TEX( v.texcoord.xy, _MaskTex );
				o.uv_MaskTex += float2( _FlowSpeedU2, _FlowSpeedV2 ) * _Time.y;
				o.color = v.color * _Color * _EmissiveIntensity;
				o.uv_Disturb = v.texcoord1.xy * _DisturbScale - float2( _DisturbSpeedU, _DisturbSpeedV ) * _Time.y;
				return o;
			}

			fixed4 frag( v2f i ) : SV_Target
			{
				fixed disturb = tex2D( _Disturb, i.uv_Disturb ).g;
				fixed4 c = tex2D( _MainTex, i.uv_MainTex + (disturb - 0.5) * _MainDisturb ) * tex2D( _MaskTex, i.uv_MaskTex ).a;
				c *= i.color;
				return c;
			}
			ENDCG 
		}
	}
	
	FallBack "KYU3D/Effect/DisturbAdd"
}
