using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class PSDScene : MonoBehaviour {
	public const string assetsDir = "Assets/";
	public string baseAssetsDir;
	public string baseTmpDir = assetsDir + "TMP/";
	public string inputFile;
	public bool isOutPng;

	private Canvas canvas;
	private GameObject eventSystem;

	private Model model;
	private bool _isCheckItem;
//	private GameObject childObj;
	//private GameObject parentObj;


	// 初始化
	public PSDScene(Model item,string baseAssetsDir,string inputFile,bool isCheckItem = true,bool isOutPng = false) {
		model = item;
		_isCheckItem = isCheckItem;
		this.baseAssetsDir = baseAssetsDir;
		this.inputFile = inputFile;
		this.isOutPng = isOutPng;
		// 画布构建
		InitCanvas ();
		// UI目录层级构建
		UIDirectoryCreate ();
		// 公共图片转公共图集
		//CommonImgParse();
		// texture公共图集【复制公共图集】  //不需要了，单独拆出了脚本来执行
		// CommonImgParseForTexture();
		// 各类控件构建
		InitUI ();
		// 公共图集预设构建
		//CommonPrefabCreate();
		// 删除临时文件夹
		if (Directory.Exists (baseTmpDir)) {
			Directory.Delete(baseTmpDir,true);
		}
	}

	// 画布构建
	public void InitCanvas() {
	#if UNITY_5_2
		EditorApplication.NewScene();
	#elif UNITY_5_3
		EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
	#endif
		//Debug.Log ("###Canvas Loading:"+PSDConst.ASSET_PATH_CANVAS);
		// 加载画布prefab
		Canvas temp = AssetDatabase.LoadAssetAtPath(PSDConst.ASSET_PATH_CANVAS, typeof(Canvas)) as Canvas;
		// 实例化显示prefab（要另外用个对象保存避免被释放）
		canvas = (Canvas)GameObject.Instantiate (temp);
		// 设置画布投影模式 
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		
		// 获取canvas画布下Scaler这个组件,修改属性设置（UI相关东西通过UnityEngine.UI去获取）
		UnityEngine.UI.CanvasScaler scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
		// 根据参考分辨率和屏幕分辨率设置画布最终大小
		scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.referenceResolution = new Vector2 (model.options.width,model.options.height);

		// 构建EventSystem
		GameObject eventSys = AssetDatabase.LoadAssetAtPath(PSDConst.ASSET_PATH_EVENTSYSTEM,typeof(GameObject)) as GameObject;
		eventSystem = GameObject.Instantiate (eventSys) as GameObject;
	}

	// UI目录层级构建
	public void UIDirectoryCreate() {
		if (model.classname != null) {
			
		}
	}

	// 控件构建
	public void InitUI() {
		// 根据数组长度判断是否直接构建控件
		InitBaseUI();
	}

	public void InitBaseUI() {
		// 利用RectTransform来构建目录结构
		RectTransform rect = PSDPrefab.PrefabInstant<RectTransform>(PSDConst.ASSET_PATH_RECTTRANSFORM,model.name+"Prefab",canvas.gameObject);

		// 总预设锚点再左上角(父锚点，锚点)
		Vector2 anchorP = PSDConst.PIVOTPOINT1;
		rect.anchorMin = anchorP;
		rect.anchorMax = anchorP;
		rect.pivot = anchorP; 

		rect.offsetMin = new Vector2 (0.0f,0.0f);
		rect.offsetMax = new Vector2 (0.0f,0.0f);
		rect.sizeDelta = new Vector2 (model.options.width,model.options.height);

		//======拷贝图片资源===============
		string tag = Path.GetFileNameWithoutExtension (inputFile);
		string fileName = tag;
		//文件名中存在下划线，就表示是item
		if(fileName.Contains("_"))
		{
			string[] fn = fileName.Split('_');
			fileName = fn[0];
		}
		//界面+item一起打包时，只要界面检测和打包一次图集就可以，item只要打包预设，不用重新执行一遍
		if(_isCheckItem)
		{
			//检测所有该panel和它的item
			string[] jsonFiles = Directory.GetFiles (baseAssetsDir);
			foreach (string jsonFile in jsonFiles) 
			{
				if((!jsonFile.Contains(tag)) || (jsonFile.Contains(".meta"))) continue;
				//过来某些不同模块，但是取差不多相同名的情况，如composePanel 和 facecomposePanel
				string jsonFileName = Path.GetFileNameWithoutExtension(jsonFile);
				string[] tmpfn = jsonFileName.Split('_');
				if(tmpfn[0] != fileName) continue;

				Debug.Log("=====拷贝图片的json文件:"+jsonFile);
				// json转模型
				PSDJson psdJson = new PSDJson (jsonFile);
				// 拷贝图片到临时指定目录
				for (int i = psdJson.model.children.Length - 1; i >= 0; i--) 
				{
					// 拷贝图片到临时目录
					copyImgToTmpPath (psdJson.model.children [i], fileName);
				}
			}
			if(this.isOutPng)
			{
				// 拷贝到outpic
				copyImgToOutPathFromPNGExport(model.name);
			}
			else
			{
				// 拷贝到图集
	            copyImgToTmpPathFromPNGExport(model.name);
			}

            //========将临时的图集目录打包================
            string tmpAtlasPath = baseTmpDir + "Atlas";
			if (Directory.Exists (tmpAtlasPath)) {
				string[] tmpAtlasChildDirs = Directory.GetDirectories (tmpAtlasPath);
				foreach (string dir in tmpAtlasChildDirs) {
					// if(dir.Contains("_")) continue;
					string tmpTag = Path.GetFileName(dir);
					string tarPath = PSDConst.GUI_PATH + tmpTag + "/" + tmpTag + PSDConst.PNG_SUFFIX;
					Debug.Log("====打包图集:" + tmpTag + "," + tarPath);
					AtlasManager.InitAtlasForTextureP(dir,tarPath);
				}
			}
		}
		
		//========生成组件内容======================
		// 这里缺少层级关系处理
		for (int i = model.children.Length - 1; i >= 0; i--) {
			InitComponentUI (model.children[i],rect.gameObject,fileName);
		}

		//设置文本zoder，同一层文本需要排到上层进行批出来
		setLabelZorder(rect.gameObject);

		AssetDatabase.Refresh ();


		// string componentPath = PSDConst.GUI_PATH + model.name + "/";
		// string prefabPath = componentPath + model.name+"Prefab" + PSDConst.PREFAB_SUFFIX;
		// if (!Directory.Exists(PSDConst.GUI_PATH)) {Directory.CreateDirectory (PSDConst.GUI_PATH);}
		// if (!Directory.Exists(componentPath)) {Directory.CreateDirectory (componentPath);}
		// PrefabUtility.CreatePrefab (prefabPath,rect.gameObject,ReplacePrefabOptions.ReplaceNameBased);

        checkSavePrefab(rect.gameObject);
        savePrefab(fileName, rect.gameObject,true);
        // savePrefab(model.name, rect.gameObject,true);
    }

    private void copyImgToTmpPathFromPNGExport(string rootName)
    {
        string pngExportPath = baseAssetsDir + rootName + "/PNGExport";
        string tmpDir = baseTmpDir + "Atlas/" + rootName;
        if (Directory.Exists(pngExportPath))
        {
            FileManager.copyDir(pngExportPath, tmpDir);
        }
    }

    private void copyImgToOutPathFromPNGExport(string rootName)
    {
        string outExportPath = baseAssetsDir + rootName + "/PNGExport";
        if (Directory.Exists(outExportPath))
        {
            FileManager.copyDir(outExportPath, PSDConst.OUT_PATH);
        }
    }

    /** 保存预设
	 * 
	 * 1.创建UI文件夹（判断）
	 * 2.根据名称创建目录结构
	 * 3.保存到指定名称的目录结构下
	*/
    public void savePrefab(string fileName,GameObject gameobj,bool needPrefabPath = false)
	{
		string componentPath = PSDConst.GUI_PATH + fileName + "/";
		string name = model.name;
		string prefabPath = componentPath + name + PSDConst.PREFAB_SUFFIX;
		if(needPrefabPath == true)
		{
			prefabPath = componentPath + name+"Prefab" + PSDConst.PREFAB_SUFFIX;
		}
		if (!Directory.Exists(PSDConst.GUI_PATH)) {Directory.CreateDirectory (PSDConst.GUI_PATH);}
		if (!Directory.Exists(componentPath)) {Directory.CreateDirectory (componentPath);}
        var a = (gameobj.transform as RectTransform).anchoredPosition.x;
        var b = (gameobj.transform as RectTransform).anchoredPosition.y;
        Debug.Log("=======保存prefab:"+prefabPath);
        PrefabUtility.CreatePrefab (prefabPath, gameobj, ReplacePrefabOptions.ReplaceNameBased);
    }

	public void checkSavePrefab(GameObject obj)
	{
		for (int i = model.children.Length - 1; i >= 0; i--) {
			initSavePrefab (model.children[i], obj);
		}
	}

	public void initSavePrefab(Children item,GameObject parentObj)
	{
		// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split('\r');
		var prop = new Property ();
		prop.DeserializeProperty (props);
		if (prop.isHidden) {
			return;
		}
		// Debug.Log("预设名 == "+item.name);
        
		if (parentObj.transform.Find(item.name) != null && prop.isSavePrefab) {
			GameObject curItem = parentObj.transform.Find(item.name).gameObject;
			// Debug.Log("保存预设名 == "+item.name);
			savePrefab2(item.name, curItem);
            DestroyImmediate(curItem);
			return;
		}

		if ((item.children.Length > 0) && (parentObj != null)) 
		{
			for (int i = item.children.Length - 1; i >= 0; i--) 
			{
				initSavePrefab (item.children [i], parentObj);
			}
		}
	}

	public void savePrefab2(string name,GameObject gameobj,bool needPrefabPath = false)
	{
		string componentPath = PSDConst.GUI_PATH + model.name + "/";
		string prefabPath = componentPath + name + PSDConst.PREFAB_SUFFIX;
		if(needPrefabPath == true)
		{
			prefabPath = componentPath + name+"Prefab" + PSDConst.PREFAB_SUFFIX;
		}
		if (!Directory.Exists(PSDConst.GUI_PATH)) {Directory.CreateDirectory (PSDConst.GUI_PATH);}
		if (!Directory.Exists(componentPath)) {Directory.CreateDirectory (componentPath);}
		Debug.Log("=============11111111112222222222222222222============");
        var a = (gameobj.transform as RectTransform).anchoredPosition.x;
        var b = (gameobj.transform as RectTransform).anchoredPosition.y;
        PrefabUtility.CreatePrefab (prefabPath, gameobj, ReplacePrefabOptions.ReplaceNameBased);
    }

	public void setLabelZorder(GameObject obj)
	{
		UnityEngine.UI.Text[] textLists = obj.GetComponentsInChildren<UnityEngine.UI.Text>();
		foreach (UnityEngine.UI.Text t in textLists)
		{
			GameObject a = t.gameObject;
			// Debug.Log("gameObj名字 = "+a.name+","+a.transform.GetSiblingIndex());
			a.transform.SetAsLastSibling();
		}
	}

	public void  InitComponentUI(Children item,GameObject obj,string fileName) {
		// 获取属性
		var propStr = item.options.property;
		var props = propStr.Split('\r');
		var prop = new Property ();
		prop.DeserializeProperty (props);
		GameObject parentObj = null;

		if (prop.isHidden) {
			return;
		}

		if ((item.classname == "LABEL") && (!prop.isInput)) {
			GameObject childObj = TextComponent.InitTextComponent (item, obj, baseAssetsDir);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if ((item.classname == "LABEL") && (prop.isInput)) {
			GameObject childObj = TextFieldComponent.InitTextFieldComponent (item, obj, baseAssetsDir);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if (item.classname == "IMAGE") {
			GameObject childObj = ImageComponent.InitImageComponent (item, obj, baseAssetsDir, fileName);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if (item.classname == "BUTTON") {
			GameObject childObj = ButtonComponent.InitButtonComponent (item, obj, baseAssetsDir, fileName);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if (item.classname == "GROUP") {
			GameObject childObj = RectComponent.InitRectComponent (item, obj, baseAssetsDir);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if (item.classname == "PROGRESSBAR") {
			GameObject childObj = ProgressComponent.InitProgressComponent (item,obj,baseAssetsDir,fileName);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		} else if (item.classname == "LIST") {
			GameObject childObj = ScrollComponent.InitComponent (item,obj,baseAssetsDir);
			if (item.children.Length > 0) {
				parentObj = childObj;
			}
		}

		if ((item.children.Length > 0) && (parentObj != null)) {
			//Debug.Log ("child:"+item.name+"<<>>"+item.children.Length);
			for (int i = item.children.Length - 1; i >= 0; i--) {
				//Debug.Log ("parentName:"+parentObj.name+"--itemName:"+item.name+"--childrenName:"+item.children[i].name);
				InitComponentUI (item.children [i], parentObj, fileName);
			}
		}
	}

	// 公共图片转公共图集(unity 自带的sprite打包图集)
	public void CommonImgParse() {
//		Debug.Log ("###common：" + baseAssetsDir);
		string commonDir = baseAssetsDir + "common/";
		bool isSprite = false;
		if (!Directory.Exists (commonDir)) {
			return;
		}

		string atlasTag = "common";
		string[] imgType = {"*.png","*.jpg"};
		string[] imgPaths = Directory.GetFiles (commonDir,"*.png",SearchOption.AllDirectories);
//		Debug.Log (commonDir);
//		Debug.Log ("###common：" + imgs.Length);
		foreach (string imgPath in imgPaths) {
//			Debug.Log ("###common：" + img);
			TextureImporter texture = AssetImporter.GetAtPath (imgPath) as TextureImporter;
			if (texture.textureType == TextureImporterType.Sprite) {
				continue;
			}
			texture = AtlasManager.InitAtlas(imgPath,atlasTag);
			Sprite sprite = AssetDatabase.LoadAssetAtPath (imgPath,typeof(Sprite)) as Sprite;
		}
	}

	// Texture公共图集
	public void CommonImgParseForTexture() {
		string srcPath = baseAssetsDir + "common/";
		//先删除旧的AtlasCommon
		// if (Directory.Exists(PSDConst.ATLAS_PATH_COMMON_TEMP))
  //       {
  //           Directory.Delete(PSDConst.ATLAS_PATH_COMMON_TEMP,true);
  //           AssetDatabase.Refresh();
  //       }
		FileManager.copyDir (srcPath,PSDConst.ATLAS_PATH_COMMON_TEMP);;
		srcPath = PSDConst.ATLAS_PATH_COMMON_TEMP;
		string tarPath = PSDConst.ATLAS_PATH_COMMON;
		bool isSave = AtlasManager.InitAtlasForTextureP (srcPath,tarPath);
		//PrefabUtility.CreatePrefab (prefabPath,imgComponent.gameObject,ReplacePrefabOptions.ReplaceNameBased);
	}

	// 面板独有图集
	private void InitAtlasForPannel(Children item) {
		string[] propertys = {""};
		var prop = new Property ();

		Sprite sprite = null;
		string[] link_split = {""};
		string rootPath;
		string imgPath;
		string prefix;
		string imgName;
		string atlasName;
		if (item.classname == "IMAGE") {
			link_split = item.options.link.Split ('#');
			propertys = item.options.property.Split ('\r');
		} else if (item.classname == "BUTTON") {
			link_split = item.options.u3dLink.options.link.Split ('#');
			propertys = item.options.u3dLink.options.property.Split ('\r');
		}
		prop.DeserializeProperty (propertys);
	
		rootPath = (link_split.Length > 0) ? (link_split[0]) : "";
		imgPath = (link_split.Length > 0) ? link_split[link_split.Length - 1] : "";
		prefix = (imgPath.Split('/').Length > 1) ? imgPath.Split('/')[0] : "";
		imgName = (imgPath.Split('/').Length > 0) ? imgPath.Split('/')[imgPath.Split('/').Length - 1] : "";
		imgName = Path.GetFileNameWithoutExtension (imgName);
		atlasName = (prefix != "") ? (prefix + "-" + imgName) : (imgName);

		// 如果是外部图片或者没有图片直接返回
		if ((prop.isOutpic > 0) || (imgPath == "")) {
			return;
		}

		//Debug.Log ("###root:"+rootPath);

		// 先到公共图集中查询是否有该图片
		if (File.Exists (PSDConst.ATLAS_PATH_COMMON)) {
			sprite = AtlasManager.getSpriteForTextureP (PSDConst.ATLAS_PATH_COMMON, atlasName);
		}
		if (sprite == null) {
			// 创建一个临时文件夹保存独有图片
			string tmpDir = baseTmpDir + "Atlas/" +rootPath;
			string singleImg = baseAssetsDir + rootPath + "/" + imgPath;
			string tarSingleImg = tmpDir + "/" + imgPath;
			string saveDir = Directory.GetParent (tarSingleImg).FullName;
			//Debug.Log (saveDir);
			if (!Directory.Exists (saveDir)) {
				Directory.CreateDirectory (saveDir);
			}
			// 如果是tabview需要把item的图片也保存到这里
			if (!File.Exists (tarSingleImg)) {
				File.Copy (singleImg,tarSingleImg);
			}
			// 根据临时文件夹初始化图集
			string tarPath = PSDConst.GUI_PATH + rootPath + '/' + rootPath + PSDConst.PNG_SUFFIX;
			AtlasManager.InitAtlasForTextureP(tmpDir,tarPath);
		}

	}

	private void copyImgToTmpPath (Children item,string fileName) {
		string[] propertys = {""};
		var prop = new Property ();

		Sprite sprite = null;
		string[] link_split = {""};
		string rootPath;
		string imgPath;
		string prefix;
		string imgName;
		string atlasName;
		if (item.classname == "IMAGE") {
			link_split = item.options.link.Split ('#');
			propertys = item.options.property.Split ('\r');
		} else if (item.classname == "BUTTON") {
			link_split = item.options.u3dLink.options.link.Split ('#');
			propertys = item.options.u3dLink.options.property.Split ('\r');
		} else if (item.classname == "PROGRESSBAR") {
			link_split = item.options.bar.options.link.Split ('#');
			propertys = item.options.bar.options.property.Split ('\r');
		}
		prop.DeserializeProperty (propertys);

		rootPath = (link_split.Length > 0) ? (link_split[0]) : "";
		imgPath = (link_split.Length > 0) ? link_split[link_split.Length - 1] : "";
		prefix = (imgPath.Split('/').Length > 1) ? imgPath.Split('/')[0] : "";
		imgName = (imgPath.Split('/').Length > 0) ? imgPath.Split('/')[imgPath.Split('/').Length - 1] : "";
		imgName = Path.GetFileNameWithoutExtension (imgName);
		atlasName = (prefix != "") ? (prefix + "-" + imgName) : (imgName);

		//Debug.Log ("###imgPath:,"+imgPath+"，是否外部,"+prop.isOutpic);

		// 如果是外部图片或者没有图片直接返回
		if (prop.isOutpic > 0) {
			return;
		}

		if (imgPath == "") {
			if (item.children.Length > 0) {
				for (int i = item.children.Length - 1; i >= 0; i--) {
					if (item.children.Length == 0) {continue;}
					copyImgToTmpPath (item.children[i],fileName);
				}
			}
		}

		if (imgPath == "") {
			return;
		}
		//判断公共图集是否已村子
		// 先到公共图集中查询是否有该图片
		if (File.Exists (PSDConst.ATLAS_PATH_COMMON)) {
			sprite = AtlasManager.getSpriteForTextureP (PSDConst.ATLAS_PATH_COMMON, atlasName);
		}
		if (sprite == null) {
			// 创建一个临时文件夹保存独有图片
			string tmpDir = baseTmpDir + "Atlas/" + rootPath;
			string singleImg = baseAssetsDir + rootPath + "/" + imgPath;
			string tarSingleImg = tmpDir + "/" + imgPath;
			string saveDir = Directory.GetParent (tarSingleImg).FullName;

			if (fileName != "") {
				tarSingleImg = baseTmpDir + "Atlas/" + fileName + "/" + imgPath;
				saveDir = Directory.GetParent (tarSingleImg).FullName;
			}

			//Debug.Log ("###tarImg:"+tarSingleImg);
			//Debug.Log("<<<imgPath:"+imgPath+">>>savedir:"+saveDir);

			if (!Directory.Exists (saveDir)) {
				Directory.CreateDirectory (saveDir);
			}
			// 如果是tabview需要把item的图片也保存到这里
			if ((!File.Exists (tarSingleImg)) && (!Directory.Exists(tarSingleImg))) {
				File.Copy (singleImg,tarSingleImg);
			}
		}
	}

	public void CommonPrefabCreate() {
		// UnityEngine.UI.Image obj = null;
		// string srcFile = PSDConst.ASSET_PATH_COMMON;
		
		// string tarFile = tarDir + Path.GetFileName(srcFile);
		// if (!File.Exists(tarFile)) {
		// 	File.Copy (srcFile,tarFile);
		// }
		// GameObject go = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject> (tarFile);
		// obj = go.GetComponent<GameLib.ui.UIImage>();
		// obj.material = AtlasManager.getMaterForAtlasPath(PSDConst.ATLAS_PATH_COMMON);
		// EditorUtility.SetDirty (obj);

		string tarDir = PSDConst.GUI_PATH + "common/common.prefab";
		GameObject go = new GameObject("common");
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = PSDConst.PIVOTPOINT1;
        rect.anchorMax = PSDConst.PIVOTPOINT1;
        rect.pivot = PSDConst.PIVOTPOINT1;
        UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
        image.material = AtlasManager.getMaterForAtlasPath(PSDConst.ATLAS_PATH_COMMON);
        go.SetActive(false);
        PrefabUtility.CreatePrefab(tarDir, go, ReplacePrefabOptions.ReplaceNameBased);
        DestroyImmediate(go, true);
        AssetDatabase.SaveAssets();
	}

//	// prefab实例化方法
//	public T PrefabInstant<T>(string path,string name,GameObject parent) where T:UnityEngine.Object {
//		GameObject temp = AssetDatabase.LoadAssetAtPath (path,typeof(GameObject)) as GameObject;
//		GameObject item = GameObject.Instantiate (temp) as GameObject;
//		if (item == null) {
//			Debug.LogError ("prefab instance failed:"+path);
//			return null;
//		}
//		item.name = name;
//		item.transform.SetParent (canvas.transform,false);
////		item.transform.localScale = Vector3.one;
//		return item.GetComponent<T> ();
//	}
}
