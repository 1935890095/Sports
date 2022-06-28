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
    public class ZFAssetIRoleWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Asset.IRole);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 1, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Play", _m_Play);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsPlaying", _m_IsPlaying);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Attach", _m_Attach);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Detach", _m_Detach);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "bounds", _g_get_bounds);
            
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Asset.IRole does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Play(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IRole gen_to_be_invoked = (ZF.Asset.IRole)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    float _speed = (float)LuaAPI.lua_tonumber(L, 3);
                    string _parameter = LuaAPI.lua_tostring(L, 4);
                    
                    gen_to_be_invoked.Play( _name, _speed, _parameter );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    float _speed = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.Play( _name, _speed );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.Play( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Asset.IRole.Play!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsPlaying(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IRole gen_to_be_invoked = (ZF.Asset.IRole)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.IsPlaying( _name );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Attach(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IRole gen_to_be_invoked = (ZF.Asset.IRole)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<ZF.Core.Render.IRenderObject>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    ZF.Core.Render.IRenderObject _robj = (ZF.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(ZF.Core.Render.IRenderObject));
                    string _point = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.Attach( _robj, _point );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<ZF.Core.Render.IRenderObject>(L, 2)) 
                {
                    ZF.Core.Render.IRenderObject _robj = (ZF.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(ZF.Core.Render.IRenderObject));
                    
                    gen_to_be_invoked.Attach( _robj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Asset.IRole.Attach!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Detach(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.IRole gen_to_be_invoked = (ZF.Asset.IRole)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<ZF.Core.Render.IRenderObject>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    ZF.Core.Render.IRenderObject _robj = (ZF.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(ZF.Core.Render.IRenderObject));
                    string _point = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.Detach( _robj, _point );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<ZF.Core.Render.IRenderObject>(L, 2)) 
                {
                    ZF.Core.Render.IRenderObject _robj = (ZF.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(ZF.Core.Render.IRenderObject));
                    
                    gen_to_be_invoked.Detach( _robj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Asset.IRole.Detach!");
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_bounds(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.IRole gen_to_be_invoked = (ZF.Asset.IRole)translator.FastGetCSObj(L, 1);
                translator.PushUnityEngineBounds(L, gen_to_be_invoked.bounds);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
