
Shader "KYU3D/UI/FontOutline"
{
    Properties
	{
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
		_OutlineColor( "_OutlineColor", Color ) = ( 0, 0, 0, 1 )
		_Offset( "_Offset", float ) = 0.0016
    }

    SubShader
	{
        Tags
		{
            "Queue"="Transparent"
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
				half o = tex2D( _MainTex, i.texcoord + _Offset ).a;
				o += tex2D( _MainTex, i.texcoord - _Offset ).a;
				o += tex2D( _MainTex, i.texcoord + half2( -_Offset, _Offset ) ).a;
				o += tex2D( _MainTex, i.texcoord + half2( _Offset, -_Offset ) ).a;
				o = min( o, 0.5 );
                c.a *= tex2D( _MainTex, i.texcoord ).a;
				c.rgb = lerp( _OutlineColor, c.rgb, c.a );
				c.a += o;
                return c;
            }

            ENDCG
        }
    }

    Fallback "GUI/Text Shader"
}
