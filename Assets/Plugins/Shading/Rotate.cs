
using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	public Vector3 _RotateSpeed = new Vector3( 0, 0, 180.0f );

	void Update()
	{
		transform.Rotate( _RotateSpeed * Time.deltaTime, Space.Self );
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
