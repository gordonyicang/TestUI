using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Security;
using System.Collections.Generic;
using LuaFramework;
using System.Text;
using System;

public class PkgAssetbundleController : Editor
{
    private static string isHaveNotWriteableMapFbx = "";
    private static BuildTarget currentTarget = BuildTarget.StandaloneWindows;
    private static string targetePath = "";

    //[MenuItem("KYTool/Lua/打包lib代码")]
    static void CreatLibAB()
    {
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = Application.dataPath + "/../../tools/libscode.bat";
        proc.StartInfo.CreateNoWindow = false;
        proc.Start();
        proc.WaitForExit();
        AssetDatabase.Refresh();
        string luaPath = Application.dataPath + "/lua/sourcelibs.txt";
        if (File.Exists(luaPath))
        {
            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            string[] list = new string[1];
            list[0] = "Assets/lua/sourcelibs.txt";
            builds[0].assetNames = list;
            builds[0].assetBundleName = "sourcelibs";
            BuildPipeline.BuildAssetBundles(Application.dataPath + "/StreamingAssets", builds, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            AssetDatabase.Refresh();
            File.Delete(luaPath);
            EditorUtility.DisplayDialog("完成", "sourcelibs打包成功！", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "找不到sourcelibs.txt文件！", "确定");
        }
    }
    //[MenuItem("KYTool/Lua/打包全部lua代码(win)")]
    static void CreateAllLuaAb_win()
    {
        CreateAllLuaAb(BuildTarget.StandaloneWindows);
    }
    //[MenuItem("KYTool/Lua/打包全部lua代码(android)")]
    static void CreateAllLuaAb_android()
    {
        CreateAllLuaAb(BuildTarget.Android);
    }
    //[MenuItem("KYTool/Lua/打包全部lua代码(ios)")]
    static void CreateAllLuaAb_ios()
    {
        CreateAllLuaAb(BuildTarget.iOS);
    }

    static void CreateAllLuaAb(BuildTarget target)
    {
        if (!Directory.Exists(Application.streamingAssetsPath + "/gamelua"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/gamelua");
        }
        string resLuaPath = Application.dataPath + "/lua/Game.lua";
        string tagLuaPath = Application.streamingAssetsPath + "/gamelua/Game.lua";
        if (File.Exists(tagLuaPath))
        {
            File.Delete(tagLuaPath);
        }
        File.Copy(resLuaPath, tagLuaPath);
        resLuaPath = Application.dataPath + "/lua/GameConfig.lua";
        tagLuaPath = Application.streamingAssetsPath + "/gamelua/GameConfig.lua";
        if (File.Exists(tagLuaPath))
        {
            File.Delete(tagLuaPath);
        }
        File.Copy(resLuaPath, tagLuaPath);
        resLuaPath = Application.dataPath + "/lua/GameScene.lua";
        tagLuaPath = Application.streamingAssetsPath + "/gamelua/GameScene.lua";
        if (File.Exists(tagLuaPath))
        {
            File.Delete(tagLuaPath);
        }
        File.Copy(resLuaPath, tagLuaPath);

        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = Application.dataPath + "/../../tools/mixcodeLua.bat";
        proc.StartInfo.CreateNoWindow = false;
        proc.Start();
        proc.WaitForExit();
        AssetDatabase.Refresh();
        string luaPath = Application.dataPath + "/lua/targetLuaFile.txt";
        if (File.Exists(luaPath))
        {
            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            string[] list = new string[1];
            list[0] = "Assets/lua/targetLuaFile.txt";
            builds[0].assetNames = list;
            builds[0].assetBundleName = "luafile";
            BuildPipeline.BuildAssetBundles(Application.dataPath + "/StreamingAssets", builds, BuildAssetBundleOptions.None, target);

            AssetDatabase.Refresh();
            File.Delete(luaPath);

            EditorUtility.DisplayDialog("完成", "luafile打包成功！", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "找不到targetLuaFile.txt文件！", "确定");
        }
    }

    [MenuItem("KYTool/AssetBundle/打包/win资源+地图数据")]
    static void CreatAllAB()
    {
        currentTarget = BuildTarget.StandaloneWindows;
        targetePath = Application.dataPath + "/StreamingAssets";
        copyMapDataToWin();
        CreateAB();
    }
    [MenuItem("KYTool/AssetBundle/打包/android资源+地图数据")]
    static void CreatAllAB_android()
    {
        currentTarget = BuildTarget.Android;
        targetePath = Application.dataPath + "/StreamingAssets_android";
        copyMapDataToAndroid();
        CreateAB();
    }
    [MenuItem("KYTool/AssetBundle/打包/ios资源+地图数据")]
    static void CreatAllAB_ios()
    {
        currentTarget = BuildTarget.iOS;
        targetePath = Application.dataPath + "/StreamingAssets_ios";
        copyMapDataToIos();
        CreateAB();
    }

    static void CreateAB(bool isShowAlert = true)
    {
        // clearUnityEditorCode(true);
        BuildPipeline.BuildAssetBundles(targetePath, BuildAssetBundleOptions.None, currentTarget);
        if (isShowAlert)
        {
            EditorUtility.DisplayDialog("完成", "全部资源打包成功！", "确定");
        }
        // clearUnityEditorCode(false);
    }

    [MenuItem("KYTool/AssetBundle/设置AB名/【自动】设置所有资源的AB名")]
    static void SetAssetBundleName()
    {
        string path = Application.dataPath + "/Resources";

        isHaveNotWriteableMapFbx = "";
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            clearFolderName(dir);
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo info = files[i];
                setSingleAssetBundleName(info.FullName, info.Name);
            }
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (isHaveNotWriteableMapFbx != "")
        {
            EditorUtility.DisplayDialog("完成", "注意：部分地图fbx资源没有设置可改写！！！！\n" + isHaveNotWriteableMapFbx, "十分确定");
        }
        else
        {
            Debug.Log("！！！！！！！！！！全部资源名设置成功！！！！！！！！！");
            // EditorUtility.DisplayDialog("完成", "全部资源名设置成功！", "确定");
        }
    }

    [MenuItem("KYTool/AssetBundle/设置AB名/【手动】设置选中资源的AB名")]
    static void SetSelectedName()
    {
        InputWindow.InputWindowBegin();
    }

    [MenuItem("KYTool/AssetBundle/设置AB名/【谨慎】清除所有资源的AB名")]
    public static void ClearAssetBundleName()
    {
        string path = Application.dataPath + "/Resources";

        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            clearFolderName(dir);
            for (var i = 0; i < files.Length; ++i)
            {
                setSingleAssetBundleName(files[i].FullName, null, true);
            }
        }
        AssetDatabase.Refresh();  
        AssetDatabase.RemoveUnusedAssetBundleNames();
        EditorUtility.DisplayDialog("完成", "全部资源名清除成功！", "确定");
    }

