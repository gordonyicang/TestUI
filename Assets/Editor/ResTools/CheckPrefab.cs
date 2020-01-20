using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CheckPrefab : EditorWindow
{
    private static List<string> showPrefabList = new List<string>();
    private static Vector2 scrollPosition;
    private static GUIStyle styleOfScroll;

    [MenuItem("Assets/CheckPrefab", priority = 1)]
    private static void CheckPrefabs()
    {
        showPrefabList.Clear();

        string[] selectionFilesOrFolders = Selection.assetGUIDs;

        if (selectionFilesOrFolders.Length != 0)
        {
            foreach (string selectionFileOrFloder in selectionFilesOrFolders)
            {
                string selectionObjPath = AssetDatabase.GUIDToAssetPath(selectionFileOrFloder);
                string selectionAbsolutePath = Application.dataPath + selectionObjPath.Replace("Assets", "");

                //选中的是一个文件（PS：一个prefab）
                if (File.Exists(selectionAbsolutePath))
                {
                    if (selectionObjPath.EndsWith(".prefab"))
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(selectionObjPath);
                        List<string> perPrefabs = CheckPerRoot(prefab, selectionObjPath);
                        foreach (string perPrefab in perPrefabs)
                        {
                            showPrefabList.Add(perPrefab);
                        }
                    }
                }

                //选中的是文件夹
                else if (Directory.Exists(selectionAbsolutePath))
                {
                    DirectoryInfo assetsPathFolder = new DirectoryInfo(selectionAbsolutePath);
                    SolveFolder(assetsPathFolder);
                }
            }
        }

        //如果没有选中任何Assets中得文件夹或文件，默认检查整个Assets下的prefab
        else
        {
            DirectoryInfo assetsPathFolders = new DirectoryInfo(Application.dataPath);
            SolveFolder(assetsPathFolders);
        }

        if (showPrefabList.Count != 0)
        {
            styleOfScroll = new GUIStyle();
            GetWindow<CheckPrefab>();
        }
        else
        {
            EditorUtility.DisplayDialog("Tips", "Dont't find out the prefab with script ,\n or the file isn't prefab !", "cancel");
        }
    }


    void OnGUI()
    {
        if (showPrefabList.Count != 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, styleOfScroll);

            foreach (string perPrefabStr in showPrefabList)
            {
                GUILayout.Space(7);

                if (GUILayout.Button(perPrefabStr))
                {
                    GoToRoot(perPrefabStr);
                }
            }
            GUILayout.EndScrollView();
        }
    }


    //定位带有脚本的节点，需要先在场景中创建该prefab对象
    private static void GoToRoot(string PrefabStr)
    {
        string[] prefabInfo = PrefabStr.Split(new string[] { "Path : ", "Root : ", "Scripts : ", ",", "!" }, StringSplitOptions.RemoveEmptyEntries);

        GameObject mainRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabInfo[0]);
        //打开父节点所在Assets路径
        Selection.activeGameObject = mainRoot;
        EditorGUIUtility.PingObject(mainRoot);

        if (mainRoot != null)
        {

            GameObject mainObj = null;
            string mainName = mainRoot.name + "_checkPrefabInstance";

            //避免重复生成物体
            if (GameObject.Find(mainName) == null)
            {
                mainObj = PrefabUtility.InstantiatePrefab(mainRoot) as GameObject;
                mainObj.name = mainName;
            }
            else
            {
                mainObj = GameObject.Find(mainName);
            }

            //通过比对节点层次字符串定位
            foreach (Transform perRoot in mainObj.GetComponentsInChildren<Transform>())
            {
                MonoBehaviour[] scripts = perRoot.GetComponents<MonoBehaviour>();

                if (scripts.Length != 0)
                {
                    //节点路径比对
                    string rootWithScriptPath = perRoot.name;
                    Transform rootParent = perRoot.parent;
                    while (rootParent != null)
                    {
                        rootWithScriptPath = rootParent.name + "/" + rootWithScriptPath;
                        rootParent = rootParent.parent;
                    }

                    if (rootWithScriptPath.Replace("_checkPrefabInstance", "").Equals(prefabInfo[2]))
                    {
                        //定位节点
                        Selection.activeTransform = perRoot;
                        EditorGUIUtility.PingObject(perRoot);
                        break;
                    }
                }
            }
        }
    }


    private static List<string> CheckPerRoot(GameObject parentRoot, string assetsPath)
    {
        List<string> rootsWithScriptPath = new List<string>();

        foreach (Transform perRoot in parentRoot.GetComponentsInChildren<Transform>())
        {

            MonoBehaviour[] scripts = perRoot.GetComponents<MonoBehaviour>();

            //剔除UI自带脚本（PS：如Text,Image等等）
            List<MonoBehaviour> nonByoScripts = new List<MonoBehaviour>();

            foreach (MonoBehaviour perScript in scripts)
            {
                if (perScript != null)
                {
                    if (typeof(UIBehaviour).IsAssignableFrom(perScript.GetType()) == false)
                    {
                        nonByoScripts.Add(perScript);
                    }
                }
                else
                {
                    nonByoScripts.Add(perScript);
                }
            }

            if (nonByoScripts.Count != 0)
            {
                //perfab携带的脚本
                string scriptsStr = null;
                int missSum = 0;//记录丢失得脚本数

                foreach (MonoBehaviour perScript in nonByoScripts)
                {
                    if (perScript != null)
                    {
                        scriptsStr += perScript.ToString() + ", ";
                    }
                    else
                    {
                        missSum += 1;
                    }
                }
                if (missSum != 0)
                {
                    scriptsStr += "Miss " + missSum + " monoscript" + ", ";
                }

                //节点路径
                string rootWithScriptPath = perRoot.name;
                Transform rootParent = perRoot.parent;
                while (rootParent != null)
                {
                    rootWithScriptPath = rootParent.name + "/" + rootWithScriptPath;
                    rootParent = rootParent.parent;
                }

                //输出信息                        
                string perRootInfo = string.Format("Path : {0},\nRoot : {1},\nScripts : {2}!", assetsPath, rootWithScriptPath, scriptsStr.Remove(scriptsStr.Length - 2));

                if (!rootsWithScriptPath.Contains(perRootInfo))
                {
                    rootsWithScriptPath.Add(perRootInfo);
                }
            }
        }
        return rootsWithScriptPath;
    }


    private static void SolveFolder(DirectoryInfo assetsPathFolder)
    {
        FileInfo[] assetsPathFiles = assetsPathFolder.GetFiles("*", SearchOption.AllDirectories);
        foreach (FileInfo perFile in assetsPathFiles)
        {
            if (perFile.Extension.Equals(".prefab"))
            {
                string assetsFilePath = "Assets/" + perFile.ToString().Replace(@"\", "/").Split(new string[] { "/Assets/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetsFilePath);
                List<string> perPrefabs = CheckPerRoot(prefab, assetsFilePath);
                foreach (string perPrefab in perPrefabs)
                {
                    showPrefabList.Add(perPrefab);
                }
            }
        }
    }


    [MenuItem("Tools/检查超大图片资源")]
    private static void CheckImagesSize()
    {
        string path = Application.dataPath + "/Resources";
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo info = files[i];
                var name = info.Name.ToLower();
                if(name.EndsWith(".jpg") || name.EndsWith(".png"))
                {
                    int subIndex = info.FullName.IndexOf("\\Assets");
                    string imgPath = info.FullName.Substring(subIndex + 1);
                    //TextureImporter imp = (TextureImporter)TextureImporter.GetAtPath(imgPath);

                    Texture2D tx = new Texture2D(100, 100);
                    tx.LoadImage(getImageByte(imgPath));
                    if(tx.width > 1024 || tx.height > 1024)
                    {
                        Debug.Log("图片尺寸过大:" + imgPath);
                    }
                }
            }
        }
    }
    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }
}