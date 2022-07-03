#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;
using System.Collections.Generic;
using System.Reflection;


namespace XLua.CSObjectWrap
{
    public class XLua_Gen_Initer_Register__
	{
        
        
        static void wrapInit0(LuaEnv luaenv, ObjectTranslator translator)
        {
        
            translator.DelayWrapLoader(typeof(LuaPerfect.ObjectRef), LuaPerfectObjectRefWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(LuaPerfect.ObjectItem), LuaPerfectObjectItemWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(LuaPerfect.ObjectFormater), LuaPerfectObjectFormaterWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi), XFXGameLuaApiWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Core.Render.IRenderObject), XFXCoreRenderIRenderObjectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Core.Render.IRenderComponent), XFXCoreRenderIRenderComponentWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IAudio), XFXAssetIAudioWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IVideoClip), XFXAssetIVideoClipWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IVideoPlayer), XFXAssetIVideoPlayerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IEffect), XFXAssetIEffectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.ITexture), XFXAssetITextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IRenderTexture), XFXAssetIRenderTextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IRole), XFXAssetIRoleWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IPrimitive3D), XFXAssetIPrimitive3DWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.ICamera), XFXAssetICameraWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.ILight), XFXAssetILightWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IRoot), XFXAssetIRootWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IReminder), XFXAssetIReminderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IFaceEffect), XFXAssetIFaceEffectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.ICanvas), XFXAssetICanvasWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector2), UnityEngineVector2Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector3), UnityEngineVector3Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector4), UnityEngineVector4Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Color), UnityEngineColorWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Quaternion), UnityEngineQuaternionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Ray), UnityEngineRayWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Bounds), UnityEngineBoundsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Ray2D), UnityEngineRay2DWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Color32), UnityEngineColor32Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Rect), UnityEngineRectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RectTransform), UnityEngineRectTransformWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Resources), UnityEngineResourcesWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.TextAsset), UnityEngineTextAssetWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Application), UnityEngineApplicationWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Screen), UnityEngineScreenWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.PlayerPrefs), UnityEnginePlayerPrefsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Time), UnityEngineTimeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Shader), UnityEngineShaderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.KeyCode), UnityEngineKeyCodeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.TouchPhase), UnityEngineTouchPhaseWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SystemLanguage), UnityEngineSystemLanguageWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AudioListener), UnityEngineAudioListenerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SortingLayer), UnityEngineSortingLayerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.MeshCollider), UnityEngineMeshColliderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.NetworkReachability), UnityEngineNetworkReachabilityWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Asset.IAction), XFXAssetIActionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Action), XFXGameLuaApiActionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Asset), XFXGameLuaApiAssetWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Coloring), XFXGameLuaApiColoringWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Http), XFXGameLuaApiHttpWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AI.NavMeshAgent), UnityEngineAINavMeshAgentWrap.__Register);
        
        }
        
        static void wrapInit1(LuaEnv luaenv, ObjectTranslator translator)
        {
        
            translator.DelayWrapLoader(typeof(UnityEngine.AI.NavMeshObstacle), UnityEngineAINavMeshObstacleWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.NavMesh), XFXGameLuaApiNavMeshWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Net), XFXGameLuaApiNetWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Scene), XFXGameLuaApiSceneWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Sdk), XFXGameLuaApiSdkWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Shaking), XFXGameLuaApiShakingWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Tweening), XFXGameLuaApiTweeningWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Update), XFXGameLuaApiUpdateWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.View), XFXGameLuaApiViewWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.PathExt), XFXGameLuaApiPathExtWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Cache), XFXGameLuaApiCacheWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Log), XFXGameLuaApiLogWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Debug), XFXGameLuaApiDebugWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Screen), XFXGameLuaApiScreenWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(XFX.Game.LuaApi.Util), XFXGameLuaApiUtilWrap.__Register);
        
        
        
        }
        
        static void Init(LuaEnv luaenv, ObjectTranslator translator)
        {
            
            wrapInit0(luaenv, translator);
            
            wrapInit1(luaenv, translator);
            
            
            translator.AddInterfaceBridgeCreator(typeof(XFX.Game.LuaContext), XFXGameLuaContextBridge.__Create);
            
        }
        
	    static XLua_Gen_Initer_Register__()
        {
		    XLua.LuaEnv.AddIniter(Init);
		}
		
		
	}
	
}
namespace XLua
{
	public partial class ObjectTranslator
	{
		static XLua.CSObjectWrap.XLua_Gen_Initer_Register__ s_gen_reg_dumb_obj = new XLua.CSObjectWrap.XLua_Gen_Initer_Register__();
		static XLua.CSObjectWrap.XLua_Gen_Initer_Register__ gen_reg_dumb_obj {get{return s_gen_reg_dumb_obj;}}
	}
	
	internal partial class InternalGlobals
    {
	    
	    static InternalGlobals()
		{
		    extensionMethodMap = new Dictionary<Type, IEnumerable<MethodInfo>>()
			{
			    
			};
			
			genTryArrayGetPtr = StaticLuaCallbacks.__tryArrayGet;
            genTryArraySetPtr = StaticLuaCallbacks.__tryArraySet;
		}
	}
}