    private static void clearFolderName(DirectoryInfo dirInfo)
    {
        setSingleAssetBundleName(dirInfo.FullName, null, true);
        DirectoryInfo[] dirList = dirInfo.GetDirectories();
        if (dirList.Length > 0)
        {
            for (var i = 0; i < dirList.Length; ++i)
            {
                DirectoryInfo subFolderInfo = dirList[i];
                clearFolderName(subFolderInfo);
            }
        }
    }

    [MenuItem("KYTool/AssetBundle/其他工具/清除模型碰撞器characterController")]
    public static void ClearCharacterController()
    {
        string path = Application.dataPath + "/Resources";
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo info = files[i];
                trueClearCharcterController(info.FullName, info.Name);
            }
        }
        AssetDatabase.Refresh();  
        EditorUtility.DisplayDialog("完成", "全部所有碰撞器成功！", "确定");
    }

    private static void trueClearCharcterController(string path, string fileName)
    {
        if (path.EndsWith(".meta")) return;
        int subIndex = path.IndexOf("\\Assets");
        string basePath = path.Substring(subIndex + 1);
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(basePath);
        if(go != null)
        {
            CharacterController controller = go.GetComponent<CharacterController>();
            if(controller != null)
            {
                Debug.Log("清除了碰撞器:"+basePath);
                DestroyImmediate(controller,true);   
            }
        }
    }

    private static void setSingleAssetBundleName(string path, string fileName, bool clearName = false)
    {
        if (path.EndsWith(".meta")) return;
        int subIndex = path.IndexOf("\\Assets");
        string basePath = path.Substring(subIndex + 1);
        string sub = "\\Resources";
        int nameIndex = basePath.IndexOf(sub);
        AssetImporter importer = AssetImporter.GetAtPath(basePath);
        if (clearName)
        {
            if (importer) importer.assetBundleName = string.Empty;
            return;
        }
        if (importer == null) { return; }

        string name = basePath.Substring(nameIndex + sub.Length + 1);
        string abName = string.Empty;
        string sourceFileName = fileName;
        int index = 0;
        fileName = fileName.ToLower();
        if (fileName.EndsWith(".ttf"))
        {
            //字体，按"font$" + 文件名 +".u"打包
            abName = "font$" + fileName + ".u";
        }
        else if (name.StartsWith("Atlas\\"))
        {
            //外部加载图集，全部打为"atlas$" + 文件夹名 + ".u"
            if (!fileName.EndsWith(".unity"))
            {
                name = name.Substring(6);
                index = name.IndexOf("\\");
                abName = "atlas$" + name.Substring(0, index) + ".u";
            }
        }
        else if (name.StartsWith("Fx\\"))
        {
            //特效，全部打为 "fx$" + 类型名_文件名 + ".u"
            if (!fileName.EndsWith(".unity") && !fileName.EndsWith(".anim"))
            {
                name = name.Substring(3);
                index = name.IndexOf("\\");
                string typeName = name.Substring(0, index);
                name = name.Substring(index + 1);
                abName = "fx$" + typeName + "_" + fileName + ".u";
                //if (typeName.StartsWith("Common") || typeName == "Scene")
                //{
                //    abName = "fx$" + typeName + "_" + fileName + ".u";
                //}
                //else
                //{
                //    index = name.IndexOf("\\");
                //    string siteName = name.Substring(0, index);
                //    abName = "fx$" + typeName + "_" + siteName + "_" + fileName + ".u";
                //}
            }
        }
        else if (name.StartsWith("GUI\\"))
        {
            //UI，将文件夹下的材质、预设、图集全部打为 "ui$" + 文件夹名 + ".u"
            if (!fileName.EndsWith(".unity"))
            {
                name = name.Substring(4);
                index = name.IndexOf("\\");
                abName = "ui$" + name.Substring(0, index) + ".u";
            }
        }
        else if (name.StartsWith("OutPic\\"))
        {
            //外部加载图，将文件夹下的图片打为 "outpic$" + 文件夹 + ".u"
            if (fileName.EndsWith(".png") || fileName.EndsWith(".jpg"))
            {
                abName = "outpic$" + fileName + ".u";
            }
        }
        else if (name.StartsWith("Maps\\"))
        {
            string folder = name.Substring(0, name.IndexOf(sourceFileName) - 1);
            bool isBattle = folder.EndsWith("b");
            //地图场景，按照"map$" + 文件名 + ".u"打包
            if (fileName.EndsWith(".unity"))
            {
                //if (fileName.IndexOf("empty") == -1) return;//10001empty.unity 是分离了Mesh的，10001.unity是原来的，只打分离后的场景
                abName = "map$" + fileName + ".u";
            }
            //切割Mesh保存的预设
            else if (fileName.EndsWith(".prefab"))
            {
                //string parentName = name.Substring(name.IndexOf("Maps\\"));
                string tmpName = name.Replace('\\','$');
                abName = tmpName + ".u";
            }
            //光照图
            else if (fileName.EndsWith(".exr") && !isBattle)
            {
                string tmpName = name.Replace('\\', '$');
                tmpName= tmpName.Replace(sourceFileName, "lightMap");
                abName = tmpName + ".u";
            }
            //地图通用资源文件夹，打包成"map$common.u"
            else if (name.StartsWith("Maps\\Common"))
            {
                if (fileName.EndsWith(".fbx") && name.IndexOf("\\Fx_source\\") >= 0)
                {
                    ModelImporter temp = (ModelImporter)importer;
                    if (temp.isReadable == false)
                    {
                        isHaveNotWriteableMapFbx += name + "\n";
                    }
                }
                name = name.Substring(5);
                index = name.IndexOf("\\");
                abName = "map$" + name.Substring(0, index) + ".u";
            }
            //地图个性资源文件夹，按照"map$" + 文件夹名 + ".u"打包
            else
            {
                if (fileName.EndsWith(".fbx") && name.IndexOf("\\Fx_source\\") >= 0)
                {
                    ModelImporter temp = (ModelImporter)importer;
                    if (temp.isReadable == false)
                    {
                        isHaveNotWriteableMapFbx += name + "\n";
                    }
                }
                name = name.Substring(5);
                index = name.IndexOf("\\");
                if (index == -1) index = name.Length;
                abName = "map$" + name.Substring(0, index) + ".u";
            }
        }
        else if (name.StartsWith("Roles\\"))
        {
            //形象资源，按照资源类型 + "$" + 文件名 + ".u"打包
            if (!fileName.EndsWith(".anim") && !fileName.EndsWith(".unity"))
            {
                name = name.Substring(6);
                index = name.IndexOf("\\");
                string typeName = name.Substring(0, index);
                // if (fileName.EndsWith(".prefab"))
                // {
                    // //如果是玩家角色的预设，则不导出（白模除外）
                    // if (typeName != "Role" || fileName == "101_equip_01.prefab" || fileName == "102_equip_01.prefab" || fileName == "103_equip_01.prefab" || fileName == "104_equip_01.prefab" || fileName == "105_equip_01.prefab" || fileName == "106_equip_01.prefab" || fileName == "107_equip_01.prefab" || fileName == "108_equip_01.prefab" || fileName == "109_equip_01.prefab" || fileName == "110_equip_01.prefab" || fileName == "111_equip_01.prefab" || fileName == "112_equip_01.prefab"
                    //     || fileName == "101_equip_01_ui.prefab" || fileName == "102_equip_01_ui.prefab" || fileName == "103_equip_01_ui.prefab"
                    //     || fileName == "104_equip_01_ui.prefab" || fileName == "105_equip_01_ui.prefab" || fileName == "106_equip_01_ui.prefab"
                    //     || fileName == "107_equip_01_ui.prefab" || fileName == "108_equip_01_ui.prefab" || fileName == "109_equip_01_ui.prefab"
                    //     || fileName == "110_equip_01_ui.prefab" || fileName == "111_equip_01_ui.prefab" || fileName == "112_equip_01_ui.prefab")
                    // {
                    //     //如果是12个职业的白模，则需要导出
                    // }
                    // else
                    // {
                    //     return;
                    // }
                    // if (fileName.IndexOf("_light") > 0) 
                    // {
                    //     fileName = fileName.Substring(0, fileName.IndexOf("_light"));
                    //     fileName = fileName+".prefab";
                    // }
                // }
                abName = typeName + "$" + fileName + ".u";
            }
        }
        else if (name.StartsWith("Shader\\"))
        {
            //Shader，全部文件打成一个包shader.u
            abName = "shader.u";
        }
        else
        {
            return;
        }

        if (importer.assetBundleName != abName)
        {
           importer.assetBundleName = abName;
        }
        //Debug.Log(path + ":::" + abName);
    }

    //[MenuItem("KYTool/AssetBundle/拷贝模板表与地图数据")]
    static void CopyTemplatesAndMapInfos()
    {
        string tempPath = Application.dataPath + "/../../tools/tempOutput/ky203.txt";
        string mapsPath = Application.dataPath + "/MapTestInfo";
        string tarPath = Application.dataPath + "/StreamingAssets/";

        if (!File.Exists(tempPath))
        {
            EditorUtility.DisplayDialog("错误", "tempOutput里找不到模板表文件ky203.txt！", "确定");
            return;
        }
        if (File.Exists(tarPath + "ky203.txt"))
        {
            File.Delete(tarPath + "ky203.txt");
        }
        File.Copy(tempPath, tarPath + "ky203.txt");
        if (Directory.Exists(mapsPath))
        {
            var dir = new DirectoryInfo(mapsPath);
            var files = dir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            for (var i = 0; i < files.Length; ++i)
            {
                var fileName = files[i].Name;
                if (File.Exists(tarPath + fileName))
                {
                    File.Delete(tarPath + fileName);
                }
                File.Copy(mapsPath + "/" + fileName, tarPath + fileName);
            }
        }
        EditorUtility.DisplayDialog("完成", "模板表与地图数据拷贝成功！", "确定");
    }

    [MenuItem("KYTool/AssetBundle/其他工具/资源检测工具")]
    static void CheckAssetBundle()
    {
        CheckWindow.CheckWindowBegin();
    }

       // 执行BAT 清空DOC目录资源
    // [MenuItem("KYTool/AssetBundle/【发布】清空DOC目录资源")]
    static void clearPublicDoc()
    {
        System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
        proc1.StartInfo.FileName = Application.dataPath + "/../tools/delDocRes.bat";
        //Debug.Log("脚本路径 == "+proc1.StartInfo.FileName);
        proc1.StartInfo.CreateNoWindow = false;
        proc1.Start();
        proc1.WaitForExit();
        AssetDatabase.Refresh();
    }

    // 拷贝地图数据
    static void copyMapData(string targetPath)
    {
        string mapDataPath = Application.dataPath + "/MapTestInfo/mapData";
        string tagPath = Application.streamingAssetsPath+targetPath + "/";
        if (Directory.Exists(mapDataPath))
        {
            var dir = new DirectoryInfo(mapDataPath);
            // var files = dir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            var files = dir.GetFiles();
            // Debug.Log("路径名字  == "+tagPath);
            // Debug.Log("地图文件长度 == "+files.Length);
            for (var i = 0; i < files.Length; ++i)
            {
                var fileName = files[i].Name;
                if (File.Exists(tagPath + fileName))
                {
                    File.Delete(tagPath + fileName);
                }
                File.Copy(mapDataPath + "/" + fileName, tagPath + fileName);
            }
        }
    }

    [MenuItem("KYTool/AssetBundle/打包/【win】拷贝地图数据")]
    static void copyMapDataToWin()
    {
        copyMapData("");
    }

    [MenuItem("KYTool/AssetBundle/打包/【android】拷贝地图数据")]
    static void copyMapDataToAndroid()
    {
        copyMapData("_android");
    }

    [MenuItem("KYTool/AssetBundle/打包/【ios】拷贝地图数据")]
    static void copyMapDataToIos()
    {
        copyMapData("_ios");
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/发布/【win】拷贝ab包到DOC目录")]
    static void copyToPublicDoc()
    {
        System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
        proc1.StartInfo.FileName = Application.dataPath + "/../tools/resToDoc.bat";
        proc1.StartInfo.CreateNoWindow = false;
        proc1.Start();
        proc1.WaitForExit();
        AssetDatabase.Refresh();
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/发布/【android】拷贝ab包到DOC目录")]
    static void copyAndroidToPublicDoc()
    {
        System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
        proc1.StartInfo.FileName = Application.dataPath + "/../tools/resAndroidToDoc.bat";
        proc1.StartInfo.CreateNoWindow = false;
        proc1.Start();
        proc1.WaitForExit();
        AssetDatabase.Refresh();
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/发布/【ios】拷贝ab包到DOC目录")]
    static void copyIosToPublicDoc()
    {
        System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
        proc1.StartInfo.FileName = Application.dataPath + "/../tools/resIosToDoc.bat";
        proc1.StartInfo.CreateNoWindow = false;
        proc1.Start();
        proc1.WaitForExit();
        AssetDatabase.Refresh();
    }

    [MenuItem("KYTool/AssetBundle/发布/【全平台jt1使用】拷贝全平台ab包到DOC目录")]
    static void copyAllToPublicDoc()
    {
        copyToPublicDoc();
        copyAndroidToPublicDoc();
        copyIosToPublicDoc();
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/一键打包发布/【andoird】命名+打包+发布doc")]
    static void oneKeyAndroidPublicDoc()
    {
        SetAssetBundleName();
        CreatAllAB_android();
        copyAndroidToPublicDoc();
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/一键打包发布/【ios】命名+打包+发布doc")]
    static void oneKeyIosPublicDoc()
    {
        SetAssetBundleName();
        CreatAllAB_ios();
        copyIosToPublicDoc();
    }

        // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/一键打包发布/【【【全平台】】】命名+打包+发布doc")]
    static void oneKeyAllPublicDoc()
    {
        SetAssetBundleName();
        CreatAllAB();
        CreatAllAB_android();
        CreatAllAB_ios();
        copyToPublicDoc();
        copyAndroidToPublicDoc();
        copyIosToPublicDoc();
    }

    // 执行BAT 拷贝到发布DOC目录下
    [MenuItem("KYTool/AssetBundle/【一键发布win】命名+打包+发布doc")]
    static void oneKeyPublicDoc()
    {
        SetAssetBundleName();
        CreatAllAB();
        copyToPublicDoc();
    }

    // 执行BAT 拷贝到发布DOC目录下
    static void clearUnityEditorCode(bool ispublic)
    {
        if(ispublic == true)
        {
            System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
            proc1.StartInfo.FileName = Application.dataPath + "/../tools/changeToPublic.bat";
            proc1.StartInfo.CreateNoWindow = false;
            proc1.Start();
            proc1.WaitForExit();
            AssetDatabase.Refresh();
        }
        else
        {
            System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
            proc1.StartInfo.FileName = Application.dataPath + "/../tools/changeToEditor.bat";
            proc1.StartInfo.CreateNoWindow = false;
            proc1.Start();
            proc1.WaitForExit();
            AssetDatabase.Refresh();
        }
    }



    [MenuItem("KYTool/地图点工具/生成地图点坐标xyz[默认排序]")]
    private static void OneKeyGerneralMonsterPos1()
    {
        // string fileName = "monsterPos.txt";
        // string path = EditorUtility.SaveFolderPanel("保存目录:", "", "");
        GameObject[] gameobjects = Selection.gameObjects;
        Array.Sort<GameObject>(gameobjects, (x, y) => y.GetInstanceID().CompareTo(x.GetInstanceID()));
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < gameobjects.Length; i++)
        {
            if (i == gameobjects.Length - 1)
            {
                stringBuilder.Append(string.Format("{0},{1}", Math.Round(gameobjects[i].transform.position.x,3), Math.Round(gameobjects[i].transform.position.z,3)));
            }
            else
            {
                stringBuilder.Append(string.Format("{0},{1}|", Math.Round(gameobjects[i].transform.position.x, 3), Math.Round(gameobjects[i].transform.position.z, 3)));
            }
        }
        Debug.Log(stringBuilder.ToString());
        // File.WriteAllText(Path.Combine(path, fileName), stringBuilder.ToString());
    }

    [MenuItem("KYTool/地图点工具/生成地图点坐标xyz[数字排序]")]
    private static void OneKeyGerneralWorldPos()
    {
      // string fileName = "wolrdPos.txt";
      // string path = EditorUtility.SaveFolderPanel("保存目录:","","");
      GameObject[] gameobjects = Selection.gameObjects;
      Array.Sort<GameObject>(gameobjects, (x, y) => int.Parse(x.name).CompareTo(int.Parse(y.name)));
      StringBuilder stringBuilder = new StringBuilder();
      for(int i = 0;i<gameobjects.Length;i++)
      {
            if (i == gameobjects.Length - 1)
            {
               stringBuilder.Append(string.Format("{0},{1},{2},{3}", gameobjects[i].name, Math.Round(gameobjects[i].transform.position.x,3), Math.Round(gameobjects[i].transform.position.y,3), Math.Round(gameobjects[i].transform.position.z,3)));
            }
            else
            {
                stringBuilder.Append(string.Format("{0},{1},{2},{3}|", gameobjects[i].name, Math.Round(gameobjects[i].transform.position.x, 3), Math.Round(gameobjects[i].transform.position.y, 3), Math.Round(gameobjects[i].transform.position.z, 3)));
            }
        }
        Debug.Log(stringBuilder.ToString());
        // File.WriteAllText(Path.Combine(path,fileName),stringBuilder.ToString());
    }

    [MenuItem("KYTool/地图点工具/一键改数字名[基于InstanceID]")]
    private static void OneKeyGerneralMonsterName()
    {
        GameObject[] gameobjects = Selection.gameObjects;
        Array.Sort<GameObject>(gameobjects, (x, y) => y.GetInstanceID().CompareTo(x.GetInstanceID()));
        for (int i = 0; i < gameobjects.Length; i++)
        {
            // Debug.Log("改名对应信息 == " + gameobjects[i].name + "," + gameobjects[i].GetInstanceID() + "," + i);
            Debug.Log("改名后对应信息= " + gameobjects[i].name + "-" + (i + 1).ToString());
            gameobjects[i].name = (i + 1).ToString();
        }
    }

    [MenuItem("KYTool/地图点工具/直接输出选中点坐标xz")]
    private static void posOutPrint()
    {
        GameObject[] gameobjects = Selection.gameObjects;
        Array.Sort<GameObject>(gameobjects, (x, y) => y.GetInstanceID().CompareTo(x.GetInstanceID()));
        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder stringBuilder1 = new StringBuilder();
        for (int i = 0; i < gameobjects.Length; i++)
        {
            if (i == gameobjects.Length - 1)
            {
                stringBuilder.Append(string.Format("{2}:{0},{1}", Math.Round(gameobjects[i].transform.position.x,3), Math.Round(gameobjects[i].transform.position.z,3),gameobjects[i].name));

                stringBuilder1.Append(string.Format("{0},{1}", Math.Round(gameobjects[i].transform.position.x,3), Math.Round(gameobjects[i].transform.position.z,3)));
            }
            else
            {
                stringBuilder.Append(string.Format("{2}:{0},{1}|", Math.Round(gameobjects[i].transform.position.x, 3), Math.Round(gameobjects[i].transform.position.z, 3),gameobjects[i].name));

                stringBuilder1.Append(string.Format("{0},{1}|", Math.Round(gameobjects[i].transform.position.x, 3), Math.Round(gameobjects[i].transform.position.z, 3)));
            }
        }
        Debug.Log(stringBuilder.ToString());
        Debug.Log(stringBuilder1.ToString());
    }

    // [MenuItem("AniSystem/同步技能编辑器数据")]
    // public static void PSDImportOneKey()
    // {
    //     System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
    //     proc1.StartInfo.FileName = Application.dataPath + "/../tools/resToResouce.bat";
    //     proc1.StartInfo.CreateNoWindow = false;
    //     proc1.Start();
    //     proc1.WaitForExit();
    //     AssetDatabase.Refresh();
    // }
    // 指定AB名复制
    public static void copyOneABToDoc(string name)
    {
        SetAssetBundleName();
        CreatAllAB();
        System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
        proc1.StartInfo.FileName = Application.dataPath + "/../tools/resOneToDoc.bat";
        proc1.StartInfo.CreateNoWindow = false;
        proc1.StartInfo.Arguments = name;
        proc1.Start();
        proc1.WaitForExit();
        AssetDatabase.Refresh();
    }
}

