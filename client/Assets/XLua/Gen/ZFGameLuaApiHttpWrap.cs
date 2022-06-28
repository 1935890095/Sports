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
    public class ZFGameLuaApiHttpWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Game.LuaApi.Http);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 7, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "SetHeader", _m_SetHeader_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetTimeout", _m_SetTimeout_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetHold", _m_SetHold_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Get", _m_Get_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Post", _m_Post_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetTextureByUrl", _m_GetTextureByUrl_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Game.LuaApi.Http does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetHeader_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    string _value = LuaAPI.lua_tostring(L, 2);
                    
                    ZF.Game.LuaApi.Http.SetHeader( _name, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetTimeout_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _timeout = LuaAPI.xlua_tointeger(L, 1);
                    
                    ZF.Game.LuaApi.Http.SetTimeout( _timeout );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetHold_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    bool _hold = LuaAPI.lua_toboolean(L, 1);
                    
                    ZF.Game.LuaApi.Http.SetHold( _hold );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Get_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    System.Action<byte[]> _responseHandler = translator.GetDelegate<System.Action<byte[]>>(L, 2);
                    System.Action<string> _errorHandler = translator.GetDelegate<System.Action<string>>(L, 3);
                    bool _now = LuaAPI.lua_toboolean(L, 4);
                    
                    ZF.Game.LuaApi.Http.Get( _path, _responseHandler, _errorHandler, _now );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Post_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    byte[] _body = LuaAPI.lua_tobytes(L, 2);
                    System.Action<byte[]> _responseHandler = translator.GetDelegate<System.Action<byte[]>>(L, 3);
                    System.Action<string> _errorHandler = translator.GetDelegate<System.Action<string>>(L, 4);
                    bool _now = LuaAPI.lua_toboolean(L, 5);
                    
                    ZF.Game.LuaApi.Http.Post( _path, _body, _responseHandler, _errorHandler, _now );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTextureByUrl_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 8&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 6)&& translator.Assignable<System.Action<UnityEngine.Texture2D>>(L, 7)&& translator.Assignable<System.Action<string>>(L, 8)) 
                {
                    string _url = LuaAPI.lua_tostring(L, 1);
                    bool _isReadCache = LuaAPI.lua_toboolean(L, 2);
                    bool _isSaveCache = LuaAPI.lua_toboolean(L, 3);
                    string _cacheName = LuaAPI.lua_tostring(L, 4);
                    int _width = LuaAPI.xlua_tointeger(L, 5);
                    int _height = LuaAPI.xlua_tointeger(L, 6);
                    System.Action<UnityEngine.Texture2D> _callback = translator.GetDelegate<System.Action<UnityEngine.Texture2D>>(L, 7);
                    System.Action<string> _errorCallback = translator.GetDelegate<System.Action<string>>(L, 8);
                    
                    ZF.Game.LuaApi.Http.GetTextureByUrl( _url, _isReadCache, _isSaveCache, _cacheName, _width, _height, _callback, _errorCallback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 7&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 6)&& translator.Assignable<System.Action<UnityEngine.Texture2D>>(L, 7)) 
                {
                    string _url = LuaAPI.lua_tostring(L, 1);
                    bool _isReadCache = LuaAPI.lua_toboolean(L, 2);
                    bool _isSaveCache = LuaAPI.lua_toboolean(L, 3);
                    string _cacheName = LuaAPI.lua_tostring(L, 4);
                    int _width = LuaAPI.xlua_tointeger(L, 5);
                    int _height = LuaAPI.xlua_tointeger(L, 6);
                    System.Action<UnityEngine.Texture2D> _callback = translator.GetDelegate<System.Action<UnityEngine.Texture2D>>(L, 7);
                    
                    ZF.Game.LuaApi.Http.GetTextureByUrl( _url, _isReadCache, _isSaveCache, _cacheName, _width, _height, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Game.LuaApi.Http.GetTextureByUrl!");
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
