using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ButtonComponent
{
	// 初始化
	public static GameObject InitButtonComponent (Children item, GameObject obj, string baseAssetsDir, string fileName)
	{
		// 获取属性
		var propertys = item.options.u3dLink.options.property.Split ('\r');
		var prop = new Property ();
		prop.DeserializeProperty (propertys);

		bool isCommon = false; // 是否使用公共图集
		string file_result = "";

		RectTransform parentRect = obj.GetComponent<RectTransform> ();

		// UIButton2 貌似是个image类型
		UnityEngine.UI.Image btnComponent = PSDPrefab.PrefabInstant<UnityEngine.UI.Image> (PSDConst.ASSET_PATH_BUTTON2, item.name, obj);
		RectTransform rectTransform = btnComponent.GetComponent<RectTransform> ();

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

		// 按钮如果是外部图片就不设置图片，先从公共图集获取再从私有图集中获取

		/**
			外部图片：outpic
			公共图片：common
			面板独有：剩下的

		1.先判断是否为外部图片，直接返回
		2.每次查询图集先从公共图集里查询
		3.剩下的加入面板独有图集
		*/

		Sprite sprite = null;
		Material material = null;
		string[] link_split = item.options.u3dLink.options.link.Split ('#');
		string rootPath = (link_split.Length > 0) ? (link_split[0]) : "";
		string imgPath = (link_split.Length > 0) ? link_split[link_split.Length - 1] : "";
		string prefix = (imgPath.Split('/').Length > 1) ? imgPath.Split('/')[0] : "";
		string imgName = (imgPath.Split('/').Length > 0) ? imgPath.Split('/')[imgPath.Split('/').Length - 1] : "";
		imgName = Path.GetFileNameWithoutExtension (imgName);
		string atlasName = (prefix != "") ? (prefix + "-" + imgName) : (imgName);

		// 获取图集名称,先从公共图集中查询
		if (File.Exists (PSDConst.ATLAS_PATH_COMMON)) {
			//Debug.Log ("!!!"+item.name+"<<>>"+atlasName);
			sprite = AtlasManager.getSpriteForTextureP (PSDConst.ATLAS_PATH_COMMON, atlasName);
			// 加载材质
			material = AtlasManager.getMaterForAtlasPath(PSDConst.ATLAS_PATH_COMMON); 
			if (sprite != null) {
				isCommon = true;
			}
		}

		if (sprite == null) {
			isCommon = false;
			// 直接对该文件夹打包
			string srcPath = baseAssetsDir + rootPath + '/';
			string tarPath = PSDConst.GUI_PATH + fileName + '/' + fileName + PSDConst.PNG_SUFFIX;
			//bool isSave = AtlasManager.InitAtlasForTextureP (srcPath,tarPath);
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
			// 外部图片统一放到"out/*/*.png"
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
				PrefabUtility.CreatePrefab (prefabPath,btnComponent.gameObject,ReplacePrefabOptions.ReplaceNameBased);
				UnityEngine.UI.Image fixPrefabObj = AssetDatabase.LoadAssetAtPath (prefabPath,typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
				fixPrefabObj.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outImg);
				fixPrefabObj.material = AtlasManager.getMaterForAtlasPath (outImg);
			}
			return btnComponent.gameObject;
		}

		btnComponent.sprite = sprite;
		btnComponent.material = material;

		var colorR = prop.colorR / 255f;
		var colorG = prop.colorG / 255f;
		var colorB = prop.colorB / 255f;
		var alpha = prop.opacity / 255f;
		btnComponent.color = new Color (colorR, colorG, colorB, alpha);

		// 是否设置九宫格
		if (item.options.u3dLink.options.isScaleImage == true) {
			TextureImporter importer;
			if (isCommon) {
				importer = AssetImporter.GetAtPath (PSDConst.ATLAS_PATH_COMMON) as TextureImporter;
				importer.isReadable = true;
			} else {
				importer = AssetImporter.GetAtPath (PSDConst.GUI_PATH + file_result + '/' + file_result + PSDConst.PNG_SUFFIX) as TextureImporter;
				importer.isReadable = true;
			}

			SpriteMetaData[] datas = importer.spritesheet;
			for (int i = 0; i < datas.Length; i++) {
				if (datas[i].name == atlasName) {
					datas [i].border = new Vector4 (item.options.u3dLink.options.left, item.options.u3dLink.options.bottom, item.options.u3dLink.options.right, item.options.u3dLink.options.top);
				}
			}
			importer.spritesheet = datas;
			btnComponent.type = UnityEngine.UI.Image.Type.Sliced;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.SaveAndReimport();
		}

		return btnComponent.gameObject;
	}
}
