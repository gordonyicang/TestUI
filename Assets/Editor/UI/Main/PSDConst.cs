using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSDConst {
	public const string META_SUFFIX = ".meta";

	public const string FONT_PATH = "Assets/Resources/Font/";
	public const string FONT_SUFFIX = ".ttf";
	public static string ASSET_PATH_MICYAHEI = FONT_PATH + "yhFont" + FONT_SUFFIX;

	public const string UI_SCRIPT_PATH = "Assets/Editor/UI/UIScript/";

	public const string PREFAB_PATH = "Assets/Editor/UI/Perfab/";
	public const string PREFAB_SUFFIX = ".prefab";
	public static string ASSET_PATH_CANVAS = PREFAB_PATH + "Canvas" + PREFAB_SUFFIX;
	public static string ASSET_PATH_COMMON = PREFAB_PATH + "common" + PREFAB_SUFFIX;
	public static string ASSET_PATH_EVENTSYSTEM = PREFAB_PATH + "EventSystem" + PREFAB_SUFFIX;
	public static string ASSET_PATH_EMPTY = PREFAB_PATH + "Empty" + PREFAB_SUFFIX;
	public static string ASSET_PATH_RECTTRANSFORM = PREFAB_PATH + "RectTransform" + PREFAB_SUFFIX;
	public static string ASSET_PATH_IMAGE = PREFAB_PATH + "Image" + PREFAB_SUFFIX;
	public static string ASSET_PATH_BUTTON = PREFAB_PATH + "Button" + PREFAB_SUFFIX;
	public static string ASSET_PATH_BUTTON2 = PREFAB_PATH + "Button2" + PREFAB_SUFFIX;
	public static string ASSET_PATH_TEXT = PREFAB_PATH + "Text" + PREFAB_SUFFIX;
	public static string ASSET_PATH_TEXTFIELD = PREFAB_PATH + "InputField" + PREFAB_SUFFIX;
	public static string ASSET_PATH_RICHLABEL = PREFAB_PATH + "RichLabel" + PREFAB_SUFFIX;
	public static string ASSET_PATH_SLIDER = PREFAB_PATH + "Slider" + PREFAB_SUFFIX;
	public static string ASSET_PATH_PROGRESSBAR = PREFAB_PATH + "ProgressBar" + PREFAB_SUFFIX;
	public static string ASSET_PATH_SCROLL = PREFAB_PATH + "ScrollView" + PREFAB_SUFFIX;

	// 文本修复高度
	public const float TEXT_FIX_Y = 6;
	public const float TEXTFIELD_FIX_H = 3;

	// 标识
	// tabview
	public const string TAG_TABVIEW = "TabView";
	public const string TAG_TABVIEW_ITEM = "Item";

	// 图集
	public const string ATLAS_TEXTURE_EXE = "C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe";
	public const bool ATLAS_ISALPHA = true;
	public const string PNG_SUFFIX = ".png";
	public const string GUI_PATH = "Assets/Resources/GUI/";
	public const string OUT_PATH = "Assets/Resources/OutPic/";
	public static string ATLAS_PATH_DEFAULT = GUI_PATH +　"common/";
	public static string ATLAS_PATH_COMMON = ATLAS_PATH_DEFAULT + "common" + PNG_SUFFIX;
	public static string ATLAS_PATH_COMMON_TEMP = "Assets/AtlasCommon/common/";

	// 材质
	public const string SHADER_PATH = "Assets/Resources/Shader/PSD2UGUI/";
	public const string SHADER_SUFFIX = ".shader";
	public static string SHADER_PATH_SPLIT_ALPHA = SHADER_PATH + "PSD2UGUI_SPLIT_ALPHA" + SHADER_SUFFIX;

	//锚点类型	
	public static Vector2 PIVOTPOINT0 = new Vector2(0.5f,0.5f);// center
	public static Vector2 PIVOTPOINT1 = new Vector2(0.0f,1.0f);// topLeft
	public static Vector2 PIVOTPOINT2 = new Vector2(0.5f,1.0f);// topCenter
	public static Vector2 PIVOTPOINT3 = new Vector2(1.0f,1.0f);// topRight
	public static Vector2 PIVOTPOINT4 = new Vector2(0.0f,0.5f);// centerLeft
	public static Vector2 PIVOTPOINT5 = new Vector2(1.0f,0.5f);// centerRight
	public static Vector2 PIVOTPOINT6 = new Vector2(0.0f,0.0f);// bottomLeft
	public static Vector2 PIVOTPOINT7 = new Vector2(1.0f,0.0f);// bottomRight
	public static Vector2 PIVOTPOINT8 = new Vector2(0.5f,0.0f);// bottomCenter
	
}
