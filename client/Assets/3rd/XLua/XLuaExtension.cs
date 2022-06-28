/********************************************************
    Id: XLuaExtension.cs
    Desc: 
    Author: figo
    Date: 2020-08-19 15:12:01
*********************************************************/
#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif


namespace XLua {
    using System;

    public partial class ObjectTranslator {
        public void PushAny<T>(RealStatePtr L, T o) 
            where T : ZF.Core.Render.IRenderObject { PushAny(L, typeof(T), o); }
        public void PushAny(RealStatePtr L, ZF.Core.Render.IRenderComponent o) { PushAny(L, typeof(ZF.Core.Render.IRenderComponent), o); }
        public void PushByType(RealStatePtr L, ZF.Core.Render.IRenderObject v) { PushAny(L, v); }
        public void PushByType(RealStatePtr L, ZF.Core.Render.IRenderComponent v) { PushAny(L, v); }

        private void PushAny(RealStatePtr L, Type type, object o ) {
            if (o == null) {
                LuaAPI.lua_pushnil(L);
                return;
            }

            int index = -1;
            bool needcache = true;
            if (needcache && reverseMap.TryGetValue(o, out index)) {
                if (LuaAPI.xlua_tryget_cachedud(L, index, cacheRef) == 1) {
                    return;
                }
            }

            bool is_first;
            int type_id = getTypeId(L, type, out is_first);

            //如果一个type的定义含本身静态readonly实例时，getTypeId会push一个实例，这时候应该用这个实例
            if (is_first && needcache && reverseMap.TryGetValue(o, out index)) {
                if (LuaAPI.xlua_tryget_cachedud(L, index, cacheRef) == 1) {
                    return;
                }
            }

            index = addObject(o, false, false);
            LuaAPI.xlua_pushcsobj(L, index, type_id, needcache, cacheRef);
        }
    }

    // 请注释XLua/src/TypeExtensions.cs文件中BaseType函数
    public static class TypeExtensions2  {
        public static Type BaseType(this Type type) {
            if (type.IsInterface) {
                if (typeof(ZF.Core.Render.IRenderObject).IsAssignableFrom(type) ||
                    typeof(ZF.Core.Render.IRenderComponent).IsAssignableFrom(type)) {
                    var types = type.GetInterfaces();
                    if (types.Length > 0) {
                        return types[0];
                    } else {
                        return null;
                    }
                }
            }
#if !UNITY_WSA || UNITY_EDITOR
            return type.BaseType;
#else
            return type.GetTypeInfo().BaseType;
#endif
        }
    }

	public static partial class Utils {
        public class Dummy  { public static implicit operator LuaCSFunction(Dummy value) { return null; } }

		public static void EndObjectRegister(Type type, RealStatePtr L, ObjectTranslator translator, LuaCSFunction csIndexer,
			LuaCSFunction csNewIndexer, Type base_type, LuaCSFunction arrayIndexer, Dummy arrayNewIndexer)
        {
            if (type == typeof(ZF.Core.Render.IRenderObject)) {
                Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddComponent", _m_AddComponent);
                Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveComponent", _m_RemoveComponent);
                Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetComponent", _m_GetComponent);
            }
            EndObjectRegister(type, L, translator, csIndexer, csNewIndexer, base_type,  arrayIndexer, (LuaCSFunction)arrayIndexer);
        }


        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddComponent(RealStatePtr L) {
            try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
                ZF.Core.Render.IRenderObject gen_to_be_invoked = (ZF.Core.Render.IRenderObject)translator.FastGetCSObj(L, 1);
                int gen_param_count = LuaAPI.lua_gettop(L);
                if (gen_param_count == 2 && translator.Assignable<System.Type>(L, 2)) {
                    System.Type _type = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    ZF.Core.Render.IRenderComponent gen_ret = gen_to_be_invoked.AddComponent(_type);

                    translator.PushAny(L, gen_ret);
                    return 1;
                } if (gen_param_count >= 2 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TTABLE) && (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 3) || translator.Assignable<object>(L, 3))) {
                    XLua.LuaTable _cls = (XLua.LuaTable)translator.GetObject(L, 2, typeof(XLua.LuaTable));
                    object[] _args = translator.GetParams<object>(L, 3);

                    object gen_ret = gen_to_be_invoked.AddComponent(_cls, _args);
                    translator.PushAny(L, gen_ret);

                    return 1;
                }
            } catch (System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }

            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Core.Render.IRenderObject.AddComponent!");
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveComponent(RealStatePtr L) {
            try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
                ZF.Core.Render.IRenderObject gen_to_be_invoked = (ZF.Core.Render.IRenderObject)translator.FastGetCSObj(L, 1);

                int gen_param_count = LuaAPI.lua_gettop(L);

                if (gen_param_count == 2 && translator.Assignable<System.Type>(L, 2)) {
                    System.Type _type = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    gen_to_be_invoked.RemoveComponent(_type);

                    return 0;
                }
                if (gen_param_count == 2 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TTABLE)) {
                    XLua.LuaTable _cls = (XLua.LuaTable)translator.GetObject(L, 2, typeof(XLua.LuaTable));
                    gen_to_be_invoked.RemoveComponent(_cls);

                    return 0;
                }
            } catch (System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }

            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Core.Render.IRenderObject.RemoveComponent!");
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetComponent(RealStatePtr L) {
            try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
                ZF.Core.Render.IRenderObject gen_to_be_invoked = (ZF.Core.Render.IRenderObject)translator.FastGetCSObj(L, 1);

                int gen_param_count = LuaAPI.lua_gettop(L);

                if (gen_param_count == 2 && translator.Assignable<System.Type>(L, 2)) {
                    System.Type _type = (System.Type)translator.GetObject(L, 2, typeof(System.Type));

                    ZF.Core.Render.IRenderComponent gen_ret = gen_to_be_invoked.GetComponent(_type);
                    translator.PushAny(L, gen_ret);

                    return 1;
                } 
                if (gen_param_count == 2 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TTABLE)) {
                    XLua.LuaTable _cls = (XLua.LuaTable)translator.GetObject(L, 2, typeof(XLua.LuaTable));

                    object gen_ret = gen_to_be_invoked.GetComponent(_cls);
                    translator.PushAny(L, gen_ret);

                    return 1;
                }
            } catch (System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }

            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Core.Render.IRenderObject.GetComponent!");
        }
    }

    public partial class LuaTable {
        public void Set(string key, ZF.Core.Render.IRenderObject value) {
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock) {
#endif
                var L = luaEnv.L;
                int oldTop = LuaAPI.lua_gettop(L);
                var translator = luaEnv.translator;

                LuaAPI.lua_getref(L, luaReference);
                translator.PushByType(L, key);
                translator.PushByType(L, value);

                if (0 != LuaAPI.xlua_psettable(L, -3)) {
                    luaEnv.ThrowExceptionFromError(oldTop);
                }
                LuaAPI.lua_settop(L, oldTop);
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
        }
        public void Set(string key, ZF.Core.Render.IRenderComponent value) {
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock) {
#endif
                var L = luaEnv.L;
                int oldTop = LuaAPI.lua_gettop(L);
                var translator = luaEnv.translator;

                LuaAPI.lua_getref(L, luaReference);
                translator.PushByType(L, key);
                translator.PushByType(L, value);

                if (0 != LuaAPI.xlua_psettable(L, -3)) {
                    luaEnv.ThrowExceptionFromError(oldTop);
                }
                LuaAPI.lua_settop(L, oldTop);
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
        }
    }
}
