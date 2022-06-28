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
    public class ZFAssetICameraWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Asset.ICamera);
			Utils.BeginObjectRegister(type, L, translator, 0, 10, 11, 11);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "WorldToViewportPoint", _m_WorldToViewportPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "WorldToScreenPoint", _m_WorldToScreenPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ViewportToWorldPoint", _m_ViewportToWorldPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ScreenToWorldPoint", _m_ScreenToWorldPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ViewportToScreenPoint", _m_ViewportToScreenPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ScreenToViewportPoint", _m_ScreenToViewportPoint);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Raycast", _m_Raycast);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FadeOut", _m_FadeOut);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FadeIn", _m_FadeIn);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Shake", _m_Shake);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "enabled", _g_get_enabled);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "backgroundColor", _g_get_backgroundColor);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "clearFlags", _g_get_clearFlags);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "cullingMask", _g_get_cullingMask);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "orthographic", _g_get_orthographic);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "orthographicSize", _g_get_orthographicSize);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "nearClipPlane", _g_get_nearClipPlane);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "farClipPlane", _g_get_farClipPlane);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "fieldOfView", _g_get_fieldOfView);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "depth", _g_get_depth);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "rect", _g_get_rect);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "enabled", _s_set_enabled);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "backgroundColor", _s_set_backgroundColor);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "clearFlags", _s_set_clearFlags);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "cullingMask", _s_set_cullingMask);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "orthographic", _s_set_orthographic);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "orthographicSize", _s_set_orthographicSize);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "nearClipPlane", _s_set_nearClipPlane);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "farClipPlane", _s_set_farClipPlane);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "fieldOfView", _s_set_fieldOfView);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "depth", _s_set_depth);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "rect", _s_set_rect);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Asset.ICamera does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WorldToViewportPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.WorldToViewportPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WorldToScreenPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.WorldToScreenPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ViewportToWorldPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.ViewportToWorldPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ScreenToWorldPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.ScreenToWorldPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ViewportToScreenPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.ViewportToScreenPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ScreenToViewportPoint(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                        UnityEngine.Vector3 gen_ret = gen_to_be_invoked.ScreenToViewportPoint( _pos );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    string _tag = LuaAPI.lua_tostring(L, 3);
                    int _up = LuaAPI.xlua_tointeger(L, 4);
                    string _resultType = LuaAPI.lua_tostring(L, 5);
                    int _layerMask = LuaAPI.xlua_tointeger(L, 6);
                    
                        object gen_ret = gen_to_be_invoked.Raycast( _pos, _tag, _up, _resultType, _layerMask );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FadeOut(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.FadeOut( _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FadeIn(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.FadeIn( _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Shake(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    float _amount = (float)LuaAPI.lua_tonumber(L, 2);
                    float _duration = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.Shake( _amount, _duration );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_enabled(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.enabled);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_backgroundColor(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                translator.PushUnityEngineColor(L, gen_to_be_invoked.backgroundColor);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_clearFlags(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.clearFlags);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_cullingMask(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.cullingMask);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_orthographic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.orthographic);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_orthographicSize(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.orthographicSize);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_nearClipPlane(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.nearClipPlane);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_farClipPlane(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.farClipPlane);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_fieldOfView(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.fieldOfView);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_depth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.depth);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_rect(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                translator.PushUnityEngineRect(L, gen_to_be_invoked.rect);
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
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.enabled = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_backgroundColor(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                UnityEngine.Color gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.backgroundColor = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_clearFlags(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.clearFlags = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_cullingMask(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.cullingMask = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_orthographic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.orthographic = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_orthographicSize(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.orthographicSize = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_nearClipPlane(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.nearClipPlane = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_farClipPlane(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.farClipPlane = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_fieldOfView(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.fieldOfView = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_depth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.depth = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_rect(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZF.Asset.ICamera gen_to_be_invoked = (ZF.Asset.ICamera)translator.FastGetCSObj(L, 1);
                UnityEngine.Rect gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.rect = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
