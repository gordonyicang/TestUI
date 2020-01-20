using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using UnityEngine.UI;

public class TexturePackerBuild : Editor
{
    private const string BuildAllFolder = "KYTool/图集打包/打包图集";
    private const string BuildSelect = "KYTool/图集打包/打包选中图集（当前目录下图片打包）";
    private const string BuildAllChildren = "KYTool/图集打包/打包选中文件夹";
    private const string BuildAllChildrenNoTrim = "KYTool/图集打包/打包选中文件夹(包括透明区域)";

    /// <summary>
    /// 输出目录
    /// </summary>
    private const string OutPutDirRoot = "Assets/Resources/Atlas/";
    /// <summary>
    /// 小图目录
    /// </summary>
    private const string OriginalDir = "Assets/Atlas/";
    private static string language = "cn";

    private static Boolean Split_Alpha = true;
    /// <summary>
    /// TexturePacker的安装目录
    /// </summary>
    //private const string TPInstallDir = "C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe";
	private const string TPInstallDir = "C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe";


    private static string commandText = " --sheet {0}.png --data {1}.tpsheet --format unity-texture2d --trim-mode CropKeepPos  --trim-threshold 6 --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT  --disable-rotation --scale 1 {2} --force-squared";

    private static string commandTextNoTrim = " --sheet {0}.png --data {1}.tpsheet --format unity-texture2d --trim-mode None --max-size 2048 --size-constraints AnySize --allow-free-size --disable-rotation --scale 1 {2} --force-squared";

    private static string PROCESS_MATCH = "nothing to do";

    // [MenuItem(BuildAllFolder)]
    public static void BuildAllTP()
    {
        string inputPath = OriginalDir + language + "/";
        var dir = new DirectoryInfo(inputPath);
        BuildFold(dir);
    }

    [MenuItem(BuildAllChildren)]
    public static void BuildAllChildrenPic()
    {
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
        for (int i = 0; i < paths.Count; i++)
        {
            var dirInfo = new DirectoryInfo(paths[i]);
            FileInfo[] images = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            string sheetName = getSheetName(dirInfo);
            bool hasChange = processCommand(TPInstallDir, string.Format(commandText, sheetName, sheetName, dirInfo.FullName));
            if (hasChange)
            {
                if (Split_Alpha) GenerateAlpha(sheetName);
                CreateMaterials(sheetName + ".png");
                CreatePrefabs(sheetName + ".png");
            }
        }
    }

    [MenuItem(BuildAllChildrenNoTrim)]
    public static void BuildAllChildrenPicNoTrim()
    {
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
        for (int i = 0; i < paths.Count; i++)
        {
            var dirInfo = new DirectoryInfo(paths[i]);
            FileInfo[] images = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            string sheetName = getSheetName(dirInfo);
            bool hasChange = processCommand(TPInstallDir, string.Format(commandTextNoTrim, sheetName, sheetName, dirInfo.FullName));
            if (hasChange)
            {
                if (Split_Alpha) GenerateAlpha(sheetName);
                CreateMaterials(sheetName + ".png");
                CreatePrefabs(sheetName + ".png");
            }
        }
    }

    private static void BuildFold(DirectoryInfo dirInfo)
    {
        DirectoryInfo[] dirList = dirInfo.GetDirectories();
        if (dirList.Length > 0)
        {
            for (var i = 0; i < dirList.Length; ++i)
            {
                DirectoryInfo subFolderInfo = dirList[i];
                BuildFold(subFolderInfo);
            }
        }
        else
        {
            FileInfo[] images = dirInfo.GetFiles("*");
            StringBuilder sb = new StringBuilder("");
            GetImageName(images, ref sb);
            string sheetName = getSheetName(dirInfo);
            UnityEngine.Debug.Log("打包地址 = "+sheetName);
            // UnityEngine.Debug.Log(string.Format(commandText, sheetName, sheetName, sb.ToString()));
            bool hasChange = processCommand(TPInstallDir, string.Format(commandText, sheetName, sheetName, sb.ToString()));
            if (hasChange)
            {
               if (Split_Alpha) GenerateAlpha(sheetName);
               CreateMaterials(sheetName + ".png");
               CreatePrefabs(sheetName + ".png");
            }
        }
    }

    private static string getSheetName(DirectoryInfo dirInfo)
    {
        // string outPut = OutPutDirRoot;
        // string fullPath = dirInfo.FullName.Replace('\\', '/');
        // int index = fullPath.IndexOf(language);
        // string tmpPath = fullPath.Substring(index + language.Length + 1);
        // UnityEngine.Debug.Log(index + "  ：" + fullPath + "  -" + tmpPath);
        // outPut += tmpPath + "/" + dirInfo.Name;
        // return outPut;
        string outPut = OutPutDirRoot;
        var name1 = dirInfo.Name;
        outPut += name1 + "/" + name1;
        return outPut;
    }

