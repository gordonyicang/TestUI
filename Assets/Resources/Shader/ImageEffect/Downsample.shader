
Shader "KYU3D/ImageEffect/Downsample"
{
	Properties
	{
		_MainTex( "Texture", 2D ) = "gray" {}
	}

	SubShader
	{
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			half2 _Offset;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				c.rgb = tex2D( _MainTex, i.uv + _Offset ).rgb;
				c.rgb = max( c.rgb, tex2D( _MainTex, i.uv - _Offset ).rgb );
				c.rgb = max( c.rgb, tex2D( _MainTex, i.uv + float2( -_Offset.x, _Offset.y ) ).rgb );
				c.rgb = max( c.rgb, tex2D( _MainTex, i.uv + float2( _Offset.x, -_Offset.y ) ).rgb );
				c.a = 1;
				return c;
			}

			ENDCG
		}
	}

	FallBack off
}
