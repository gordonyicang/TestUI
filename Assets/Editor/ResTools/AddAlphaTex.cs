using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
public class AddAlphaTex : ScriptableWizard
{
    //用于还原的material
    private static Dictionary<Material, MatShaderAndTextureSetting> matInfo = new Dictionary<Material, MatShaderAndTextureSetting>();

	[MenuItem("Assets/AlphaTexOpration/AddAlphaTexToMats",priority = 1)]
    private static void TexOperation()
    {
        string operationTip = "Is Alpha separation starting?";
        if(EditorUtility.DisplayDialog("Tips", operationTip, "yes","no"))
        {
            matInfo.Clear();
            string[] selectPathIds = Selection.assetGUIDs;

            if (selectPathIds.Length != 0)
            {
                bool haveMat = false;//判断所选文件或者文件夹有没有material

                foreach (string perPathId in selectPathIds)
                {
                    string perPath = AssetDatabase.GUIDToAssetPath(perPathId);
                    string absolutePath = Application.dataPath.Replace("Assets", "") + perPath;
                    DirectoryInfo di = new DirectoryInfo(absolutePath);

                    //处理文件夹
                    if (di.Extension.Equals(""))
                    {
                        FileInfo[] fi = di.GetFiles("*", SearchOption.AllDirectories);
                        foreach (FileInfo f in fi)
                        {
                            if (f.Extension == ".mat")
                            {
                                if (haveMat == false)
                                {
                                    haveMat = true;
                                }
                                string realAssetsPath = perPath + f.ToString().Replace(@"\", "/").Split(new string[] { perPath }, StringSplitOptions.RemoveEmptyEntries)[1];
                                JudgeShader(realAssetsPath);
                            }
                        }
                    }

                    //处理文件
                    else
                    {
                        if (perPath.ToString().EndsWith(".mat"))
                        {
                            haveMat = true;
                            JudgeShader(perPath);
                        }
                    }
                }

                if (haveMat == false)
                {
                    EditorUtility.DisplayDialog("Tips", "There is not material in the selected folder!", "cancel");
                }
                else
                {
                    operationTip = "Finish";
                    EditorUtility.DisplayDialog("Tips", operationTip, "ok");
                }
                AssetDatabase.Refresh();
            }       
        }       
    }


    //还原
    [MenuItem("Assets/AlphaTexOpration/ReStore", priority = 2)]
    private static void ReStore()
    {
        foreach(Material perMat in matInfo.Keys)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(perMat.GetTexture("_AlphaTex")));
            Texture2D mainTex = perMat.mainTexture as Texture2D;
            if (mainTex != null)
            {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mainTex)) as TextureImporter;
                importer.SetPlatformTextureSettings(matInfo[perMat].setting);
            }
            perMat.shader = matInfo[perMat].shader;
        }
    }


    private static void JudgeShader(string assetPath)
    {      
        Material loadMat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        string shaderName = loadMat.shader.name;
        string tempMatPath = null;
        Material tempMat = null;

        switch (shaderName)
        {

            case "KYU3D/Scene/PlantCut":
                SaveShaderAndTexSetting(loadMat);
                tempMatPath = assetPath.Replace(".mat", "_temp.mat");
                tempMat = CreateTempMateral(assetPath, tempMatPath);
                loadMat.shader = Shader.Find("KYU3D/Scene/PlantCutWithAlpha");
                loadMat = SolveShaderProperties(tempMat, loadMat);
                AssetDatabase.DeleteAsset(tempMatPath);
                break;

            case "KYU3D/Scene/PlantBlend":
                SaveShaderAndTexSetting(loadMat);
                tempMatPath = assetPath.Replace(".mat", "_temp.mat");
                tempMat = CreateTempMateral(assetPath, tempMatPath);
                loadMat.shader = Shader.Find("KYU3D/Scene/PlantBlendWithAlpha");
                loadMat = SolveShaderProperties(tempMat, loadMat);
                AssetDatabase.DeleteAsset(tempMatPath);
                break;

            case "KYU3D/Scene/PlantFlutterCut":
                SaveShaderAndTexSetting(loadMat);
                tempMatPath = assetPath.Replace(".mat", "_temp.mat");
                tempMat = CreateTempMateral(assetPath, tempMatPath);
                loadMat.shader = Shader.Find("KYU3D/Scene/PlantFlutterCutWithAlpha");
                loadMat = SolveShaderProperties(tempMat, loadMat);
                AssetDatabase.DeleteAsset(tempMatPath);
                break;
        }
    }


    private static Material CreateTempMateral(string assetPath, string tempMatPath)
    {      
        AssetDatabase.CopyAsset(assetPath, tempMatPath);
        Material originalTempMat = AssetDatabase.LoadAssetAtPath<Material>(tempMatPath);
        return originalTempMat;
    }


    //设置参数
    private static Material SolveShaderProperties(Material originalMat, Material mat)
    {
        int originalProLength = ShaderUtil.GetPropertyCount(originalMat.shader);
        int newProLength = ShaderUtil.GetPropertyCount(mat.shader);
        Dictionary<string,ShaderUtil.ShaderPropertyType> oldProperties = new Dictionary<string,ShaderUtil.ShaderPropertyType>();
        for(int i = 0;i<originalProLength;i++)
        {
            string properName = ShaderUtil.GetPropertyName(originalMat.shader, i);
            ShaderUtil.ShaderPropertyType proType =  ShaderUtil.GetPropertyType(originalMat.shader, i);
            if(!oldProperties.ContainsKey(properName))
            {
                oldProperties.Add(properName,proType);
            }
        }

        //获取分离alpha通道的路径
        string path = null;

        //进行属性值复制
        for (int i = 0; i < newProLength; i++)
        {
            string properName = ShaderUtil.GetPropertyName(mat.shader, i);
            ShaderUtil.ShaderPropertyType proType =  ShaderUtil.GetPropertyType(mat.shader, i);
            
            if(oldProperties.ContainsKey(properName))
            {
                if (proType == oldProperties[properName])
                {
                    switch (proType)
                    {

                        case ShaderUtil.ShaderPropertyType.Color:
                            Color col = originalMat.GetColor(properName);
                            mat.SetColor(properName, col);
                            break;

                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            if(properName == "_MainTex")
                            {
                                Texture2D tex = originalMat.GetTexture(properName) as Texture2D;
                                if (tex != null)
                                {
                                    tex = ChangeFormat(tex);

                                    Vector2 offset = originalMat.GetTextureOffset(properName);
                                    Vector2 scale = originalMat.GetTextureScale(properName);

                                    mat.SetTexture(properName, tex);
                                    mat.SetTextureOffset(properName, offset);
                                    mat.SetTextureScale(properName, scale);

                                    path = "Assets/" + ChannelSeperate.Seperate(tex as Texture2D).Split(new string[] { "/Assets/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                }
                            }                            
                            break;

                        case ShaderUtil.ShaderPropertyType.Float:
                            float floatVal = originalMat.GetFloat(properName);
                            mat.SetFloat(properName, floatVal);
                            break;

                        case ShaderUtil.ShaderPropertyType.Vector:
                            Vector4 vec = originalMat.GetVector(properName);
                            mat.SetVector(properName, vec);
                            break;

                        case ShaderUtil.ShaderPropertyType.Range:
                            float rangeVal = originalMat.GetFloat(properName);
                            mat.SetFloat(properName, rangeVal);
                            break;

                        default:
                            break;

                    }
                }                       
            }
            else if(proType == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                if (path != null)
                {
                    if(path.Contains("_alpha"))
                    {
                        Texture2D alpha = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                        TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(alpha)) as TextureImporter;
                        importer.SetPlatformTextureSettings(matInfo[mat].setting);
                        alpha = ChangeFormat(alpha);                        

                        mat.SetTexture(properName, alpha);
                    }                  
                }
            }
        }
        return mat;
    }


    //修改纹理格式为etc_4bit
    private static Texture2D ChangeFormat(Texture2D texture)
    {
        TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
        TextureImporterPlatformSettings textureSetting = importer.GetPlatformTextureSettings("Android");

        if(textureSetting.overridden == true && textureSetting.name == "Android")
        {
            textureSetting.format = TextureImporterFormat.ETC_RGB4;
        }
        
        importer.SetPlatformTextureSettings(textureSetting);
        importer.SaveAndReimport();
        return texture;
    }


    //保存materialShader和其mainTex设置
    public static void SaveShaderAndTexSetting(Material loadMat)
    {
        Texture2D texture = loadMat.GetTexture("_MainTex") as Texture2D;        
        TextureImporterPlatformSettings setting = MatShaderAndTextureSetting.GainTexSetting(texture);
        MatShaderAndTextureSetting matShaderAndTexSetting = new MatShaderAndTextureSetting(loadMat.shader, setting);
        if(!matInfo.ContainsKey(loadMat))
        {
            matInfo.Add(loadMat, matShaderAndTexSetting);

        }       
    }
}


//获取原材质的shader和mainTex设置
class MatShaderAndTextureSetting
{
    public Shader shader;
    public TextureImporterPlatformSettings setting;
    public MatShaderAndTextureSetting(Shader shader,TextureImporterPlatformSettings setting)
    {
        this.shader = shader;
        this.setting = setting;
    }


    //获取原主纹理格式
    public static TextureImporterPlatformSettings GainTexSetting(Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }
        TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
        TextureImporterPlatformSettings textureSetting = importer.GetPlatformTextureSettings("Android");
        return textureSetting;
    }
}
