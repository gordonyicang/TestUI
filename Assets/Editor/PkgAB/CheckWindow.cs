using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class CheckWindow : EditorWindow
{
    private AssetBundle manifestAB;
    private AssetBundleManifest manifest;
    private string abNames = "";
    private string result = "";
    private string countStr = "";

    public static void CheckWindowBegin()
    {
        CheckWindow.CreateInstance<CheckWindow>().Show();
    }

    public void Awake()
    {
        LoadManifest();
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("请输入资源名称，多个资源请换行", GUILayout.Width(200));
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("添加选中资源", GUILayout.Width(120)))
        {
            AddSelectAssetToList();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        abNames = EditorGUILayout.TextArea(abNames, GUILayout.Width(170),GUILayout.Height(200));
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.TextArea(result, GUILayout.Width(170), GUILayout.Height(200));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("检测资源完整性", GUILayout.Width(120)))
        {
            CheckAssetBundle();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label(countStr, GUILayout.Width(150));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void LoadManifest()
    {
        if(manifestAB != null)
        {
            manifestAB.Unload(true);
        }
        manifestAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/StreamingAssets");
        manifest = (AssetBundleManifest)manifestAB.LoadAsset("AssetBundleManifest");
    }

    private void CheckAssetBundle()
    {
        Dictionary<string, bool> nameDic = new Dictionary<string, bool>();
        string[] abNameList = abNames.Split('\n');
        for (var i = 0;i < abNameList.Length;i++)
        {
            string name = abNameList[i];
            if(name == "" || name == string.Empty)
            {
                continue;
            }
            try
            {
                nameDic.Add(name, false);
            }
            catch (ArgumentException)
            {

            }
            string[] depList = getDependList(name);
            for (var j = 0; j < depList.Length; j++)
            {
                try
                {
                    nameDic.Add(depList[j], false);
                }
                catch (ArgumentException)
                {
                    
                }
            }
        }
        result = "";

        string nothasList = "";
        string hasList = "";
        int notCount = 0;
        foreach (string key in nameDic.Keys)
        {
            bool isHas = File.Exists(Application.streamingAssetsPath + "/" + key);
            if(isHas)
            {
                hasList += ("√" + key + "\n");
                //如果是几乎每个资源都会依赖到的shader就不用去检测了
                if(key != "shader.u")
                {
                    CheckAssetComplete(key);
                }
            }
            else
            {
                notCount++;
                nothasList += ("×" + key + "\n");
            }
        }
        countStr = "一共有" + notCount + "个资源未找到！";
        result += nothasList + hasList;
    }

    private void CheckAssetComplete(string name)
    {
        AssetBundle tempAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + name);
        List<AssetBundle> dependABs = new List<AssetBundle>();
        string[] depList = getDependList(name);
        for (var i = 0; i < depList.Length; i++)
        {
            AssetBundle temp = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + depList[i]);
            dependABs.Add(temp);
        }
        string[] assetNames = tempAB.GetAllAssetNames();
        for (var j = 0; j < assetNames.Length; j++)
        {
            string assetName = assetNames[j];
            if(assetName.EndsWith(".prefab"))
            {
                GameObject prefab = (GameObject)tempAB.LoadAsset(assetName);
                GameObject go = GameObject.Instantiate(prefab);
                Animator ani = go.GetComponent<Animator>();
                SkinnedMeshRenderer ren = go.GetComponentInChildren<SkinnedMeshRenderer>();

                if (ani.runtimeAnimatorController == null)
                {
                    result += ("×缺少动作" + assetName + "\n");
                }
                if (ani.avatar == null)
                {
                    result += ("×缺少蒙皮" + assetName + "\n");
                }
                if (ren.sharedMesh == null)
                {
                    result += ("×缺少网格" + assetName + "\n");
                }
                if (ren.sharedMaterial == null)
                {
                    result += ("×缺少贴图" + assetName + "\n");
                }

                GameObject.DestroyImmediate(go);
            }
        }

        tempAB.Unload(true);
        for (int k = 0; k < dependABs.Count; k++)
        {
            dependABs[k].Unload(true);
        }
    }

    private string[] getDependList(string name)
    {
        if(manifest == null) { return null; }
        return manifest.GetAllDependencies(name);
    }

    private void AddSelectAssetToList()
    {
        Dictionary<string, bool> nameDic = new Dictionary<string, bool>();

        UnityEngine.Object[] selects = Selection.objects;
        foreach (UnityEngine.Object item in selects)
        {
            string path = AssetDatabase.GetAssetPath(item);
            AssetImporter asset = AssetImporter.GetAtPath(path);
            string name = asset.assetBundleName;
            if(name != null && name != string.Empty && name != "" && nameDic.ContainsKey(name) == false)
            {
                nameDic.Add(name,false);
                abNames += (name + "\n");
            }
        }
    }

    public void OnDestroy()
    {
        if(manifestAB != null)
        {
            manifestAB.Unload(true);
        }
        manifestAB = null;
        manifest = null;
    }
}