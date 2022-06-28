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
    public class ZFAssetIVideoPlayerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Asset.IVideoPlayer);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 7, 3);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddClip", _m_AddClip);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Play", _m_Play);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Pause", _m_Pause);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Stop", _m_Stop);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "player", _g_get_player);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "renderTexture", _g_get_renderTexture);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "waitForFirstFrame", _g_get_waitForFirstFrame);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "skipOnDrop", _g_get_skipOnDrop);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "loop", _g_get_loop);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "frameRate", _g_get_frameRate);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isPlaying", _g_get_isPlaying);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "waitForFirstFrame", _s_set_waitForFirstFrame);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "skipOnDrop", _s_set_skipOnDrop);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "loop", _s_set_loop);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Asset.IVideoPlayer does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddClip(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.AddClip( _filename );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Play(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Play(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Pause(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Pause(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Stop(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    bool _removeClip = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.Stop( _removeClip );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1) 
                {
                    
                    gen_to_be_invoked.Stop(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Asset.IVideoPlayer.Stop!");
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_player(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.player);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_renderTexture(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                translator.PushAny(L, gen_to_be_invoked.renderTexture);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_waitForFirstFrame(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.waitForFirstFrame);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_skipOnDrop(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.skipOnDrop);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_loop(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.loop);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_frameRate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.frameRate);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isPlaying(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isPlaying);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_waitForFirstFrame(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.waitForFirstFrame = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_skipOnDrop(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.skipOnDrop = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_loop(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IVideoPlayer gen_to_be_invoked = (ZF.Asset.IVideoPlayer)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.loop = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
