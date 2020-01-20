using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Runtime.InteropServices;
//using System.Windows.Forms;

public class PSDMenu {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	static void handler(string inputFile,bool isCopyToDoc,bool isOneKey = false,bool isOutPng = false)
	{
		// 文件相对路径获取
		string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";

		string tag = Path.GetFileNameWithoutExtension (inputFile);
		string fileName = tag;
		//文件名中存在下划线，就表示是item
		if(fileName.Contains("_"))
		{
			if(isOneKey == true)return;
			string[] fn = fileName.Split('_');
			fileName = fn[0];
		}
		//检测所有该panel和它的item
		string[] jsonFiles = Directory.GetFiles (baseAssetsDir);
		bool isCheckItem = false;
		foreach (string jsonFile in jsonFiles) 
		{
			if((!jsonFile.Contains(fileName)) || (jsonFile.Contains(".meta"))) continue;
			//过来某些不同模块，但是取差不多相同名的情况，如composePanel 和 facecomposePanel
			string jsonFileName = Path.GetFileNameWithoutExtension(jsonFile);
			if (jsonFileName == fileName) 
				isCheckItem = true; //主面板要做检测是否存在item,将item的图片一起打包
			else 
				isCheckItem = false;
			string[] tmpfn = jsonFileName.Split('_');
			if(tmpfn[0] != fileName) continue;
			// json转模型
			PSDJson psdJson = new PSDJson (jsonFile);
			PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,jsonFile,isCheckItem,isOutPng);
		}

		if(isCopyToDoc)
		{
			PkgAssetbundleController.copyOneABToDoc(fileName);
		}
	}

// 	//[MenuItem("KY工具/PSD2UGUI")]
// 	[MenuItem("KYTool/UI/PSD2UGUI")]
// 	public static void PSDImport()
// 	{
//         //		OpenFileDialog dialog = new OpenFileDialog ();
//         //		dialog.Filter = "json files (*.json)|*.json";
//         ////		dialog.InitialDirectory = "D";
//         //		if (dialog.ShowDialog () == DialogResult.OK) {
//         //			Debug.Log (dialog.FileName);
//         //		}

//         // EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath, "psdoutput");

//         string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
// 		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
// 		{
// 			// 文件相对路径获取
// 			string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";

// 			PSDJson psdJson = new PSDJson (inputFile);
// 			PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile);
// 		}

