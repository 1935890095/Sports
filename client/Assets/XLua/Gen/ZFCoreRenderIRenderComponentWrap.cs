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
    public class ZFCoreRenderIRenderComponentWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Core.Render.IRenderComponent);
			Utils.BeginObjectRegister(type, L, translator, 0, 5, 2, 1);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Create", _m_Create);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Destroy", _m_Destroy);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Update", _m_Update);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LateUpdate", _m_LateUpdate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Command", _m_Command);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "renderObject", _g_get_renderObject);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "enabled", _g_get_enabled);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "enabled", _s_set_enabled);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Core.Render.IRenderComponent does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Create(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    ZF.Core.Render.IRenderObject _renderObject = (ZF.Core.Render.IRenderObject)translator.GetObject(L, 2, typeof(ZF.Core.Render.IRenderObject));
                    
                    gen_to_be_invoked.Create( _renderObject );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Destroy(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Destroy(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Update(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Update(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LateUpdate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LateUpdate(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Command(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _cmd = LuaAPI.lua_tostring(L, 2);
                    object[] _args = translator.GetParams<object>(L, 3);
                    
                    gen_to_be_invoked.Command( _cmd, _args );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_renderObject(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
                translator.PushAny(L, gen_to_be_invoked.renderObject);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_enabled(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.enabled);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_enabled(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Core.Render.IRenderComponent gen_to_be_invoked = (ZF.Core.Render.IRenderComponent)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.enabled = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
