Shader "PSD2UGUI/UiObj_Mask"
{
	Properties
	{
		_LightX( "_LightX", Range( -1, 1 ) ) = -0.5
		_LightY( "_LightY", Range( -1, 1 ) ) = 0.7
		_LightZ( "_LightZ", Range( -1, 1 ) ) = -1.0
		_MainTex( "_MainTex", 2D ) = "gray" {}
		_SpecularTex( "_SpecularTex(R)", 2D ) = "white" {}
		_DiffuseColor( "_DiffuseColor", Color ) = ( 0.8, 0.8, 0.8, 1 )
		_AmbientColor( "_AmbientColor", Color ) = ( 0.7, 0.7, 0.7, 1 )
		_SpecularColor( "_SpecularColor", Color ) = ( 0.9, 0.9, 0.9, 1 )
		_Shininess( "_Shininess", float ) = 10
		_SpecularIntensity( "_SpecularIntensity", float ) = 2
		_MinX ("Min X", Float) = -10
		_MaxX ("Max X", Float) = 10
		_MinY ("Min Y", Float) = -10
		_MaxY ("Max Y", Float) = 10
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType"="Transparent"
		}

		Pass
		{
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
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
				half3 normal : TEXCOORD1;
				half3 view : TEXCOORD2;
				half3 light : TEXCOORD3;
				float3 vpos : TEXCOORD4;
			};
			
			half _LightX;
			half _LightY;
			half _LightZ;
			sampler2D _MainTex;
			sampler2D _SpecularTex;
			half3 _AmbientColor;
			half3 _DiffuseColor;
			half3 _SpecularColor;
			half _Shininess;
			half _SpecularIntensity;
			float _MinX;
            float _MaxX;
            float _MinY;
            float _MaxY;

			v2f vert( appdata_full v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = mul( v.normal, (half3x3)unity_WorldToObject );
				o.view = _WorldSpaceCameraPos - mul( unity_ObjectToWorld, v.vertex ).xyz;
				o.light = normalize( half3( _LightX, _LightY, _LightZ ) );
				o.vpos = v.vertex.xyz;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{
				half4 c;
				half4 st = tex2D( _SpecularTex, i.uv );
				half4 tex = tex2D( _MainTex, i.uv );
				half3 n = normalize( i.normal );
				half3 v = normalize( i.view );
				half s = pow( max( dot( normalize( i.light + v ), n ), 0 ), _Shininess ) * st.r * _SpecularIntensity;
				c.rgb = tex.rgb * ( _AmbientColor + _DiffuseColor * max( dot( i.light, n ), 0 ) + _SpecularColor * s );
				c.a = tex.a * (i.vpos.x >= _MinX) * (i.vpos.x <= _MaxX) * (i.vpos.y >= _MinY) * (i.vpos.y <= _MaxY);
				return c;
			}

			ENDCG
		}
	}
	
	Fallback "KYU3D/Scene/DynamicObj"
}
