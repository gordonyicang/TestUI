﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_CollisionFlagsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(UnityEngine.CollisionFlags));
		L.RegVar("None", get_None, null);
		L.RegVar("Sides", get_Sides, null);
		L.RegVar("Above", get_Above, null);
		L.RegVar("Below", get_Below, null);
		L.RegVar("CollidedSides", get_CollidedSides, null);
		L.RegVar("CollidedAbove", get_CollidedAbove, null);
		L.RegVar("CollidedBelow", get_CollidedBelow, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
		TypeTraits<UnityEngine.CollisionFlags>.Check = CheckType;
		StackTraits<UnityEngine.CollisionFlags>.Push = Push;
	}

	static void Push(IntPtr L, UnityEngine.CollisionFlags arg)
	{
		ToLua.Push(L, arg);
	}

	static bool CheckType(IntPtr L, int pos)
	{
		return TypeChecker.CheckEnumType(typeof(UnityEngine.CollisionFlags), L, pos);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_None(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.None);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Sides(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.Sides);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Above(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.Above);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Below(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.Below);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CollidedSides(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.CollidedSides);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CollidedAbove(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.CollidedAbove);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CollidedBelow(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.CollisionFlags.CollidedBelow);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		UnityEngine.CollisionFlags o = (UnityEngine.CollisionFlags)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}
