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
    public class XFXGameLuaApiAssetWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(XFX.Game.LuaApi.Asset);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 10, 2, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateRoot", _m_CreateRoot_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateAudio", _m_CreateAudio_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateEffect", _m_CreateEffect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateTexture", _m_CreateTexture_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateText", _m_CreateText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateCanvas", _m_CreateCanvas_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreatePrimitive3D", _m_CreatePrimitive3D_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateCamera", _m_CreateCamera_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateLight", _m_CreateLight_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "root", _g_get_root);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "viewroot", _g_get_viewroot);
            
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "XFX.Game.LuaApi.Asset does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateRoot_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.IRoot gen_ret = XFX.Game.LuaApi.Asset.CreateRoot( _name, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateAudio_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.IAudio gen_ret = XFX.Game.LuaApi.Asset.CreateAudio( _filename, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateEffect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.IEffect gen_ret = XFX.Game.LuaApi.Asset.CreateEffect( _filename, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateTexture_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.ITexture gen_ret = XFX.Game.LuaApi.Asset.CreateTexture( _filename, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateText_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    
                        XFX.Asset.IText gen_ret = XFX.Game.LuaApi.Asset.CreateText( _filename );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateCanvas_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.ICanvas gen_ret = XFX.Game.LuaApi.Asset.CreateCanvas( _filename, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreatePrimitive3D_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _filename = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.IPrimitive3D gen_ret = XFX.Game.LuaApi.Asset.CreatePrimitive3D( _filename, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateCamera_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.ICamera gen_ret = XFX.Game.LuaApi.Asset.CreateCamera( _name, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateLight_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    XFX.Core.Render.IRenderObject _parent = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(XFX.Core.Render.IRenderObject));
                    
                        XFX.Asset.ILight gen_ret = XFX.Game.LuaApi.Asset.CreateLight( _name, _parent );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_root(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.PushAny(L, XFX.Game.LuaApi.Asset.root);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_viewroot(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.PushAny(L, XFX.Game.LuaApi.Asset.viewroot);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
