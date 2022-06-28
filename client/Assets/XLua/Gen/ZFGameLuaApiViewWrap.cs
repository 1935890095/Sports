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
    public class ZFGameLuaApiViewWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZF.Game.LuaApi.View);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 39, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Open", _m_Open_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsOpen", _m_IsOpen_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Close", _m_Close_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Dump", _m_Dump_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Lookup", _m_Lookup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Find", _m_Find_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GameObject", _m_GameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Transform", _m_Transform_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetEvent", _m_SetEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearEvents", _m_ClearEvents_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RectTransform", _m_RectTransform_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "VerticalLayoutGroup", _m_VerticalLayoutGroup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Button", _m_Button_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DropDown", _m_DropDown_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Toggle", _m_Toggle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToggleGroup", _m_ToggleGroup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Graphic", _m_Graphic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Text", _m_Text_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Outline", _m_Outline_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Shadow", _m_Shadow_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "InputField", _m_InputField_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Slider", _m_Slider_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ScrollRect", _m_ScrollRect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Scrollbar", _m_Scrollbar_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Tween", _m_Tween_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Image", _m_Image_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RawImage", _m_RawImage_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Canvas", _m_Canvas_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CanvasGroup", _m_CanvasGroup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RaycastUI", _m_RaycastUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RaycastUIGetFrist", _m_RaycastUIGetFrist_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateEffect", _m_CreateEffect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateFaceEffect", _m_CreateFaceEffect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateCanvas", _m_CreateCanvas_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateRole", _m_CreateRole_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateReminder", _m_CreateReminder_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateAudio", _m_CreateAudio_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateVideoPlayer", _m_CreateVideoPlayer_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ZF.Game.LuaApi.View does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Open_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                    ZF.Game.LuaApi.View.Open( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsOpen_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                        bool gen_ret = ZF.Game.LuaApi.View.IsOpen( _name );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Close_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    bool _destroy = LuaAPI.lua_toboolean(L, 2);
                    
                    ZF.Game.LuaApi.View.Close( _name, _destroy );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Dump_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = ZF.Game.LuaApi.View.Dump(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Lookup_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _parentid = LuaAPI.xlua_tointeger(L, 2);
                    string _path = LuaAPI.lua_tostring(L, 3);
                    string _type = LuaAPI.lua_tostring(L, 4);
                    
                        int gen_ret = ZF.Game.LuaApi.View.Lookup( _name, _parentid, _path, _type );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Find_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    string _path = LuaAPI.lua_tostring(L, 2);
                    
                        int gen_ret = ZF.Game.LuaApi.View.Find( _name, _path );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.GameObject( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Transform_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Transform( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    int _type = LuaAPI.xlua_tointeger(L, 3);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 4);
                    
                    ZF.Game.LuaApi.View.SetEvent( _name, _id, _type, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearEvents_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    
                    ZF.Game.LuaApi.View.ClearEvents( _name, _id );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RectTransform_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object[] _args = translator.GetParams<object>(L, 4);
                    
                        object gen_ret = ZF.Game.LuaApi.View.RectTransform( _name, _id, _func, _args );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_VerticalLayoutGroup_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.VerticalLayoutGroup( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Button_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Button( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DropDown_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object[] _args = translator.GetParams<object>(L, 4);
                    
                        object gen_ret = ZF.Game.LuaApi.View.DropDown( _name, _id, _func, _args );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Toggle_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Toggle( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToggleGroup_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.ToggleGroup( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Graphic_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Graphic( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Text_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Text( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Outline_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Outline( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Shadow_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Shadow( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InputField_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.InputField( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Slider_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Slider( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ScrollRect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.ScrollRect( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Scrollbar_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Scrollbar( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Tween_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object[] _args = translator.GetParams<object>(L, 4);
                    
                        object gen_ret = ZF.Game.LuaApi.View.Tween( _name, _id, _func, _args );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Image_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object[] _vals = translator.GetParams<object>(L, 4);
                    
                        object gen_ret = ZF.Game.LuaApi.View.Image( _name, _id, _func, _vals );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RawImage_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.RawImage( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Canvas_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.Canvas( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CanvasGroup_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _func = LuaAPI.lua_tostring(L, 3);
                    object _value = translator.GetObject(L, 4, typeof(object));
                    
                        object gen_ret = ZF.Game.LuaApi.View.CanvasGroup( _name, _id, _func, _value );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RaycastUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 1, out _pos);
                    
                        bool gen_ret = ZF.Game.LuaApi.View.RaycastUI( _pos );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RaycastUIGetFrist_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _pos;translator.Get(L, 1, out _pos);
                    
                        string gen_ret = ZF.Game.LuaApi.View.RaycastUIGetFrist( _pos );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
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
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _filename = LuaAPI.lua_tostring(L, 3);
                    
                        ZF.Asset.IEffect gen_ret = ZF.Game.LuaApi.View.CreateEffect( _name, _id, _filename );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateFaceEffect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _filename = LuaAPI.lua_tostring(L, 3);
                    
                        ZF.Asset.IFaceEffect gen_ret = ZF.Game.LuaApi.View.CreateFaceEffect( _name, _id, _filename );
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
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    
                        ZF.Asset.ICanvas gen_ret = ZF.Game.LuaApi.View.CreateCanvas( _name, _id );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateRole_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _filename = LuaAPI.lua_tostring(L, 3);
                    
                        ZF.Asset.IRole gen_ret = ZF.Game.LuaApi.View.CreateRole( _name, _id, _filename );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateReminder_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _filename = LuaAPI.lua_tostring(L, 3);
                    
                        ZF.Asset.IReminder gen_ret = ZF.Game.LuaApi.View.CreateReminder( _name, _id, _filename );
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
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    string _filename = LuaAPI.lua_tostring(L, 3);
                    
                        ZF.Asset.IAudio gen_ret = ZF.Game.LuaApi.View.CreateAudio( _name, _id, _filename );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateVideoPlayer_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    
                        ZF.Asset.IVideoPlayer gen_ret = ZF.Game.LuaApi.View.CreateVideoPlayer( _name, _id );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
