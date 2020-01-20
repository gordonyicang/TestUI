
Shader "KYU3D/ImageEffect/Bright"
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
			half _Threshold;
			half _Intensity;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c = tex2D( _MainTex, i.uv );
				half g = max( c.r, max( c.g, c.b ) );
				half t = g - _Threshold;
				half d = clamp( t + 0.5, 0, 1 );
				c.rgb *= max( t, d * d * 0.5 ) / max( g, 1e-5 ) * _Intensity;
				return c;
			}

			ENDCG
		}
	}

	FallBack off
}
