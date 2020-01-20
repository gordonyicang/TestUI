
using UnityEngine;
using System.Collections;

public class FloatAni : MonoBehaviour
{
	public Vector3 _Amplitude = new Vector3( 0, 1, 0 );
	public Vector3 _Speed = new Vector3( 0, 2, 0 );
	public Vector3 _Offset = new Vector3( 0, 1, 0 );

	private Vector3 _OriginalPos;

	private void Start()
	{
		_OriginalPos = transform.position;
	}

	void Update()
	{
		Vector3 pos = _OriginalPos + _Offset;
		pos.x += Mathf.Sin( Time.timeSinceLevelLoad * _Speed.x ) * _Amplitude.x;
		pos.y += Mathf.Sin( Time.timeSinceLevelLoad * _Speed.y ) * _Amplitude.y;
		pos.z += Mathf.Sin( Time.timeSinceLevelLoad * _Speed.z ) * _Amplitude.z;
		transform.position = pos;
	}

	void OnBecameVisible()
	{
		enabled = true;
	}

	void OnBecameInvisible()
	{
		enabled = false;
	}
}
