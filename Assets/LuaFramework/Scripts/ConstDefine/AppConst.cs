﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LuaFramework {
    public class AppConst {
		public static bool IsEditor = false;
		public static bool DebugMode = true;                       //调试模式-用于内部测试
        /// <summary>
        /// 如果想删掉框架自带的例子，那这个例子模式必须要
        /// 关闭，否则会出现一些错误。
        /// </summary>
		public static bool ExampleMode = false;                       //例子模式 

        /// <summary>
        /// 如果开启更新模式，前提必须启动框架自带服务器端。
        /// 否则就需要自己将StreamingAssets里面的所有内容
        /// 复制到自己的Webserver上面，并修改下面的WebUrl。
        /// </summary>
		public static bool UpdateMode = false;                       //更新模式-默认关闭 
		public static bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
		public static bool LuaBundleMode = false;                    //Lua代码AssetBundle模式

        public const int TimerInterval = 1;
        public const int GameFrameRate = 30;                        //游戏帧频

        public const string AppName = "LuaFramework";               //应用程序名称
        public const string LuaTempDir = "Lua/";                    //临时目录
        public const string AppPrefix = AppName + "_";              //应用程序前缀
        public const string ExtName = ".unity3d";                   //素材扩展名
        public const string AssetDir = "StreamingAssets";           //素材目录 
        public const string WebUrl = "http://localhost:6688/";      //测试更新地址
		public static string editorLuaOutputDir = "";             //编辑器模式下，编译后的Lua输出目录

        public static string UserId = string.Empty;                 //用户ID
        public static int SocketPort = 0;                           //Socket服务器端口
		public static string SocketAddress = string.Empty;          //Socket服务器地址public static string isReleaseMode = "";                  //-1|0:开发模式(普通),1:发布模式",2:ArtResouce编辑器模式
		public static string isReleaseMode = "";                  //-1|0:开发模式(普通),1:发布模式",2:ArtResouce编辑器模式
		public static string luaCodePath = "";

        public static string FrameworkRoot {
            get {
                return Application.dataPath + "/" + AppName;
            }
        }
    }
}