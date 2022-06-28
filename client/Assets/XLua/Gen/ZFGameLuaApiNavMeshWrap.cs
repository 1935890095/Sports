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
    public class ZFGameLuaApiNavMeshWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Game.LuaApi.NavMesh);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 6, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "AddNavMeshAgent", _m_AddNavMeshAgent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddNavMeshObstacle", _m_AddNavMeshObstacle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsInNavMesh", _m_IsInNavMesh_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AdjustToMesh", _m_AdjustToMesh_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CalculatePath", _m_CalculatePath_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Game.LuaApi.NavMesh does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddNavMeshAgent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    ZF.Core.Render.RenderObject _renderObject = (ZF.Core.Render.RenderObject)translator.GetObject(L, 1, typeof(ZF.Core.Render.RenderObject));
                    
                        UnityEngine.AI.NavMeshAgent gen_ret = ZF.Game.LuaApi.NavMesh.AddNavMeshAgent( _renderObject );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddNavMeshObstacle_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<ZF.Core.Render.RenderObject>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    ZF.Core.Render.RenderObject _renderObject = (ZF.Core.Render.RenderObject)translator.GetObject(L, 1, typeof(ZF.Core.Render.RenderObject));
                    int _shape = LuaAPI.xlua_tointeger(L, 2);
                    
                        UnityEngine.AI.NavMeshObstacle gen_ret = ZF.Game.LuaApi.NavMesh.AddNavMeshObstacle( _renderObject, _shape );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& translator.Assignable<ZF.Core.Render.RenderObject>(L, 1)) 
                {
                    ZF.Core.Render.RenderObject _renderObject = (ZF.Core.Render.RenderObject)translator.GetObject(L, 1, typeof(ZF.Core.Render.RenderObject));
                    
                        UnityEngine.AI.NavMeshObstacle gen_ret = ZF.Game.LuaApi.NavMesh.AddNavMeshObstacle( _renderObject );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Game.LuaApi.NavMesh.AddNavMeshObstacle!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsInNavMesh_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 1, out _pos);
                    int _areaMask = LuaAPI.xlua_tointeger(L, 2);
                    
                        bool gen_ret = ZF.Game.LuaApi.NavMesh.IsInNavMesh( _pos, _areaMask );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AdjustToMesh_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector3>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 1, out _pos);
                    int _navmask = LuaAPI.xlua_tointeger(L, 2);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        UnityEngine.Vector3 gen_ret = ZF.Game.LuaApi.NavMesh.AdjustToMesh( _pos, _navmask, _radius );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.Vector3>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 1, out _pos);
                    int _navmask = LuaAPI.xlua_tointeger(L, 2);
                    
                        UnityEngine.Vector3 gen_ret = ZF.Game.LuaApi.NavMesh.AdjustToMesh( _pos, _navmask );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZF.Game.LuaApi.NavMesh.AdjustToMesh!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CalculatePath_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _source;translator.Get(L, 1, out _source);
                    UnityEngine.Vector3 _target;translator.Get(L, 2, out _target);
                    int _areaMask = LuaAPI.xlua_tointeger(L, 3);
                    
                        bool gen_ret = ZF.Game.LuaApi.NavMesh.CalculatePath( _source, _target, _areaMask );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
