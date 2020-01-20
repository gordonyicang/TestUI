
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SceneLightMgr : MonoBehaviour
{
	public Color _Ambient = new Color( 0.7f, 0.7f, 0.7f );
	public float _AmbientIntensity = 1;

	public Color _CharDiffuse = new Color( 0.7f, 0.7f, 0.7f );
	public float _CharDiffuseIntensity = 1;
	public Color _CharSpecular = new Color( 0.7f, 0.7f, 0.7f );
	public float _CharSpecularIntensity = 1;
	
	public Color _ObjDiffuse = new Color( 0.7f, 0.7f, 0.7f );
	public float _ObjDiffuseIntensity = 1;
	public Color _ObjSpecular = new Color( 0.7f, 0.7f, 0.7f );
	public float _ObjSpecularIntensity = 1;

	public Color _HarmoneyColor = new Color( 0.3f, 1.0f, 0.7f, 1.0f );

	public bool _FogEnable = true;
	public float _FogStart = 12;
	public float _FogEnd = 50;
	public float _FogIntensity = 1;
	public Color _FogColor1 = new Color( 0.0f, 0.8f, 1.0f );
	public Color _FogColor2 = new Color( 0.6f, 0.8f, 0.7f );
	public float _FogVaryCenter = 15;
	public float _FogVaryRange = 10;

	void Update()
	{
		UpdateLight();
	}

	private void UpdateLight()
	{
		Shader.SetGlobalColor( "_Ambient", _Ambient * _AmbientIntensity);
		Shader.SetGlobalColor( "_ObjDiffuse", _ObjDiffuse * _ObjDiffuseIntensity );
		Shader.SetGlobalColor( "_ObjSpecular", _ObjSpecular * _ObjSpecularIntensity );
		Shader.SetGlobalColor( "_CharDiffuse", _CharDiffuse * _CharDiffuseIntensity );
		Shader.SetGlobalColor( "_CharSpecular", _CharSpecular * _CharSpecularIntensity );
		
		Shader.SetGlobalFloat( "_FogStart", _FogStart );
		Shader.SetGlobalFloat( "_FogIncrease", _FogEnable ? 1 / (_FogEnd - _FogStart) * _FogIntensity : 0 );
		Shader.SetGlobalColor( "_FogColor1", _FogColor1 );
		Shader.SetGlobalColor( "_ColorDelta", _FogColor2 - _FogColor1 );
		Shader.SetGlobalFloat( "_Color1Y", _FogVaryCenter + 0.5f * _FogVaryRange );
		Shader.SetGlobalFloat( "_ColorYDelta", 1 / _FogVaryRange );

		float g = _HarmoneyColor.r * 0.299f + _HarmoneyColor.g * 0.587f + _HarmoneyColor.b * 0.114f;
		Color c = new Color( _HarmoneyColor.r / g, _HarmoneyColor.g / g, _HarmoneyColor.b / g );
		Shader.SetGlobalColor( "_HarmoneyColor", c );
	}
}
