
Shader "KYU3D/UI/IconEffectAdd"
{
    Properties
    {
		_MainTex ("Sprite Texture", 2D) = "white" {}

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

		_Row( "_Row", float ) = 4
		_Col( "_Col", float ) = 4
		_FrameTime( "_FrameTime", float ) = 0.1
		_Intensity( "_Intensity", float ) = 1
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
        Blend One One
        ColorMask [_ColorMask]

        Pass
        {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };
			
            sampler2D _MainTex;
            float4 _ClipRect;
			float _Row;
			float _Col;
			float _FrameTime;
			float _Intensity;

            v2f vert( appdata_t v )
            {
                v2f o;
                o.vertex = UnityObjectToClipPos( v.vertex );
				float f = round( fmod( floor( _Time.y / _FrameTime ), _Row * _Col ) );
				float r = floor( f / _Col );
				float c = fmod( f, _Col );
				o.uv.x = ( c + v.texcoord.x ) / _Col;
				o.uv.y = ( r + v.texcoord.y ) / _Row;
                return o;
            }

            fixed4 frag( v2f i ) : SV_Target
            {
				fixed4 c = tex2D( _MainTex, i.uv );
				c.rgb *= _Intensity;
				return c;
            }

			ENDCG
        }
    }

    Fallback "UI/Default"
}
