using UnityEngine;
using UnityEditor;

public class InputWindow : EditorWindow
{
    private string abName = string.Empty;

    public static void InputWindowBegin()
    {
        InputWindow.CreateInstance<InputWindow>().Show();
    }

    public void Awake()
    {
        Object[] selects = Selection.objects;
        if (selects.Length > 0)
        {
            string path = AssetDatabase.GetAssetPath(selects[0]);
            AssetImporter asset = AssetImporter.GetAtPath(path);
            abName = asset.assetBundleName;
        }
        if (abName == null)
        {
            abName = string.Empty;
        }
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("设置当前选中资源的AB名称", GUILayout.Width(200));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("请输入AB名：", GUILayout.Width(80));
        abName = EditorGUILayout.TextField("", abName, GUILayout.Width(165));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("修改", GUILayout.Width(80)))
        {
            setSelectFiles(abName);
        }
        if (GUILayout.Button("清空", GUILayout.Width(80)))
        {
            setSelectFiles(string.Empty);
        }
        if (GUILayout.Button("取消", GUILayout.Width(80)))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void setSelectFiles(string name)
    {
        Object[] selects = Selection.objects;
        int count = 0;
        foreach (Object item in selects)
        {
            string path = AssetDatabase.GetAssetPath(item);
            AssetImporter asset = AssetImporter.GetAtPath(path);
            asset.assetBundleName = name;
            asset.SaveAndReimport();
            count++;
        }
        AssetDatabase.Refresh();
        if (count > 0)
        {
            Debug.Log("设置完成(共" + count + "个文件)");
        }
        else
        {
            Debug.Log("未选中任何文件！");
        }
    }
}