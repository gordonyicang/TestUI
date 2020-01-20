using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class SliderComponent {
	public static GameObject InitSliderComponent(Children item,GameObject obj,string baseAssetsDir,string fileName) {
		Debug.Log ("###InitSliderComponent");

		// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split('\r');
		var prop = new Property ();
		prop.DeserializeProperty (props);

		// 设置锚点（暂时全部控件设置为左上）
		Vector2 anchorP = new Vector2(0.0f,1.0f);
		bool isCommon = false; // 是否使用公共图集
		string file_result = "";

		// 控件获取
		UnityEngine.UI.Slider sliderComponent = PSDPrefab.PrefabInstant<UnityEngine.UI.Slider>(PSDConst.ASSET_PATH_SLIDER,item.name,obj);
		RectTransform rectTransform = sliderComponent.GetComponent<RectTransform>();
		RectTransform parentRect = obj.GetComponent<RectTransform>();
		UnityEngine.UI.Image fillImg = sliderComponent.fillRect.GetComponent<UnityEngine.UI.Image> ();
		var fillRect = sliderComponent.transform.Find("Fill Area").GetComponent<RectTransform>();
		var fillImgRect = fillImg.GetComponent<RectTransform> ();

		rectTransform.anchorMax = anchorP;
		rectTransform.anchorMin = anchorP;
		rectTransform.pivot = anchorP;
		fillImgRect.anchorMax = anchorP;
		fillImgRect.anchorMin = anchorP;
		fillImgRect.pivot = anchorP;

		// 额外属性设置
		if ((prop.scaleX > 0f) || (prop.scaleY > 0f)) {
			rectTransform.localScale = new Vector3 (prop.scaleX,prop.scaleY);
		}

		// 位置尺寸
		if (anchorP == new Vector2 (0.0f, 1.0f)) { // 左上
			float x = item.options.x;
			float y = item.options.y;
			rectTransform.localPosition = new Vector3(x,-y);
			rectTransform.sizeDelta = new Vector2(item.options.width, item.options.height);
		} else { // 默认中间
			rectTransform.sizeDelta = new Vector2(item.options.width, item.options.height);
			float x = item.options.x - (parentRect.rect.width / 2) + (item.options.width / 2);
			float y = item.options.y - (parentRect.rect.height / 2) + (item.options.height / 2);
			rectTransform.localPosition = new Vector3(x,-y);
		}
			
		// 颜色

		// 图片--额外属性
		var imgPropStr = item.options.bar.options.property;
		var imgProps = propStr.Split('\r');
		var imgProp = new Property ();
		imgProp.DeserializeProperty (imgProps);
		if (imgPropStr.Length > 0) {
//			Debug.Log ("根据额外的属性值重设控件");
			fillImg.color = new Color(imgProp.colorR,imgProp.colorG,imgProp.colorB,imgProp.opacity);
			fillRect.anchorMin = anchorP;
			fillRect.anchorMax = anchorP;
			fillRect.pivot = anchorP;
			fillRect.localScale = new Vector3 (imgProp.scaleX,imgProp.scaleY);
		}
		// 图片--位置尺寸
		fillRect.sizeDelta = new Vector2(item.options.width, item.options.height);
		fillRect.localPosition = new Vector3(0,0);

		/*
		// 图片
		string imgPath = baseAssetsDir+item.options.bar.options.link.Replace("#","/");

		// 使用最外层目录作为图集标记
		string atlasTag = item.options.bar.options.link.Split('#')[0];
		TextureImporter texture = AtlasManager.InitAtlas (imgPath,atlasTag);
		*/

		Sprite sprite = null;
		Material material = null;
		string[] link_split = item.options.bar.options.link.Split ('#');
		string rootPath = (link_split.Length > 0) ? (link_split[0]) : "";
		string imgPath = (link_split.Length > 0) ? link_split[link_split.Length - 1] : "";
		string prefix = (imgPath.Split('/').Length > 1) ? imgPath.Split('/')[0] : "";
		string imgName = (imgPath.Split('/').Length > 0) ? imgPath.Split('/')[imgPath.Split('/').Length - 1] : "";
		imgName = Path.GetFileNameWithoutExtension (imgName);
		string atlasName = (prefix != "") ? (prefix + "-" + imgName) : (imgName);

		// 获取图集名称,先从公共图集中查询
		if (File.Exists (PSDConst.ATLAS_PATH_COMMON)) {
			sprite = AtlasManager.getSpriteForTextureP (PSDConst.ATLAS_PATH_COMMON, atlasName);
			// 加载材质
			material = AtlasManager.getMaterForAtlasPath(PSDConst.ATLAS_PATH_COMMON); 
			if (sprite != null) {
				isCommon = true;
			}
		}

		// Texture图集打包(面板独有)
		if (sprite == null) {
			isCommon = false;
			string srcPath = baseAssetsDir + rootPath + '/';
			string tarPath = PSDConst.GUI_PATH + fileName + '/' + fileName + PSDConst.PNG_SUFFIX;
			file_result = fileName;
			if ((fileName.Contains (PSDConst.TAG_TABVIEW)) && (fileName.Contains (PSDConst.TAG_TABVIEW_ITEM))) {
				string[] file_split = fileName.Split ('_');
				for (int i = 0; i < file_split.Length; i++) {
					if (file_split[i].Contains(PSDConst.TAG_TABVIEW_ITEM)) {
						file_result = fileName.Replace (("_"+file_split[i]),(""));
						tarPath = PSDConst.GUI_PATH + file_result + '/' + file_result + PSDConst.PNG_SUFFIX;
					}
				}
			}
			if (File.Exists(tarPath)) {
				sprite = AtlasManager.getSpriteForTextureP (tarPath,atlasName);
			}
			// 加载材质
			material = AtlasManager.getMaterForAtlasPath(tarPath); 
		}

		if (prop.isOutpic > 0) { // 外部图片
			// 外部图片统一放到"out/*/*.png"  Item_tvaabb  Item_tvaabbBoy
			string outSrc = baseAssetsDir + rootPath + "/" + imgPath;
			string outDir = PSDConst.OUT_PATH + "/";
			string outImg = outDir + imgPath;
			string saveDir = Directory.GetParent (outImg).FullName;
			if (!Directory.Exists (saveDir)) {
				Directory.CreateDirectory (saveDir);
			}
			if (!File.Exists (outImg)) {
				File.Copy (outSrc,outImg);
			}
			sprite = null;
		}

		// 图片--九宫格
		if (item.options.bar.options.isScaleImage == true) {
			/*
			texture.spriteBorder = new Vector4 (item.options.bar.options.left,item.options.bar.options.bottom,item.options.bar.options.right,item.options.bar.options.left);
			fillImg.type = UnityEngine.UI.Image.Type.Sliced;
			AssetDatabase.WriteImportSettingsIfDirty (imgPath);
			AssetDatabase.ImportAsset (imgPath);
			*/
			// Texture图集九宫格切图
			TextureImporter importer = null;
			if (isCommon) {
				importer = AssetImporter.GetAtPath (PSDConst.ATLAS_PATH_COMMON) as TextureImporter;
				importer.isReadable = true;
			} else {
				importer = AssetImporter.GetAtPath (PSDConst.GUI_PATH + file_result + '/' + file_result + PSDConst.PNG_SUFFIX) as TextureImporter;
				importer.isReadable = true;
			}

			SpriteMetaData[] datas = importer.spritesheet;
			for (int i = 0; i < datas.Length; i++) {
				//Debug.Log (datas[i].name + "<<<>>>" + atlasName);
				if (datas[i].name == atlasName) {
					//Debug.Log ("9sclice");
					datas [i].border = new Vector4 (item.options.bar.options.left, item.options.bar.options.bottom, item.options.bar.options.right, item.options.bar.options.top);
				}
			}
			importer.spritesheet = datas;
			fillImg.type = UnityEngine.UI.Image.Type.Sliced;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.SaveAndReimport();
		}

		//Sprite sprite = AssetDatabase.LoadAssetAtPath (imgPath, typeof(Sprite)) as Sprite;
		fillImg.sprite = sprite;
		fillImg.material = material;
		sliderComponent.value = 1f;

		return sliderComponent.gameObject;
	}

}
