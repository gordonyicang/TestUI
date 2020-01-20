
--协议类型--
ProtocalType = 
{
	BINARY = 0,
	PB_LUA = 1,
	PBC    = 2,
	SPROTO = 3,
}

--资源加载优先级
ResPriority =
{
    Normal      = 500,       --默认优先级
    Effect      = 600,       --普通特效加载
    Anim        = 700,       --动画|动画控制器 
    Npc         = 800,       --NPC模型加载 
    Item        = 900,       --场景模型加载
    Collect     = 1000,      --采集物       
    Monster     = 1100,      --怪物模型加载
    Role        = 1200,      --角色模型加载
    MapObject   = 1300,      --场景物件加载
    Teleport    = 1400,      --传送门
    UI          = 2000,      --UI加载
    Skill       = 2100,      --技能加载
    Me          = 2200,      --玩家自身加载
    System      = 9000       --系统加载
}


--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;
--全局事件管理器
EventDispatcher = require 'Common/events';

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;

--GC = System.GC;
WWW = UnityEngine.WWW;
Sprite = UnityEngine.Sprite;
Resources = UnityEngine.Resources;
GameObject = UnityEngine.GameObject;
Application = UnityEngine.Application;
RuntimePlatform = UnityEngine.RuntimePlatform;
RenderSettings = UnityEngine.RenderSettings;
LightmapSettings = UnityEngine.LightmapSettings;
StaticBatchingUtility = UnityEngine.StaticBatchingUtility;
RectTransformUtility  = UnityEngine.RectTransformUtility;
CameraClearFlags = UnityEngine.CameraClearFlags;
AnimatorCullingMode = UnityEngine.AnimatorCullingMode;
PointerInputModule  = UnityEngine.EventSystems.PointerInputModule;

Image = UnityEngine.UI.Image;
Shader = UnityEngine.Shader;
Camera = UnityEngine.Camera;
Screen = UnityEngine.Screen;
FogMode = UnityEngine.FogMode;
Physics = UnityEngine.Physics;
LayerMask = UnityEngine.LayerMask;
RenderMode = UnityEngine.RenderMode;
Input = UnityEngine.Input;
KeyCode = UnityEngine.KeyCode;
-- LineType= UnityEngine.UI.InputField.LineType;
EventSystem = UnityEngine.EventSystems.EventSystem;
HorizontalWrapMode = UnityEngine.HorizontalWrapMode;

-- ScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode;
AnimatorStateInfo = UnityEngine.AnimatorStateInfo;
AudioRolloffMode = UnityEngine.AudioRolloffMode;
CollisionFlags = UnityEngine.CollisionFlags;
Matrix4x4 = UnityEngine.Matrix4x4;
Profiler = UnityEngine.Profiler; 
PrimitiveType = UnityEngine.PrimitiveType;

DelegateMgr = GameLoader.DelegateMgr.instance;
-- CSEventDispatcher = GameLoader.Event.CSEventDispatcher.instance;
-- JComponentAudioCollective = GameUI.Components.JComponentAudioCollective;
-- SortOrderedRenderAgent = GameClient.Base.ExtendComponent.SortOrderedRenderAgent;
AniSystemUtils = GameClient.Base.Utils.AniSystemUtils;
BitUtils = GameClient.Base.Utils.BitUtils;
-- FileAccessMgr = GameLoader.FileSystem.FileAccessMgr;
-- GameFileSys = GameLoader.FileSystem.Instance;
-- HttpUtils = GameClient.Base.Utils.HttpUtils;
-- ImageWrapper = Game.UI.ImageWrapper;
CanvasGroup = UnityEngine.CanvasGroup;
-- TextWrapper = Game.UI.TextWrapper;
-- Outline = Game.UI.Outline;
TextAnchor = UnityEngine.TextAnchor;
-- AssetBridge = Game.AssetBridge.AssetBridge;

-- PlatformSdkMgr = GameClient.PlatformSdk.PlatformSdkMgr.instance;


--全局常量
oneVector3 = Vector3.one;
zeroVector3 = Vector3.zero;
identity = Quaternion.identity;