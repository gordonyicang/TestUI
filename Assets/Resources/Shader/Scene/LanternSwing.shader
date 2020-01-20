// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KYU3D/Scene/LanternSwing" 
{
	Properties 
	{
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_LightmapIntensity( "_LightmapIntensity", float ) = 1
		_Cutoff( "_Cutoff", float ) = 0.7
		_Amplitude( "_Amplitude", float ) = 1
		_Frequency( "_Frequency", float ) = 0.5
		_DirectionX( "_DirectionX", float ) = 0.2
		_DirectionY( "_DirectionY", float ) = 0
		_DirectionZ( "_DirectionZ", float ) = 0	
		_HightOffset( "_HightOffset", float ) = 0.1
	}
	SubShader 
	{
		LOD 200

		Tags 
		{ 
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"DisableBatching" = "True"
		}

		Pass 
		{
			Lighting Off 
			
			Cull Off 
			ZTest LEqual
			ZWrite On

			CGPROGRAM 

			#pragma vertex vert 
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_presicion_hint_fastest
			#include"../KyUnity.cginc"

			struct v2f 
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				half2 lmuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex;
			half _LightmapIntensity;
			half _Cutoff;
			half _Amplitude;
			half _Frequency;
			half _DirectionX;
			half _DirectionY;
			half _DirectionZ;
			half _HightOffset;

			v2f vert( appdata_full v )
			{					
				v2f o;
				float3 moveDir = float3( _DirectionX, _DirectionY, _DirectionZ );
				float3 pos = float3( 0, 0, 0 ) + float3( 0, _HightOffset, 0 );
				float dis = distance( pos, v.vertex.xyz );
				v.vertex.xz += _Amplitude * sin( _Frequency * _Time.y  ) * moveDir.xz * v.color.a;
				float distancesXZ = distance( pos.xz, v.vertex.xz );
				v.vertex.y = pos.y - sqrt( dis * dis - distancesXZ * distancesXZ );
				o.pos = UnityObjectToClipPos( v.vertex );			
				o.uv = v.texcoord.xy;
				o.lmuv = LightMapUV( v.texcoord1 );
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag( v2f i ) : Color 
			{
				fixed4 c = tex2D( _MainTex, i.uv );
				fixed3 lm;
				lm = LightMapColor( i.lmuv ) * _LightmapIntensity;
				c.rgb *= lm;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}

			ENDCG 
		}
	}

	FallBack "KYU3D/Scene/PlantFlutterCut"
}
