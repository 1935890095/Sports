#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class XFXGameLuaApiSceneWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(XFX.Game.LuaApi.Scene);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 7, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Setup", _m_Setup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetMapRootProperty", _m_GetMapRootProperty_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadScene", _m_LoadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadScene", _m_UnloadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadAllScene", _m_UnloadAllScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast", _m_Raycast_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "XFX.Game.LuaApi.Scene does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Setup_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    XFX.Game.LuaApi.Scene.Setup(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetMapRootProperty_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _componentName = LuaAPI.lua_tostring(L, 1);
                    string _propName = LuaAPI.lua_tostring(L, 2);
                    
                        object gen_ret = XFX.Game.LuaApi.Scene.GetMapRootProperty( _componentName, _propName );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action<string, bool, bool, float>>(L, 4)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    bool _synchronize = LuaAPI.lua_toboolean(L, 2);
                    bool _additive = LuaAPI.lua_toboolean(L, 3);
                    System.Action<string, bool, bool, float> _onInvoke = translator.GetDelegate<System.Action<string, bool, bool, float>>(L, 4);
                    
                    XFX.Game.LuaApi.Scene.LoadScene( _name, _synchronize, _additive, _onInvoke );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    bool _synchronize = LuaAPI.lua_toboolean(L, 2);
                    bool _additive = LuaAPI.lua_toboolean(L, 3);
                    
                    XFX.Game.LuaApi.Scene.LoadScene( _name, _synchronize, _additive );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    bool _synchronize = LuaAPI.lua_toboolean(L, 2);
                    
                    XFX.Game.LuaApi.Scene.LoadScene( _name, _synchronize );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                    XFX.Game.LuaApi.Scene.LoadScene( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to XFX.Game.LuaApi.Scene.LoadScene!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                    XFX.Game.LuaApi.Scene.UnloadScene( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadAllScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    XFX.Game.LuaApi.Scene.UnloadAllScene(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector3>(L, 1)&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    UnityEngine.Vector3 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector3 _dir;translator.Get(L, 2, out _dir);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    float _maxDistance = (float)LuaAPI.lua_tonumber(L, 4);
                    
                        object gen_ret = XFX.Game.LuaApi.Scene.Raycast( _origin, _dir, _func, _maxDistance );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector3>(L, 1)&& translator.Assignable<UnityEngine.Vector3>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    UnityEngine.Vector3 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector3 _dir;translator.Get(L, 2, out _dir);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    
                        object gen_ret = XFX.Game.LuaApi.Scene.Raycast( _origin, _dir, _func );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to XFX.Game.LuaApi.Scene.Raycast!");
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
