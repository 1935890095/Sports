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
    public class XFXGameLuaApiActionWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(XFX.Game.LuaApi.Action);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 6, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Play", _m_Play_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Stop", _m_Stop_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShowArea", _m_ShowArea_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HideArea", _m_HideArea_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveArea", _m_RemoveArea_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "XFX.Game.LuaApi.Action does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Play_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 5&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<XFX.Asset.IRoot>(L, 2)&& translator.Assignable<System.Func<string, XFX.Asset.IRole[]>>(L, 3)&& translator.Assignable<System.Func<string, XFX.Asset.IRole[]>>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    XFX.Asset.IRoot _root = (XFX.Asset.IRoot)translator.GetObject(L, 2, typeof(XFX.Asset.IRoot));
                    System.Func<string, XFX.Asset.IRole[]> _onFire = translator.GetDelegate<System.Func<string, XFX.Asset.IRole[]>>(L, 3);
                    System.Func<string, XFX.Asset.IRole[]> _onHit = translator.GetDelegate<System.Func<string, XFX.Asset.IRole[]>>(L, 4);
                    float _speed = (float)LuaAPI.lua_tonumber(L, 5);
                    
                        XFX.Asset.IAction gen_ret = XFX.Game.LuaApi.Action.Play( _name, _root, _onFire, _onHit, _speed );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<XFX.Asset.IRoot>(L, 2)&& translator.Assignable<System.Func<string, XFX.Asset.IRole[]>>(L, 3)&& translator.Assignable<System.Func<string, XFX.Asset.IRole[]>>(L, 4)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    XFX.Asset.IRoot _root = (XFX.Asset.IRoot)translator.GetObject(L, 2, typeof(XFX.Asset.IRoot));
                    System.Func<string, XFX.Asset.IRole[]> _onFire = translator.GetDelegate<System.Func<string, XFX.Asset.IRole[]>>(L, 3);
                    System.Func<string, XFX.Asset.IRole[]> _onHit = translator.GetDelegate<System.Func<string, XFX.Asset.IRole[]>>(L, 4);
                    
                        XFX.Asset.IAction gen_ret = XFX.Game.LuaApi.Action.Play( _name, _root, _onFire, _onHit );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to XFX.Game.LuaApi.Action.Play!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Stop_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    XFX.Asset.IAction _action = (XFX.Asset.IAction)translator.GetObject(L, 1, typeof(XFX.Asset.IAction));
                    
                    XFX.Game.LuaApi.Action.Stop( _action );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowArea_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    XFX.Core.Render.IRenderObject _renderObject = (XFX.Core.Render.IRenderObject)translator.GetObject(L, 1, typeof(XFX.Core.Render.IRenderObject));
                    int _sn = LuaAPI.xlua_tointeger(L, 2);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    float _angle = (float)LuaAPI.lua_tonumber(L, 4);
                    
                    XFX.Game.LuaApi.Action.ShowArea( _renderObject, _sn, _distance, _angle );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HideArea_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sn = LuaAPI.xlua_tointeger(L, 1);
                    
                    XFX.Game.LuaApi.Action.HideArea( _sn );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveArea_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sn = LuaAPI.xlua_tointeger(L, 1);
                    
                    XFX.Game.LuaApi.Action.RemoveArea( _sn );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
