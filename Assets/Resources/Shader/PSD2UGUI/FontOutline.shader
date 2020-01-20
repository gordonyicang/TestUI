
Shader "KYU3D/UI/FontOutline2"
{
    Properties
	{
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
		_OutlineColor( "_OutlineColor", Color ) = ( 0, 0, 0, 1 )
		_Offset( "_Offset", float ) = 0.0016

        _OffsetX( "_OffsetX", Range( -1, 1 ) ) = -1
        _OffsetY( "_OffsetY", Range( -1, 1 ) ) = -1
        _OffsetZ( "_OffsetZ", Range( -1, 1 ) ) = -1
    }

    SubShader
	{
        Tags
		{
            "Queue"="Geometry-50"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Lighting Off
		Cull Off
		ZTest Always
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;
            uniform fixed3 _OutlineColor;
            uniform half _Offset;

            half _OffsetX;
            half _OffsetY;
            half _OffsetZ;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float4 pos = v.vertex;
                pos.xyz += half3(_OffsetX,_OffsetY,_OffsetZ);
                o.vertex = UnityObjectToClipPos(pos);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 c ;
                c.a = tex2D( _MainTex, i.texcoord ).a;
                c.rgb = _OutlineColor;
                return c;
            }

            ENDCG
        }

        Pass
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"

            struct appdata_t
			{
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
			{
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;
			uniform fixed3 _OutlineColor;
			uniform half _Offset;

            half _OffsetX;
            half _OffsetY;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 c = i.color;
    //             half2 offset = half2(_OffsetX/100,_OffsetY/100);
				// half o = tex2D( _MainTex, i.texcoord + offset ).a;
                c.a = tex2D( _MainTex, i.texcoord ).a;
				c.rgb = i.color.rgb;
				// c.a += o;
                return c;
            }

            ENDCG
        }

        
    }

    Fallback "GUI/Text Shader"
}
