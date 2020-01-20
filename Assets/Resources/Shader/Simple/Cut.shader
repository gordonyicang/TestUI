Shader "KYU3D/Simple/Cut"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "gray" {}
	}
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// Fog {Mode Off}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				CUT;
				return c;
			}
			ENDCG
		}
	}

	Fallback "Transparent/Cutout/Diffuse"
}
