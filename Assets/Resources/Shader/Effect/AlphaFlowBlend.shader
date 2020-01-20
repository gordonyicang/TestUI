Shader "KYU3D/Effect/AlphaFlowBlend" 
{
	Properties 
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_MainScaleU( "_MainScaleU", float ) = 1
		_MainScaleV( "_MainScaleV", float ) = 1
		_MainSpeedU( "_MainSpeedU", Float ) = 0.5 
		_MainSpeedV( "_MainSpeedV", Float ) = 0
		_Color( "_Color", Color ) = ( 1, 1, 1, 1 )
		_Intensity ( "_Intensity", Range( 0.1, 3 )) = 1
		_AlphaTex( "_AlphaTex( R )", 2D ) = "gray" {}
		_AlphaScaleU( "_AlphaScaleU", float ) = 1
		_AlphaScaleV( "_AlphaScaleV", float ) = 1
		_AlphaSpeedU( "_AlphaSpeedU", Float ) = 0
		_AlphaSpeedV( "_AlphaSpeedV", Float ) = 0.5
	}

	SubShader
	{		
		LOD 200
		
		Tags
		{
			"Queue"  = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass 
		{
			Lighting Off
			Cull Off
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag
			#pragma multi_compile_fog 
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "../KyUnity.cginc"

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				fixed4 color :TEXCOORD1;
			};

			sampler2D _MainTex;
			half _MainScaleU;
			half _MainScaleV;
			half _MainSpeedU;
			half _MainSpeedV;
			fixed4 _Color;
			half _Intensity;
			sampler2D _AlphaTex;
			half _AlphaScaleU;
			half _AlphaScaleV;
			half _AlphaSpeedU;
			half _AlphaSpeedV;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xyxy * float4( _MainScaleU, _MainScaleV, _AlphaScaleU, _AlphaScaleV ) 
							- float4( _MainSpeedU, _MainSpeedV, _AlphaSpeedU, _AlphaSpeedV ) * _Time.y;
				o.color = v.color * _Intensity * _Color;
				return o;
			}
		
			fixed4 frag( v2f i ) :COLOR
			{
				fixed4 c = tex2D( _MainTex, i.uv.xy );
				c.a *= tex2D( _AlphaTex, i.uv.zw ).r;
				c *= i.color;				
				return c;
			}

			ENDCG
		}		
	}

	FallBack "KYU3D/Effect/Blend"
}
