
Shader "KYU3D/ImageEffect/Combine"
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
			sampler2D _Bloom;
			half _Minus;
			half2 _Center;
			half _Smoothness;
			half3 _Color;
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
				half4 b = tex2D( _Bloom, i.uv );
				half2 d = i.uv - _Center;
				half dd = pow( dot( d, d ), _Smoothness );
				c.rgb += b.rgb - _Minus;
				c.rgb = lerp( c.rgb, _Color * c.rgb, dd * _Intensity );
				return c;
			}

			ENDCG
		}
	}

	FallBack off
}
