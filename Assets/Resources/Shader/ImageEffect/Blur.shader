
Shader "KYU3D/ImageEffect/Blur"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "white" {}
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

			fixed4 frag(v2f i) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				c.rgb *= 0.1942;
				c.rgb += tex2D( _MainTex, i.uv + _Offset ).rgb * 0.1654;
				c.rgb += tex2D( _MainTex, i.uv - _Offset ).rgb * 0.1654;
				c.rgb += tex2D( _MainTex, i.uv + _Offset * 2 ).rgb * 0.1323;
				c.rgb += tex2D( _MainTex, i.uv - _Offset * 2 ).rgb * 0.1323;
				c.rgb += tex2D( _MainTex, i.uv + _Offset * 3 ).rgb * 0.1052;
				c.rgb += tex2D( _MainTex, i.uv - _Offset * 3 ).rgb * 0.1052;
				return c;
			}

			ENDCG
		}
	}

	FallBack off
}
