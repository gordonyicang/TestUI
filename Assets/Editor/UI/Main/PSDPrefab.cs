using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PSDPrefab {
	// prefab实例化方法
	public static T PrefabInstant<T>(string path,string name,GameObject parent) where T:UnityEngine.Object {
		GameObject temp = AssetDatabase.LoadAssetAtPath (path,typeof(GameObject)) as GameObject;
		GameObject item = GameObject.Instantiate (temp) as GameObject;
		if (item == null) {
			Debug.LogError ("prefab instance failed:"+path);
			return null;
		}
		item.name = name;
		item.transform.SetParent (parent.transform,false);
		//		item.transform.localScale = Vector3.one;
		return item.GetComponent<T> ();
	}
}