// //		string[] filters = { "json","*" };
// //		string inputFile2 = EditorUtility.OpenFilePanelWithFilters("Choose",Application.dataPath,filters);
// 		GC.Collect();
// 	}

	//[MenuItem("KY工具/PSD2UGUI")]
	[MenuItem("KYTool/UI/PSD2UGUIII")]
	public static void PSDImportII()
	{
        string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			handler(inputFile,false);
			// // 文件相对路径获取
			// string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";

			// string tag = Path.GetFileNameWithoutExtension (inputFile);
			// string fileName = tag;
			// //文件名中存在下划线，就表示是item
			// if(fileName.Contains("_"))
			// {
			// 	string[] fn = fileName.Split('_');
			// 	fileName = fn[0];
			// }
			// //检测所有该panel和它的item
			// string[] jsonFiles = Directory.GetFiles (baseAssetsDir);
			// bool isCheckItem = false;
			// foreach (string jsonFile in jsonFiles) 
			// {
			// 	if((!jsonFile.Contains(tag)) || (jsonFile.Contains(".meta"))) continue;
			// 	//过来某些不同模块，但是取差不多相同名的情况，如composePanel 和 facecomposePanel
			// 	string jsonFileName = Path.GetFileNameWithoutExtension(jsonFile);
			// 	if (jsonFileName == fileName) 
			// 		isCheckItem = true; //主面板要做检测是否存在item,将item的图片一起打包
			// 	else 
			// 		isCheckItem = false;
			// 	string[] tmpfn = jsonFileName.Split('_');
			// 	if(tmpfn[0] != fileName) continue;
			// 	// json转模型
			// 	PSDJson psdJson = new PSDJson (jsonFile);
			// 	PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,jsonFile,isCheckItem);
			// }
		}
		GC.Collect();
	}

	// [MenuItem("KYTool/UI/PSD2UGUI【一键到DOC】")]
	// public static void PSDImportOneKey()
	// {
	// 	string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
	// 	if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
	// 	{
	// 		// 文件相对路径获取
	// 		string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";

	// 		PSDJson psdJson = new PSDJson (inputFile);
	// 		PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile);
	// 		PkgAssetbundleController.copyOneABToDoc(psdJson.model.name);
	// 	}
	// 	GC.Collect();
	// }

	[MenuItem("KYTool/UI/PSD2UGUI【一键到DOC】II")]
	public static void PSDImportOneKeyII()
	{
		string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			// // 文件相对路径获取
			// string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";
			// PSDJson psdJson = new PSDJson (inputFile);
			// PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile);
			// PkgAssetbundleController.copyOneABToDoc(psdJson.model.name);
			handler(inputFile,true);
		}
		GC.Collect();
	}

	[MenuItem("KYTool/UI/PSD2UGUI【一键到DOC】【图片到outPic】")]
	public static void PSDImportOneKeyIIOut()
	{
		string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			// // 文件相对路径获取
			// string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";
			// PSDJson psdJson = new PSDJson (inputFile);
			// PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile);
			// PkgAssetbundleController.copyOneABToDoc(psdJson.model.name);
			handler(inputFile,true,false,true);
		}
		GC.Collect();
	}

	//[MenuItem("KY工具/拷贝选中文件夹到公共图集")]
	[MenuItem("KYTool/UI/拷贝选中文件夹到公共图集")]
	public static void PSDCopy() {
		/*
		// 选择的文件夹
		var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
		foreach (string path in paths) {
			FileManager.copyDir (path,PSDConst.ATLAS_PATH_COMMON_TEMP);
		}
		*/
		string inputDir = EditorUtility.OpenFolderPanel ("copy file to common",Application.dataPath,"");
		if ((inputDir != null) && (inputDir != "")) {
			// 获取相对路径
			string path = "Assets/" + inputDir.Remove(0, Application.dataPath.Length + 1) + "/";
			FileManager.copyDir (path,PSDConst.ATLAS_PATH_COMMON_TEMP);
		}
	}

	[MenuItem("KYTool/UI/公共图集总生成(带九宫参数)")]
	public static void PSDCommonCreate(bool isCopyToDoc = false) {
		string inputFile = Application.dataPath+"/psd/PSDOutput/common.json";
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			// 文件相对路径获取
			string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";

			// 注意！！！不能删除meta文件！！！不然guid会变化
			File.Delete (PSDConst.GUI_PATH + "common/common.png");
			// File.Delete (PSDConst.GUI_PATH + "common/common.png.meta");
			File.Delete (PSDConst.GUI_PATH + "common/common.tpsheet");
			// File.Delete (PSDConst.GUI_PATH + "common/common.tpsheet.meta");
			File.Delete (PSDConst.GUI_PATH + "common/common_alpha.png");
			// File.Delete (PSDConst.GUI_PATH + "common/common_alpha.png.meta");
			File.Delete (PSDConst.GUI_PATH + "common/common_etc.mat");
			// File.Delete (PSDConst.GUI_PATH + "common/common_etc.mat.meta");
			// File.Delete (PSDConst.GUI_PATH + "common/commonPrefab.prefab");
			// File.Delete (PSDConst.GUI_PATH + "common/commonPrefab.prefab.meta");

			PSDJson psdJson = new PSDJson (inputFile);
			PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile);
			if(isCopyToDoc)
			{
				PkgAssetbundleController.copyOneABToDoc(psdJson.model.name);
			}
			File.Delete (PSDConst.GUI_PATH + "common/commonPrefab.prefab");
			File.Delete (PSDConst.GUI_PATH + "common/commonPrefab.prefab.meta");
		}
		GC.Collect();
	}

	[MenuItem("KYTool/UI/公共图集总生成(带九宫参数)[一键DOC]")]
	public static void PSDCommonCreateOneKey() 
	{
		PSDCommonCreate(true);
	}

	[MenuItem("KYTool/UI/公共图集PNG生成(图标打进图集用)")]
	public static void PSDCommonPNGCreate() {
		string srcPath = PSDConst.ATLAS_PATH_COMMON_TEMP;
		string tarPath = PSDConst.ATLAS_PATH_COMMON;
		bool isSave = AtlasManager.InitAtlasForTextureP(srcPath,tarPath);
	}


		[MenuItem("KYTool/UI/批量打/批量打文件夹下全部UI预设（选文件夹)")]
    public static void PSDImportFolds()
    {
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
        for (int i = 0; i < paths.Count; i++)
        {
            var dirInfo = new DirectoryInfo(paths[i]);
            FileInfo[] files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            for(int j = 0;j < files.Length; j++)
            {
                if (files[j].Name.EndsWith("json"))
                {
                    string tmpName = files[j].FullName.Replace("\\", "/");
                    UnityEngine.Debug.Log(tmpName + "==="+ Application.dataPath+","+ tmpName.StartsWith(Application.dataPath));
                    if ((tmpName != null) && (tmpName != "") && (tmpName.StartsWith(Application.dataPath)))
					{
						// 文件相对路径获取
						// string baseAssetsDir = "Assets/" + Path.GetDirectoryName(tmpName.Remove(0, Application.dataPath.Length + 1)) + "/";
						// PSDJson psdJson = new PSDJson (tmpName);
						// PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,tmpName);
						handler(tmpName,false,true);
					}
                }
            }
        }
        GC.Collect();
    }

   	[MenuItem("KYTool/UI/批量打/批量打UI预设(请选择多个json文件)")]
    public static void PSDImportFolds2()
    {
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToList();
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].EndsWith("json"))
            {
                var dirInfo = new DirectoryInfo(paths[i]);
                string tmpName = dirInfo.FullName.Replace("\\", "/");
                UnityEngine.Debug.Log(tmpName + "===" + Application.dataPath + "," + tmpName.StartsWith(Application.dataPath));
                if ((tmpName != null) && (tmpName != "") && (tmpName.StartsWith(Application.dataPath)))
                {
                    // 文件相对路径获取
                    // string baseAssetsDir = "Assets/" + Path.GetDirectoryName(tmpName.Remove(0, Application.dataPath.Length + 1)) + "/";
                    // PSDJson psdJson = new PSDJson(tmpName);
                    // PSDScene psdScene = new PSDScene(psdJson.model, baseAssetsDir, tmpName);
                    handler(tmpName,false,true);
                }
            }
        }
        GC.Collect();
    }

	[MenuItem("KYTool/UI/公共图集预设组件生成")]
    static void commonItemHandler(bool isCopyToDoc = false)
	{
		string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			// 文件相对路径获取
			string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";
			// json转模型
			PSDJson psdJson = new PSDJson (inputFile);
			PSDScene psdScene = new PSDScene (psdJson.model,baseAssetsDir,inputFile,false);
			if(isCopyToDoc)
			{
				PkgAssetbundleController.copyOneABToDoc("common");
			}
		}
		GC.Collect();
	}

	[MenuItem("KYTool/UI/公共图集预设组件生成[一键DOC]")]
	public static void commonItemHandlerOneKey() 
	{
		commonItemHandler(true);
	}

	[MenuItem("KYTool/UI/回合制主城[一键DOC]")]
	public static void mainCityPkg()
	{
		string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath+"/psd/PSDOutput/", "json");
		if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
		{
			// 文件相对路径获取
			string baseAssetsDir = "Assets/" + Path.GetDirectoryName(inputFile.Remove(0, Application.dataPath.Length + 1)) + "/";
			// json转模型
			PSDJson psdJson = new PSDJson (inputFile);
			PSDSceneMainCity psdScene = new PSDSceneMainCity (psdJson.model,baseAssetsDir,inputFile);
			PkgAssetbundleController.copyOneABToDoc(psdJson.model.name);
		}
		GC.Collect();
	}
}
