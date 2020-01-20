﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameLib_ui_LinkImageTextWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameLib.ui.LinkImageText), typeof(UnityEngine.UI.Text));
		L.RegFunction("SetVerticesDirty", SetVerticesDirty);
		L.RegFunction("OnPointerClick", OnPointerClick);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("m_positionList", get_m_positionList, set_m_positionList);
		L.RegVar("funLoadSprite", get_funLoadSprite, set_funLoadSprite);
		L.RegVar("textHeight", get_textHeight, set_textHeight);
		L.RegVar("onLinkClick", get_onLinkClick, set_onLinkClick);
		L.RegVar("preferredHeight", get_preferredHeight, null);
		L.RegVar("preferredWidth", get_preferredWidth, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetVerticesDirty(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)ToLua.CheckObject<GameLib.ui.LinkImageText>(L, 1);
			obj.SetVerticesDirty();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnPointerClick(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)ToLua.CheckObject<GameLib.ui.LinkImageText>(L, 1);
			UnityEngine.EventSystems.PointerEventData arg0 = (UnityEngine.EventSystems.PointerEventData)ToLua.CheckObject<UnityEngine.EventSystems.PointerEventData>(L, 2);
			obj.OnPointerClick(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_positionList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			System.Collections.Generic.List<UnityEngine.Vector2> ret = obj.m_positionList;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index m_positionList on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_funLoadSprite(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			System.Func<string,UnityEngine.GameObject> ret = obj.funLoadSprite;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index funLoadSprite on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_textHeight(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			int ret = obj.textHeight;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index textHeight on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onLinkClick(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			GameLib.ui.HrefClickEvent ret = obj.onLinkClick;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index onLinkClick on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_preferredHeight(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			float ret = obj.preferredHeight;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index preferredHeight on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_preferredWidth(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			float ret = obj.preferredWidth;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index preferredWidth on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_positionList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			System.Collections.Generic.List<UnityEngine.Vector2> arg0 = (System.Collections.Generic.List<UnityEngine.Vector2>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
			obj.m_positionList = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index m_positionList on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_funLoadSprite(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			System.Func<string,UnityEngine.GameObject> arg0 = (System.Func<string,UnityEngine.GameObject>)ToLua.CheckDelegate<System.Func<string,UnityEngine.GameObject>>(L, 2);
			obj.funLoadSprite = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index funLoadSprite on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_textHeight(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.textHeight = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index textHeight on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onLinkClick(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameLib.ui.LinkImageText obj = (GameLib.ui.LinkImageText)o;
			GameLib.ui.HrefClickEvent arg0 = (GameLib.ui.HrefClickEvent)ToLua.CheckObject<GameLib.ui.HrefClickEvent>(L, 2);
			obj.onLinkClick = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index onLinkClick on a nil value");
		}
	}
}
