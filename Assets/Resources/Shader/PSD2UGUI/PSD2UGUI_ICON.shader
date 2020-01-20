// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PSD2UGUI/PSD2UGUI ICON ETC"
 {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Alpha Texture", 2D) = "white" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
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

			fixed3 Multiply(in fixed3 cB, in fixed3 cS) { return cB * cS; }
			fixed3 Screen(in fixed3 cB, in fixed3 cS) { return 1.0 - (1.0 - cB) * (1.0 - cS); }
			fixed3 HardLight(in fixed3 cB, in fixed3 cS) { return lerp( Multiply( cB, 2.0 * cS ), Screen( cB, 2.0 * cS - 1 ), step(0.5, cS)); }
			fixed3 Overlay(in fixed3 cB, in fixed3 cS) 
			{ 
				return HardLight(cS, cB); 
			}

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1: TEXCOORD1;
			};

			struct V2F
			{
				float4 pos:SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv:TEXCOORD0;
				half2 extra		: TEXCOORD1;
			};

			V2F vert(appdata_t IN)
			{
				V2F o;
				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
				o.color = IN.color;
				o.extra = IN.texcoord1;
				return o;
			}

			fixed4 frag(V2F IN):COLOR
			{
				fixed4 color = tex2D(_MainTex, IN.uv);
				fixed4 overlayColor = saturate(IN.color);
				fixed3 overlayed = Overlay(color.rgb, overlayColor.rgb);
				color.rgb = lerp(color.rgb, overlayed, overlayColor.a);
				color.a *= tex2D(_AlphaTex,IN.uv).r * (1 - IN.extra.y);
				float grey = dot(color.rgb, float3(0.22, 0.707, 0.071));
				color.rgb = lerp(color.rgb, float3(grey, grey, grey), IN.extra.x);
				clip (color.a - 0.01);
				return color;
			}

			ENDCG
		}

	} 
	
}
