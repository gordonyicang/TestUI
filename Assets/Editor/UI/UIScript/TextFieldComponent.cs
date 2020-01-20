using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TextFieldComponent {
	public static GameObject InitTextFieldComponent(Children item,GameObject obj,string baseAssetsDir) {
				// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split('\r');
		var prop = new Property ();
		prop.DeserializeProperty (props);

		UnityEngine.UI.InputField fieldComponent = PSDPrefab.PrefabInstant<UnityEngine.UI.InputField>(PSDConst.ASSET_PATH_TEXTFIELD,item.name,obj);
		UnityEngine.UI.Text textComponent = fieldComponent.textComponent;
		RectTransform textRectTransform = textComponent.GetComponent<RectTransform>();
		RectTransform rectTransform = fieldComponent.GetComponent<RectTransform>();
		RectTransform parentRect = obj.GetComponent<RectTransform>();

		// 如果有额外属性，先根据额外属性设置控件
		if ((prop.scaleX > 0f) || (prop.scaleY > 0f)) {
			rectTransform.localScale = new Vector3 (prop.scaleX,prop.scaleY,1f);
			textRectTransform.localScale = new Vector3 (prop.scaleX,prop.scaleY,1f);
		}
		if ((prop.colorR < 255) || (prop.colorG < 255) || (prop.colorB < 255)) {
			textComponent.color = new Color (prop.colorR,prop.colorG,prop.colorB);
		}

		// 构建控件
		if(item.options.Leading == 0)
		{
			//修正 ps默认行间距是 字号的1.75倍
			item.options.Leading = 1f * item.options.fontSize;
		}
		// Debug.Log("input文本内容 = "+item.options.text);
		textComponent.lineSpacing = item.options.Leading / item.options.fontSize;
		string a = item.options.text.Replace("\r","\n");
		// Debug.Log("input修正后内容 = "+a);
		fieldComponent.text = a;
		textComponent.fontSize = item.options.fontSize;
		// textComponent.verticalOverflow = UnityEngine.VerticalWrapMode.Truncate;
		//多行
		// if (!item.options.singleLine)
		// {
			// fieldComponent.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;
            // textComponent.horizontalOverflow = UnityEngine.HorizontalWrapMode.Wrap;
        // }

		// 居中方式
		switch(item.options.Justification) 
		{
		case 0: // 左
			textComponent.alignment = TextAnchor.UpperLeft;
			break;
		case 1: // 右
			textComponent.alignment = TextAnchor.UpperRight;
			break;
		case 2: // 中
			textComponent.alignment = TextAnchor.UpperCenter;
			break;
		}


		// 默认勾上，按几何对齐，否则距离上边缘会有2个像素偏差
		//勾上 输入英文和_时，会出问题
        // textComponent.alignByGeometry = true;

		var colorR = item.options.colorR/255f;
		var colorG = item.options.colorG/255f;
		var colorB = item.options.colorB/255f;
		var alpha = item.options.opacity/255f;
		textComponent.color = new Color (colorR,colorG,colorB,alpha);

		var font = AssetDatabase.LoadAssetAtPath(PSDConst.ASSET_PATH_MICYAHEI,typeof(Font)) as Font;
		textComponent.font = font;

		//设置锚点(父锚点默认左上跟psd一致，锚点读psd设置)
		//文本中心点pivot只能左上角，因为文字是从中心点开始往右输出的
		Vector2 anchorP = PSDConst.PIVOTPOINT1;
		rectTransform.anchorMin = PSDConst.PIVOTPOINT1;
		rectTransform.anchorMax = PSDConst.PIVOTPOINT1;
		rectTransform.pivot = anchorP; 

		textRectTransform.anchorMin = PSDConst.PIVOTPOINT1;
		textRectTransform.anchorMax = PSDConst.PIVOTPOINT1;
		textRectTransform.pivot = PSDConst.PIVOTPOINT1;


		// RectTransform 根据锚点计算 x,y位置
		if (anchorP == PSDConst.PIVOTPOINT0) 
		{ //中间
			float x = item.options.x + (item.options.width / 2);
			float y = item.options.y + (item.options.height / 4);
			rectTransform.localPosition = new Vector3(x,-y);

			float width = item.options.width;
			float height = (item.options.height < (float)(item.options.fontSize)) ? (float)(item.options.fontSize) : item.options.height;
			rectTransform.sizeDelta = new Vector2 (width,height);
			textRectTransform.sizeDelta = new Vector2 (width,height);
		}
		else//默认左上
		// else if (anchorP == PSDConst.PIVOTPOINT1) 
		{ // 左上
			float x = item.options.x;
			float y = item.options.y - PSDConst.TEXT_FIX_Y;
			rectTransform.localPosition = new Vector3(x,-y);

			float width = item.options.width;
			float height = item.options.height;
			height = (height < (float)(item.options.fontSize)) ? (float)(item.options.fontSize) : height;
			rectTransform.sizeDelta = new Vector2 (width,height + PSDConst.TEXTFIELD_FIX_H);
			textRectTransform.sizeDelta = new Vector2 (width,height + PSDConst.TEXTFIELD_FIX_H);
		}  

		return fieldComponent.gameObject;
	}
}
