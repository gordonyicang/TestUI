using System;
using System.IO;
using UnityEngine;
using UnityEditor;
public class ChannelSeperate 
{

    [MenuItem("Assets/AlphaTexOpration/Seperate Channel", priority = 0)]
    private static void SelectSeperate()
    {
        Texture2D selectTex = Selection.activeObject as Texture2D;   
        if (selectTex != null)
        {
            Seperate(selectTex);
        }
        else
        {
            EditorUtility.DisplayDialog("warning", "You didn't selected a texture!", "cancel");
        }
    }


    public static string Seperate(Texture2D texure)
    {        
        string selectPath = AssetDatabase.GetAssetPath(texure);
        string savePath = AssetPathToWindowPath(selectPath);//存储路径

        //创建所选中纹理临时纹理，方便处理的时候不破坏原纹理的设置
        FileInfo fi = new FileInfo(savePath);//获取后缀名
        string copyPath = selectPath.Replace(fi.Extension, "_clone$$" + fi.Extension);
        AssetDatabase.CopyAsset(selectPath, copyPath);            
        Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(copyPath);
            

        //设置临时纹理属性
        TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(temp)) as TextureImporter;
        TextureImporterPlatformSettings textureSetting = new TextureImporterPlatformSettings();
        textureSetting.overridden = true;
        textureSetting.name = "Android";
        textureSetting.format = TextureImporterFormat.RGBA32;
        importer.SetPlatformTextureSettings(textureSetting);
        importer.isReadable = true;
        importer.SaveAndReimport();

        //创建rgb贴图和alpha贴图
        Texture2D alphaTex = new Texture2D(temp.width, temp.height, TextureFormat.RGB24, true);
        //分别设置填充各个像素的颜色
        Color[] alphaColors = new Color[temp.width * temp.height];
        for (int i = 0; i < temp.width; i++)
        {
            for (int j = 0; j < temp.height; j++)
            {
                Color color = temp.GetPixel(i, j);
                //处理alpha通道图
                alphaColors[temp.width * j + i] = new Color(color.a, color.a, color.a);
            }
        }
        alphaTex.SetPixels(alphaColors);
        alphaTex.Apply();

        //删除临时纹理
        AssetDatabase.DeleteAsset(copyPath);

        //保存           
        string alphaPath = SolvePath(savePath);           
        if (File.Exists(alphaPath))
        {
            if (EditorUtility.DisplayDialog("warning", "There is already the same name's texture in the folder!\nDo you replace it?", "sumbit", "cancel"))
            {
                Save(alphaTex, alphaPath);
            }               
        }
        else
        {
            Save(alphaTex, alphaPath);
        }
        return alphaPath;
    }


    //设置路径
    private static string SolvePath(string path)
    {
        FileInfo fi=new FileInfo(path);      
        path = path.Split(new string[] { fi.Extension }, StringSplitOptions.RemoveEmptyEntries)[0] + "_alpha";
        path = path + ".png";
        return path;
    }


    //处理相同的名字的纹理
    private static void Save(Texture2D alphaTex,string alphaPath)
    {
        byte[] bytes = alphaTex.EncodeToPNG();
        File.WriteAllBytes(alphaPath, bytes);
        AssetDatabase.Refresh();
        //选中生成的纹理
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/" + alphaPath.Split(new string[] { "/Assets/" }, StringSplitOptions.RemoveEmptyEntries)[1]);
        EditorGUIUtility.PingObject(Selection.activeObject);   
    }

    //获取windows的资源路径
    public static string AssetPathToWindowPath(string assetPath)
    {
        string[] splitPath = Application.dataPath.Split('/');//得到该数组最后一个字符串Assets，把其剔除掉，由于assetPath包含了Assets
        string savePath = Application.dataPath.Replace(splitPath[splitPath.Length - 1], "") + assetPath;//最终windows存储路径
        return savePath;
    }
}
