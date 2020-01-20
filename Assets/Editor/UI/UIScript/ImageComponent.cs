using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ImageComponent
{
	// 初始化
	public static GameObject InitImageComponent (Children item, GameObject obj, string baseAssetsDir, string fileName)
	{
		// 获取属性
		var propertys = item.options.property.Split ('\r');
		var prop = new Property ();
		prop.DeserializeProperty (propertys);


		
		bool isCommon = false; // 是否使用公共图集
		string file_result = "";


		RectTransform parentRect = obj.GetComponent<RectTransform> ();

		/**
			外部图片：outpic
			公共图片：common
			面板独有：剩下的

		1.先判断是否为外部图片，预留空go，单独图集
		2.每次查询图集先从公共图集里查询
		3.剩下的加入面板独有图集
		*/

		// 构建图片控件
		UnityEngine.UI.Image imgComponent = PSDPrefab.PrefabInstant<UnityEngine.UI.Image> (PSDConst.ASSET_PATH_IMAGE, item.name, obj);
		// 设置图片控件位移及锚点信息
		RectTransform rectTransform = imgComponent.GetComponent<RectTransform> ();

		// 设置锚点
		// Vector2 anchorP = new Vector2 (prop.anchors[0], prop.anchors[1]);
		// 默认中心的点
		Vector2 anchorP = PSDConst.PIVOTPOINT0;
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
			rectTransform.localPosition = new Vector3 (x, -y);
			
			rectTransform.sizeDelta = new Vector2 (item.options.width, item.options.height);
		} 

		string assetPath = "";
		/* unity 自带图集sprite打包
		 * // 获取图片名称，先从公共图集中查询是否有
		string commonDir = baseAssetsDir + "common";
		int linkLast = (item.options.link.Split ('/').Length > 0) ? (item.options.link.Split('/').Length - 1) : 0;
		string imgName = item.options.link.Split ('/') [linkLast];
//		Debug.Log ("###imgName:"+imgName);


		TextureImporter textureImporter;
		bool isFind = false;
		if (Directory.Exists(commonDir)) {
			string[] imgPaths = Directory.GetFiles (commonDir,imgName,SearchOption.AllDirectories);
			isFind = (imgPaths.Length > 0) ? true : false;
			if (isFind) {
				assetPath = imgPaths [0];
			}
		}
		if (!isFind) {
			// 普通图片转精灵图片
			assetPath = baseAssetsDir + item.options.link.Replace ("#", "/"); // 图片路径
			string atlasTag = item.options.link.Split ('#') [0];
			// 根据文件名称合并图集
			textureImporter = AtlasManager.InitAtlas (assetPath, atlasTag);
		}
			
		// 设置精灵图片
		textureImporter = AssetImporter.GetAtPath (assetPath) as TextureImporter;
		Sprite sprite = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Sprite)) as Sprite;
		*/
		// btn-closeBtn2
		Sprite sprite = null;
		Material material = null;
		string[] link_split = item.options.link.Split ('#');
		string rootPath = (link_split.Length > 0) ? (link_split[0]) : "";
		string imgPath = (link_split.Length > 0) ? link_split[link_split.Length - 1] : "";
		string prefix = (imgPath.Split('/').Length > 1) ? imgPath.Split('/')[0] : "";
		string imgName = (imgPath.Split('/').Length > 0) ? imgPath.Split('/')[imgPath.Split('/').Length - 1] : "";
		imgName = Path.GetFileNameWithoutExtension (imgName);
		string atlasName = (prefix != "") ? (prefix + "-" + imgName) : (imgName);
		//atlasName = Path.GetFileNameWithoutExtension (atlasName);
		//Debug.Log("$$$:"+rootPath);
		// 获取图集名称,先从公共图集中查询
		if (File.Exists (PSDConst.ATLAS_PATH_COMMON)) {
			sprite = AtlasManager.getSpriteForTextureP (PSDConst.ATLAS_PATH_COMMON, atlasName);
			// 加载材质
			material = AtlasManager.getMaterForAtlasPath(PSDConst.ATLAS_PATH_COMMON); 
			if (sprite != null) {
				isCommon = true;
			}
		}
		Debug.Log ("$$$imgPath: "+imgPath);
		//Debug.Log ("$$$src:"+baseAssetsDir + rootPath + '/');
		//Debug.Log ("$$$tar:"+PSDConst.GUI_PATH + rootPath + '/' + rootPath + PSDConst.PNG_SUFFIX);
		// Texture图集打包(面板独有)
		if (sprite == null) {
			isCommon = false;

			// 直接对该文件夹打包
			string srcPath = baseAssetsDir + rootPath + '/';
			//string tarPath = PSDConst.GUI_PATH + rootPath + '/' + rootPath + PSDConst.PNG_SUFFIX;
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
			//bool isSave = AtlasManager.InitAtlasForTextureP (srcPath,tarPath);
			if (File.Exists(tarPath)) {
				Debug.Log ("!!!!:"+imgPath.Split('/').Length);
				Debug.Log ("###atlasName:"+atlasName);
				sprite = AtlasManager.getSpriteForTextureP (tarPath,atlasName);
			}
			else{
				Debug.Log ("File is not Exists:"+tarPath);
			}
			// 加载材质
			material = AtlasManager.getMaterForAtlasPath(tarPath); 

		}

		if (sprite == null) {
            //Debug.Log ("loading asset at path: " + assetPath);
        }

        // 设置控件背景颜色（包含透明度）
        var colorR = 1f;
        var colorG = 1f;
        var colorB = 1f;
        var alpha = item.options.opacity / 255f;
        if (item.options.property != "")
        {
            colorR = prop.colorR / 255f;
            colorG = prop.colorG / 255f;
            colorB = prop.colorB / 255f;
            alpha = prop.opacity / 255f;
        }
        if (prop.isOutpic > 0)
		{
            material = null;
            alpha = 0;
		}
		imgComponent.color = new Color (colorR, colorG, colorB, alpha);
        imgComponent.sprite = sprite;
        imgComponent.material = material;

        if (prop.isOutpic > 0) { // 外部图片
			if(prop.isOutpic == 2) return imgComponent.gameObject;
			// 外部图片统一放到"out/*/*.png"  Item_tvaabb  Item_tvaabbBoy
			string outSrc = baseAssetsDir + rootPath + "/" + imgPath;
			string outDir = PSDConst.OUT_PATH + "/";
			string outImg = outDir + rootPath+"-"+imgPath;
			string saveDir = Directory.GetParent (outImg).FullName;
			string outMetaSrc = outSrc + PSDConst.META_SUFFIX;
			string outMetaImg = outImg + PSDConst.META_SUFFIX;
			// prefab路径只能使用相对路径!!!
			string prefabPath = Path.GetDirectoryName(outImg) + "/" + Path.GetFileNameWithoutExtension(outImg) + PSDConst.PREFAB_SUFFIX;
			if (!Directory.Exists (saveDir)) {
				Directory.CreateDirectory (saveDir);
			}
			if (!File.Exists (outImg)) {
				// 对原图先转换成精灵图片后拷贝
				//SpriteManager.conversion(outSrc);
				File.Copy (outSrc,outImg);
				File.Copy (outMetaSrc,outMetaImg);
				// 生成材质 prefab 分离透明通道
				// AtlasManager.GenerateAlpha (outImg);
				// AtlasManager.CreateMaterials (outImg);
				// PrefabUtility.CreatePrefab (prefabPath,imgComponent.gameObject,ReplacePrefabOptions.ReplaceNameBased);
				// UnityEngine.UI.Image fixPrefabObj = AssetDatabase.LoadAssetAtPath (prefabPath,typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
				// fixPrefabObj.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outImg);
				// fixPrefabObj.material = AtlasManager.getMaterForAtlasPath (outImg);
			}
			else
			{
				File.Delete(outImg);
				string metaTxt = outImg + ".meta";
				if(File.Exists (metaTxt))
				{
					File.Delete(metaTxt);
				}
				File.Copy (outSrc,outImg);
				File.Copy (outMetaSrc,outMetaImg);	
			}
			return imgComponent.gameObject;
		}
		// 是否设置九宫格
		if (item.options.isScaleImage == true) {
			/* unity 自带图集sprite情况的九宫格切图
			textureImporter.spriteBorder = new Vector4 (item.options.left, item.options.bottom, item.options.right, item.options.top);
			imgComponent.type = UnityEngine.UI.Image.Type.Sliced;
			// 重新加载
			AssetDatabase.WriteImportSettingsIfDirty (assetPath);
			AssetDatabase.ImportAsset (assetPath);
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
					datas [i].border = new Vector4 (item.options.left, item.options.bottom, item.options.right, item.options.top);
				}
			}
			importer.spritesheet = datas;
			imgComponent.type = UnityEngine.UI.Image.Type.Sliced;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.SaveAndReimport();
		}

		return imgComponent.gameObject;
	}
}
