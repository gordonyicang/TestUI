Shader "KYU3D/Scene/BackSky"
{
	Properties
	{
		[NoScaleOffset]_MainTex ( "_MainTex", 2D ) = "gray" {}
	}

	SubShader
	{
		LOD 200
		Tags 
		{ 
			"Queue" = "Background" 
		}
		
		Pass
		{
			Lighting Off 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{				
				float4 vertex : SV_POSITION;
				half2 uv_MainTex : TEXCOORD0;
			};

			sampler2D _MainTex;
			
			v2f vert ( appdata_base v )
			{
				v2f o;
				o.vertex = UnityObjectToClipPos( v.vertex );
				o.uv_MainTex = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D( _MainTex, i.uv_MainTex );
				return c;
			}
			ENDCG
		}
	}

	Fallback "Unlit/Texture"
}
