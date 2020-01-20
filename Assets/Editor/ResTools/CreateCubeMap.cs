using UnityEngine;
using UnityEditor;
using System;
using System.IO;

//拖拽场景中的物体实现cubemap
public class CreateCubeMap : Editor
{
    private static int size = 256;

    [MenuItem("CreateAssets/Selecting a gameobject for rendering cubemap ", priority = 1)]  
    private static void RenderCubeMap()
    {
        GameObject renderPos = Selection.activeGameObject;

        if(renderPos == null)
        {
            EditorUtility.DisplayDialog("Warning", "Please select the gameObject in the scene at first!", "cancel");
            return;
        }

        SaveCubeMap(renderPos, size);        
    }


    public static void SaveCubeMap(GameObject renderPos, int size)
    {
        string path = EditorUtility.SaveFilePanel("Create CubeMap", Application.dataPath, "new Cube", "cubemap");

        if (path != "")
        {
            if (!path.Contains(@"\Assets\"))
            {
                path = "Assets/" + path.Split(new string[] { "/Assets/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                Cubemap cube = new Cubemap(size, TextureFormat.RGBA32, false);
                if (cube != null)
                {
                    AssetDatabase.CreateAsset(cube, path);
                    Render(cube, renderPos);
                    GetSixFaceTexFromCubeMap(cube, path, size);
                    Selection.activeObject = cube;                    
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", "Please select folder of assets in this project!", "cancel");
            }
        }
    }


    public static void Render(Cubemap cube, GameObject renderPos)
    {
        GameObject go = new GameObject("RenderCamera", typeof(Camera));
        go.transform.position = renderPos.transform.position;
        go.GetComponent<Camera>().RenderToCubemap(cube);
        DestroyImmediate(go);
    }


    public static void GetSixFaceTexFromCubeMap(Cubemap cubeMap, string path, int size)
    {
        Texture2D[] textures = new Texture2D[6];
        for(int i = 0; i < 6; i++)
        {
            textures[i] = new Texture2D(size, size, TextureFormat.ARGB32, false);

            //由于读取的像素是左右上下反过来的，需要将cubemap的每个面的像素从右到左下到上的顺序读取并写入到新的纹理中
            for (int heigth = size; heigth > 0; heigth--)
            {
                for (int width = size; width > 0; width--)
                {
                    Color perPexilColor = cubeMap.GetPixel((CubemapFace)i, heigth - 1, width - 1);
                    textures[i].SetPixel(size - heigth, size - width, perPexilColor);
                }
            }

            //处理存储路径
            string textureName = Enum.GetName(typeof(CubemapFace), i) + ".png";
            string[] pathSplit = path.Split('/');
            string realPath = ChannelSeperate.AssetPathToWindowPath(path.Replace(pathSplit[pathSplit.Length - 1], textureName)); 
            if(File.Exists(realPath))
            {
                realPath = realPath.Replace(".png", "_FCM.png");
            }

            //保存
            byte[] bytes = textures[i].EncodeToPNG();
            File.WriteAllBytes(realPath, bytes);
            AssetDatabase.Refresh();
        }
    }
}

//选中场景中的物体实现cubemap
public class GainCubeMap : ScriptableWizard
{
    public GameObject renderPos;

    private int size = 64;
    private static string[] diplayStr = { "16", "32", "64", "128", "256", "512", "1024", "2048" };
    private static int[] selection;

    [MenuItem("CreateAssets/Rendering cubemap", priority = 2)]
    private static void RenderCubeMap()
    {
        ScriptableWizard.DisplayWizard<GainCubeMap>("Render cubemap", "Render!");
        selection = Array.ConvertAll<string, int>(diplayStr, s => int.Parse(s));
    }

    void OnWizardCreate()
    {
        CreateCubeMap.SaveCubeMap(renderPos, size);
    }

    void OnWizardUpdate()
    {

        helpString = "Please appoint the render position and drag the cubemap for rendering:";
        isValid = (renderPos != null);

    }

    protected override bool DrawWizardGUI()
    {
        base.DrawWizardGUI();
        size = EditorGUILayout.IntPopup("Face size", size, diplayStr, selection);
        return true;
    }
}