    // [MenuItem(BuildSelect, false)]
    public static void BuildSelectTP()
    {
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
        for (int i = 0; i < paths.Count; i++)
        {
            var dir = new DirectoryInfo(paths[i]);
            BuildFold(dir);
        }
    }

    private static StringBuilder GetImageName(FileInfo[] fileName, ref StringBuilder sb)
    {
        for (int j = 0; j < fileName.Length; j++)
        {
            string extenstion = fileName[j].Extension;
            if (extenstion == ".png")
            {
                sb.Append(fileName[j]);
                sb.Append("  ");
            }
        }
        return sb;
    }

    //[MenuItem("KYTool/TexturePacker/保存九宫", false)]
    private static void SaveBorder()
    {
		// 使用AssetImporter加载图集
		TextureImporter textureImporter = AssetImporter.GetAtPath("Assets/Resources/Atlas/Athletics/Athletics.png") as TextureImporter;
        // 设置可读属性
		textureImporter.isReadable = true;
		// 获取图集下的所有小图数据
        SpriteMetaData[] spriteMetaDatas = textureImporter.spritesheet;
		// 修改九宫格边界
        for (int i = 0; i < spriteMetaDatas.Length;i ++)
        {
            UnityEngine.Debug.Log(spriteMetaDatas[i].name + "," + spriteMetaDatas[i].border);
			if (spriteMetaDatas[i].name == "name6")
            {
                spriteMetaDatas[i].border = new Vector4(2,3,4,5);
            }
        }
        textureImporter.spritesheet = spriteMetaDatas;
    }

    //带
    private static StringBuilder GetSubImageName(FileInfo[] fileName, ref StringBuilder sb)
    {
        for (int j = 0; j < fileName.Length; j++)
        {
            string extenstion = fileName[j].Extension;
            if (extenstion == ".png")
            {
                sb.Append(fileName[j]);
                sb.Append("  ");
            }
        }
        return sb;
    }

    private static bool processCommand(string command, string argument)
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = false;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
        Process p = Process.Start(start);
        string outPut = "";
        if (!start.UseShellExecute)
        {
            outPut = p.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log("###progress output:"+outPut);
			UnityEngine.Debug.Log("###progress standardE:"+p.StandardError.ReadToEnd());
        }
        p.WaitForExit();
        p.Close();
        AssetDatabase.Refresh();

        bool hasChange = outPut.IndexOf(PROCESS_MATCH) == -1;
        return hasChange;
        // BuildTexturePacker();
        // ClearOtherFiles();
        // AssetDatabase.Refresh();
    }

    private static void GenerateAlpha(string path)
    {
        ImageChannelSpliterWrapper.Execute(path);
        //ImportAsset(path + "_alpha.png");
    }

    private static void ImportAsset(string assetPath)
    {
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUncompressedImport);
    }

    private static void CreateMaterials(string path)
    {
            Shader shader = AssetDatabase.LoadAssetAtPath("Assets/Resources/Shader/PSD2UGUI/PSD2UGUI_SPLIT_ALPHA.Shader", typeof(Shader)) as Shader;
            Material material = new Material(shader);
            Texture2D texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            Texture2D alphaTexture = AssetDatabase.LoadAssetAtPath(GetAlphaPngPath(path), typeof(Texture2D)) as Texture2D;
            material.SetTexture("_MainTex", texture);
            if (Split_Alpha)
            {
                material.SetTexture("_AlphaTex", alphaTexture);
            }
            AssetDatabase.CreateAsset(material, path.Replace(".png", "_etc.mat"));
            AssetDatabase.SaveAssets();
    }

    private static void CreatePrefabs(string path)
    {
            GameObject go = new GameObject("AtlasSprite");
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            Image image = go.AddComponent<Image>();
            image.material = AssetDatabase.LoadAssetAtPath(GetMaterialPath(path), typeof(Material)) as Material;
            string prefabPath = GetPrefabPath(path);
            go.SetActive(false);
            PrefabUtility.CreatePrefab(GetPrefabPath(path), go, ReplacePrefabOptions.ReplaceNameBased);
            DestroyImmediate(go, true);
            AssetDatabase.SaveAssets();
    }

    private static string GetMaterialPath(string pngPath)
    {
        return pngPath.Replace(".png", "_etc.mat");
    }

    private static string GetPrefabPath(string pngPath)
    {
        return pngPath.Replace(".png", ".prefab");
    }

    private static string GetAlphaPngPath(string pngPath)
    {
        return pngPath.Replace(".png", "_alpha.png");
    }
}
