using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RectComponent {

	// 初始化
	public static GameObject InitRectComponent(Children item,GameObject obj,string baseAssetsDir) {
		// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split('\r');
		var prop = new Property ();
		prop.DeserializeProperty (props);

		// 构建控件
		RectTransform rect = PSDPrefab.PrefabInstant<RectTransform>(PSDConst.ASSET_PATH_RECTTRANSFORM,item.name,obj);
		RectTransform parentRect = obj.GetComponent<RectTransform>();

		// 设置锚点
		// Vector2 anchorP = new Vector2 (prop.anchors[0], prop.anchors[1]);
		Vector2 anchorP = prop.anchors;
		rect.anchorMin = PSDConst.PIVOTPOINT1;
		rect.anchorMax = PSDConst.PIVOTPOINT1;
		rect.pivot = anchorP; 

		// RectTransform 根据锚点计算 x,y位置
		if (anchorP == PSDConst.PIVOTPOINT0)
		{ // 中间
			float x = item.options.x + (item.options.width / 2);
			float y = item.options.y + (item.options.height / 2);
			rect.localPosition = new Vector3 (x, -y);

			rect.sizeDelta = new Vector2 (item.options.width, item.options.height);
		}
		else//默认左上
		// else if (anchorP == PSDConst.PIVOTPOINT1) 
		{ // 左上
			float x = item.options.x;
			float y = item.options.y;
			rect.localPosition = new Vector3(x,-y);
			
			rect.sizeDelta = new Vector2 (item.options.width,item.options.height);
		} 

		return rect.gameObject;
	}
}
