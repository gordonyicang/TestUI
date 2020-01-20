// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PSD2UGUI/PSD2UGUI_SCROLL"
 {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Alpha Texture", 2D) = "white" {}
		_ScrollTex("ScrollTex", 2D) = "black" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		
		_ScrollU ("Scroll speed U", Float) = 1.0
		_ScrollV ("Scroll speed V", Float) = 0.0
	}

	SubShader 
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AlphaTex;
			sampler2D _ScrollTex;
			float4 _ScrollTex_ST;
	
			half _ScrollU;
			half _ScrollV;
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1: TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex	:	SV_POSITION;
				fixed4 color    :	COLOR;
				float2 texcoord	:	TEXCOORD0;
				half2 extra		: TEXCOORD1;
				half2 texcoord2	:	TEXCOORD2;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color			= IN.color;
				OUT.extra	= IN.texcoord1;
				OUT.texcoord2 = TRANSFORM_TEX(IN.texcoord, _ScrollTex) + half2(_ScrollU, _ScrollV) * _Time.y;
				
				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 color = tex2D(_MainTex, IN.texcoord);
				color.rgb += tex2D(_ScrollTex, IN.texcoord2).rgb * 2;
				color.a *= tex2D(_AlphaTex,IN.texcoord).r * (1 - IN.extra.y);
				return color;
			}

			ENDCG
		}
	}
	
}
