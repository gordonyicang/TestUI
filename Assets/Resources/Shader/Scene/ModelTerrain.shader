
Shader "KYU3D/Scene/ModelTerrain"
{
	Properties
	{
		_Control( "_Control", 2D ) = "red" {}
		_Splat0( "_Splat0", 2D ) = "gray" {}
		_Splat1( "_Splat1", 2D ) = "gray" {}
		_Splat2( "_Splat2", 2D ) = "gray" {}
		_Splat3( "_Splat3", 2D ) = "gray" {}
		_MainTex( "_MainTex", 2D ) = "green" {}
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Geometry-20"
			"SplatCount" = "4"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog {Mode Off}
			Cull Back
			ZTest LEqual
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "../KyUnity.cginc"

			struct v2f
			{
				float4 pos : POSITION;
				half4 uv01 : TEXCOORD0;
				half4 uv23 : TEXCOORD1;
				half4 uvCtrl : TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			sampler2D _Control;
			half4 _Control_ST;
			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			half4 _Splat0_ST;
			half4 _Splat1_ST;
			half4 _Splat2_ST;
			half4 _Splat3_ST;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv01.xy = TRANSFORM_TEX( v.texcoord, _Splat0 );
				o.uv01.zw = TRANSFORM_TEX( v.texcoord, _Splat1 );
				o.uv23.xy = TRANSFORM_TEX( v.texcoord, _Splat2 );
				o.uv23.zw = TRANSFORM_TEX( v.texcoord, _Splat3 );
				o.uvCtrl.xy = TRANSFORM_TEX( v.texcoord, _Control );
				o.uvCtrl.zw = LightMapUV( v.texcoord1 );
				UNITY_TRANSFER_FOG(o,o.pos);
				// o.fog = FogColor( o.pos.w, v.vertex );
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 ctrl = tex2D( _Control, i.uvCtrl.xy );
				half3 l = LightMapColor( i.uvCtrl.zw );
				c.rgb = tex2D( _Splat0, i.uv01.xy ).rgb * ctrl.r
						+ tex2D( _Splat1, i.uv01.zw ).rgb * ctrl.g
						+ tex2D( _Splat2, i.uv23.xy ).rgb * ctrl.b
						+ tex2D( _Splat3, i.uv23.zw ).rgb * ctrl.a;
				c.rgb *= l;
				UNITY_APPLY_FOG(i.fogCoord, c);
				c.a = 1;
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Scene/StaticObjLow"
}
