using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class PSDSceneMainCity : MonoBehaviour {
	public const string assetsDir = "Assets/";
	public string baseAssetsDir;
	public string baseTmpDir = assetsDir + "TMP/";
	public string inputFile;

	private Canvas canvas;
	private GameObject eventSystem;

	private Model model;


	// 初始化
	public PSDSceneMainCity(Model item,string baseAssetsDir,string inputFile) {
		model = item;
		this.baseAssetsDir = baseAssetsDir;
		this.inputFile = inputFile;
		// 画布构建
		InitCanvas ();
		// 各类控件构建
		InitUI ();
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

		// 拷贝图片到文件目录
		string fileName = Path.GetFileNameWithoutExtension (inputFile);
		for (int i = model.children.Length - 1; i >= 0; i--) 
		{
			copyImgToTmpPath(model.children [i], fileName);
		}

		//========生成组件内容======================
		// 这里缺少层级关系处理
		for (int i = model.children.Length - 1; i >= 0; i--) {
			InitComponentUI (model.children[i],rect.gameObject,fileName);
		}
		//设置文本zoder，同一层文本需要排到上层进行批出来
		setLabelZorder(rect.gameObject);
		AssetDatabase.Refresh();
        checkSavePrefab(rect.gameObject);
        savePrefab(fileName, rect.gameObject,true);
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
			GameObject childObj = ImageComponentMainCity.InitImageComponentMainCity (item, obj, baseAssetsDir, fileName);
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
			for (int i = item.children.Length - 1; i >= 0; i--) {
				InitComponentUI (item.children [i], parentObj, fileName);
			}
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

		if (sprite == null) {
			// 创建一个临时文件夹保存独有图片
			string tmpDir = PSDConst.GUI_PATH + fileName;
			string singleImg = baseAssetsDir + rootPath + "/" + imgPath;
			string tarSingleImg = tmpDir + "/" + imgPath;

			if (!Directory.Exists (tmpDir)) {
				Directory.CreateDirectory (tmpDir);
			}

			if ((!File.Exists (tarSingleImg)) && (!Directory.Exists(tarSingleImg))) {
				File.Copy (singleImg,tarSingleImg);

				// 记得要调用，否则AssetDatabase.LoadAssetAtPath取不到
				AssetDatabase.Refresh();
			}
		}
	}
}
