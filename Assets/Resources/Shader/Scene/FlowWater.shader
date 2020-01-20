Shader "KYU3D/Scene/FlowWater"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "blue" {}
		_Color( "_Color", Color ) = ( 0.5, 1, 1, 1 )

		_Intensity1( "_Intensity1", float ) = 1
		_TexScale1( "_TexScale1", float ) = 1
		_FlowSpeedU1( "_FlowSpeedU1", float ) = 0
		_FlowSpeedV1( "_FlowSpeedV1", float ) = 0.5
		
		_Intensity2( "_Intensity2", float ) = 1
		_TexScale2( "_TexScale2", float ) = 1.5
		_FlowSpeedU2( "_FlowSpeedU2", float ) = 0
		_FlowSpeedV2( "_FlowSpeedV2", float ) = 0.4
	}
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			// Fog {Mode Off}
			Cull Off
			ZTest LEqual
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				fixed alpha : TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			sampler2D _MainTex;
			fixed4 _Color;
			fixed _Intensity1;
			float _TexScale1;
			float _FlowSpeedU1;
			float _FlowSpeedV1;
			fixed _Intensity2;
			float _TexScale2;
			float _FlowSpeedU2;
			float _FlowSpeedV2;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv1 = v.texcoord.xy * _TexScale1 + float2( _FlowSpeedU1, _FlowSpeedV1 ) * _Time.y;
				o.uv2 = v.texcoord.xy * _TexScale2 + float2( _FlowSpeedU2, _FlowSpeedV2 ) * _Time.y;
				o.alpha = v.color.a;
				// o.fog = FogColor( o.pos.w, v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c1 = tex2D( _MainTex, i.uv1 );
				fixed4 c2 = tex2D( _MainTex, i.uv2 );
				fixed4 c = c1 * _Intensity1 *  c2 * _Intensity2 * _Color;
				c.a *= i.alpha;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}

	Fallback "KYU3D/Simple/Transparent"
}
