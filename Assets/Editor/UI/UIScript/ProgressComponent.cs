using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class ProgressComponent {
	public static GameObject InitProgressComponent(Children item,GameObject obj,string baseAssetsDir,string fileName) {
		// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split ('\r');
		var prop = new Property ();
		// 图片--额外属性
		var imgPropStr = item.options.bar.options.property;
		var imgProps = propStr.Split('\r');
		var imgProp = new Property ();
		imgProp.DeserializeProperty (imgProps);
		prop.DeserializeProperty (props);

		bool isCommon = false; // 是否使用公共图集
		string file_result = "";

		// 控件获取
		UnityEngine.UI.Image progressComponent = PSDPrefab.PrefabInstant<UnityEngine.UI.Image>(PSDConst.ASSET_PATH_PROGRESSBAR,item.name,obj);
		RectTransform rectTransform = progressComponent.GetComponent<RectTransform> ();
		RectTransform parentRect = obj.GetComponent<RectTransform>();

		// 额外属性设置
		if ((prop.scaleX > 0f) || (prop.scaleY > 0f)) {
			rectTransform.localScale = new Vector3 (prop.scaleX,prop.scaleY);
		}

		// 设置锚点
		// Vector2 anchorP = new Vector2 (prop.anchors[0], prop.anchors[1]);
		Vector2 anchorP = prop.anchors;
		rectTransform.anchorMin = PSDConst.PIVOTPOINT1;
		rectTransform.anchorMax = PSDConst.PIVOTPOINT1;
		rectTransform.pivot = anchorP; 

		// RectTransform 根据锚点计算 x,y位置
		if (anchorP == PSDConst.PIVOTPOINT0)
		{ // 中间
			float x = item.options.x + (item.options.width / 2);
			float y = item.options.y + (item.options.height / 2);
			rectTransform.localPosition = new Vector3 (x, -y);

			rectTransform.sizeDelta = new Vector2 (item.options.width, item.options.height);
		}
		else//默认左上
		// else if (anchorP == PSDConst.PIVOTPOINT1) 
		{ // 左上
			float x = item.options.x;
			float y = item.options.y;
			rectTransform.localPosition = new Vector3(x,-y);
			
			rectTransform.sizeDelta = new Vector2 (item.options.width,item.options.height);
		} 

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
			string outSrc = baseAssetsDir + rootPath + "/" + imgPath;
			string outDir = PSDConst.OUT_PATH + "/";
			string outImg = outDir + imgPath;
			string saveDir = Directory.GetParent (outImg).FullName;
			string outMetaSrc = outSrc + PSDConst.META_SUFFIX;
			string outMetaImg = outImg + PSDConst.META_SUFFIX;
			string prefabPath = Path.GetDirectoryName(outImg) + "/" + Path.GetFileNameWithoutExtension(outImg) + PSDConst.PREFAB_SUFFIX;
			if (!Directory.Exists (saveDir)) {
				Directory.CreateDirectory (saveDir);
			}
			if (!File.Exists (outImg)) {
				// 对原图先转换成精灵图片后拷贝
				SpriteManager.conversion(outSrc);
				File.Copy (outSrc,outImg);
				File.Copy (outMetaSrc,outMetaImg);
				// 生成材质 prefab 分离透明通道
				AtlasManager.GenerateAlpha (outImg);
				AtlasManager.CreateMaterials (outImg);
				PrefabUtility.CreatePrefab (prefabPath,progressComponent.gameObject,ReplacePrefabOptions.ReplaceNameBased);
				UnityEngine.UI.Image fixPrefabObj = AssetDatabase.LoadAssetAtPath (prefabPath,typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
				fixPrefabObj.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outImg);
				fixPrefabObj.material = AtlasManager.getMaterForAtlasPath (outImg);
			}
			sprite = null;
		}

		// 图片--九宫格
		if (item.options.bar.options.isScaleImage == true) {
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
			progressComponent.type = UnityEngine.UI.Image.Type.Sliced;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.SaveAndReimport();
		}

		progressComponent.sprite = sprite;
		progressComponent.material = material;
		return progressComponent.gameObject;
	}
}
