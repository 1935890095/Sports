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
    public class ZFGameLuaApiCacheWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Game.LuaApi.Cache);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 10, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Get", _m_Get_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Save", _m_Save_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Exists", _m_Exists_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadProtobuf", _m_LoadProtobuf_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetServerUrl", _m_GetServerUrl_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetUserId", _m_GetUserId_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetConfig", _m_GetConfig_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SendMail", _m_SendMail_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DeleteDirectory", _m_DeleteDirectory_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Game.LuaApi.Cache does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Get_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = ZF.Game.LuaApi.Cache.Get( _fileName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Save_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    string _content = LuaAPI.lua_tostring(L, 2);
                    bool _cachePath = LuaAPI.lua_toboolean(L, 3);
                    
                    ZF.Game.LuaApi.Cache.Save( _fileName, _content, _cachePath );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    string _content = LuaAPI.lua_tostring(L, 2);
                    
                    ZF.Game.LuaApi.Cache.Save( _fileName, _content );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Game.LuaApi.Cache.Save!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Exists_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    
                        bool gen_ret = ZF.Game.LuaApi.Cache.Exists( _fileName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadProtobuf_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    
                        byte[] gen_ret = ZF.Game.LuaApi.Cache.LoadProtobuf( _fileName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetServerUrl_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = ZF.Game.LuaApi.Cache.GetServerUrl(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetUserId_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = ZF.Game.LuaApi.Cache.GetUserId(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetConfig_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 0) 
                {
                    
                        string gen_ret = ZF.Game.LuaApi.Cache.GetConfig(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = ZF.Game.LuaApi.Cache.GetConfig( _name );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Game.LuaApi.Cache.GetConfig!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendMail_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _dress = LuaAPI.lua_tostring(L, 1);
                    string _title = LuaAPI.lua_tostring(L, 2);
                    
                    ZF.Game.LuaApi.Cache.SendMail( _dress, _title );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DeleteDirectory_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                    ZF.Game.LuaApi.Cache.DeleteDirectory( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
