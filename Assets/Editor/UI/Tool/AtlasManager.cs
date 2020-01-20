using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AtlasManager {
	//private static bool Split_Alpha = true;

	/** 初始化图集（SpritePacker）

	参数1：图片路径
	参数2：图集标记
	*/
	public static TextureImporter InitAtlas(string path,string tag) {
		TextureImporter texture = AssetImporter.GetAtPath (path) as TextureImporter;
		texture.textureType = TextureImporterType.Sprite;
		texture.mipmapEnabled = false;
		texture.maxTextureSize = 2048;
		texture.spritePackingTag = tag;
		AssetDatabase.WriteImportSettingsIfDirty (path);
		AssetDatabase.ImportAsset (path);

		return texture;
	}

	/** 初始化图集（TexturePacker）
	 * 
	 * srcPath 来源文件夹路径
	 * tarPath 图集保存路径
	*/
	public static bool InitAtlasForTextureP(string srcPath,string tarPath) {
		string tarDir = Path.GetDirectoryName(tarPath); // 图集保存目录
		string delExtTarPath = tarDir + "/" + Path.GetFileNameWithoutExtension(tarPath); // 去后缀文件路径
		if (!Directory.Exists(srcPath)) {
			Debug.Log ("###TexturePacker error:来源文件夹不存在");
			return false;
		}
		if (!Directory.Exists (tarDir)) {
			Directory.CreateDirectory (tarDir);
		}
		// TexturePacker.exe路径
		string texturePacker = PSDConst.ATLAS_TEXTURE_EXE;
		// 命令行格式 CropKeepPos
		string commandText = " --sheet {0}.png --data {1}.tpsheet --format unity-texture2d --trim-mode None --trim-threshold 6 --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT  --disable-rotation --scale 1 {2} --force-squared";
		bool isSave = TerminalManager.cmd (texturePacker,string.Format(commandText,delExtTarPath,delExtTarPath,srcPath));
		if (isSave) {
			// 构建透明
			if (PSDConst.ATLAS_ISALPHA) GenerateAlpha(tarPath);
			// 构建材质
			CreateMaterials(tarPath);
		}
		return isSave;
	}

	/** 构建透明
	 * 
	 * path 
	*/
	public static void GenerateAlpha(string path) {
		string tarDir = Path.GetDirectoryName(path); // 图集保存目录
		string delExtTarPath = tarDir + "/" + Path.GetFileNameWithoutExtension(path); // 去后缀文件路径
		ImageChannelSpliterWrapper.Execute(delExtTarPath);
	}

	/** 构建材质
	 * 
	 * path 图片路径
	*/
	public static void CreateMaterials(string path) {
		Shader shader = AssetDatabase.LoadAssetAtPath(PSDConst.SHADER_PATH_SPLIT_ALPHA, typeof(Shader)) as Shader;
		Material material = new Material(shader);
		Texture2D texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		Texture2D alphaTexture = AssetDatabase.LoadAssetAtPath(GetAlphaPngPath(path), typeof(Texture2D)) as Texture2D;
		material.SetTexture("_MainTex", texture);
		if (PSDConst.ATLAS_ISALPHA)
		{
			material.SetTexture("_AlphaTex", alphaTexture);
		}
		Debug.Log("最终保存图片 === "+path);
		AssetDatabase.CreateAsset(material, path.Replace(".png", "_etc.mat"));
		AssetDatabase.SaveAssets();
	}

	private static string GetAlphaPngPath(string pngPath)
	{
		return pngPath.Replace(".png", "_alpha.png");
	}

	/** 从图集中获取小图（TexturePacker）

	path 图集绝对路径
	name 小图名称
	*/
	public static Sprite getSpriteForTextureP (string path,string name)
	{
		bool isFind = false;
		Object[] atlas = AssetDatabase.LoadAllAssetsAtPath (path);
		foreach (Object img in atlas) {
			if (img.name != name) {
				continue;
			}
			isFind = true;
			return (Sprite)img;
		}

		if (!isFind) {
			//Debug.Log ("图集"+path+"查询不到"+name);
		}
		return null;
	}

	/** 材质路径转换
	 * 
	 * path 图集路径
	*/
	public static Material getMaterForAtlasPath(string path) {
		return AssetDatabase.LoadAssetAtPath (path.Replace(".png","_etc.mat"),typeof(Material)) as Material;
	}
}
