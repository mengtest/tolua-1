﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class System_Collections_Generic_Dictionary_int_TestAccount_EnumeratorWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator), null, "Enumerator");
		L.RegFunction("MoveNext", new LuaCSFunction(MoveNext));
		L.RegFunction("Dispose", new LuaCSFunction(Dispose));
		L.RegFunction("New", new LuaCSFunction(_CreateSystem_Collections_Generic_Dictionary_int_TestAccount_Enumerator));
		L.RegFunction("__tostring", new LuaCSFunction(ToLua.op_ToString));
		L.RegVar("Current", new LuaCSFunction(get_Current), null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateSystem_Collections_Generic_Dictionary_int_TestAccount_Enumerator(IntPtr L)
	{
		System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator obj = new System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator();
		ToLua.PushValue(L, obj);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MoveNext(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator obj = StackTraits<System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator>.Check(L, 1);
			bool o = obj.MoveNext();
			LuaDLL.lua_pushboolean(L, o);
			ToLua.SetBack(L, 1, obj);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Dispose(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator obj = StackTraits<System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator>.Check(L, 1);
			obj.Dispose();
			ToLua.SetBack(L, 1, obj);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Current(IntPtr L)
	{
		System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator obj = default(System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator);
		try
		{
			obj = ToLua.ToGenericObject<System.Collections.Generic.Dictionary<int,TestAccount>.Enumerator>(L, 1);
			System.Collections.Generic.KeyValuePair<int,TestAccount> ret = obj.Current;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, obj, "attempt to index Current on a nil value");
		}
	}
}

