using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpriteManager
{
	/**
	 * 转换精灵图片
	 * 
	 * path 图片路径
	 * 
	 * 注:拷贝后的图片,textureImporter返回null
	*/
	public static void conversion(string path)
	{
		TextureImporter texture = AssetImporter.GetAtPath (path) as TextureImporter;
		texture.textureType = TextureImporterType.Sprite;
		texture.mipmapEnabled = false;
		texture.maxTextureSize = 2048;
		AssetDatabase.WriteImportSettingsIfDirty (path);
		AssetDatabase.ImportAsset (path);
	}

	/**
	 * 生成精灵图片
	 * 
	 * path 图片路径
	 * 
	 * 注:转换精灵图片失效的情况下,可以考虑这个方法
	*/
	public static Sprite create(string path)
	{
		AssetDatabase.WriteImportSettingsIfDirty (path);
		AssetDatabase.ImportAsset (path);
		Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D> (path);
		Rect rect = new Rect (0,0,texture.width,texture.height);
		Vector2 anchor = new Vector2 (0.0f, 1.0f);
		return Sprite.Create (texture,rect,anchor);
	}
}

