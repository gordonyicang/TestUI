Shader "PSD2UGUI/PSD2UGUI ICON SPLIT ALPHA"
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
//Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AlphaTex;


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct V2F
			{
				float4 pos:SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv:TEXCOORD0;
			};

			V2F vert(appdata_t IN)
			{
				V2F o;
				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
				o.color = IN.color;
				return o;
			}

			fixed4 frag(V2F IN):COLOR
			{
				fixed4 color = tex2D(_MainTex, IN.uv);
				color.rgb *= IN.color.rgb;
				color.a = tex2D(_AlphaTex,IN.uv).r * IN.color.a;
				return color;
			}

			ENDCG
		}

	} 
	
}
