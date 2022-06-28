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
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi), ZFGameLuaApiWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Core.Render.IRenderObject), ZFCoreRenderIRenderObjectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Core.Render.IRenderComponent), ZFCoreRenderIRenderComponentWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IAudio), ZFAssetIAudioWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IVideoClip), ZFAssetIVideoClipWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IVideoPlayer), ZFAssetIVideoPlayerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IEffect), ZFAssetIEffectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.ITexture), ZFAssetITextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IRenderTexture), ZFAssetIRenderTextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IRole), ZFAssetIRoleWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IPrimitive3D), ZFAssetIPrimitive3DWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.ICamera), ZFAssetICameraWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.ILight), ZFAssetILightWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IRoot), ZFAssetIRootWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IReminder), ZFAssetIReminderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IFaceEffect), ZFAssetIFaceEffectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.ICanvas), ZFAssetICanvasWrap.__Register);
        
        
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
        
        
            translator.DelayWrapLoader(typeof(ZF.Asset.IAction), ZFAssetIActionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Action), ZFGameLuaApiActionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Asset), ZFGameLuaApiAssetWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Coloring), ZFGameLuaApiColoringWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Http), ZFGameLuaApiHttpWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AI.NavMeshAgent), UnityEngineAINavMeshAgentWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AI.NavMeshObstacle), UnityEngineAINavMeshObstacleWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.NavMesh), ZFGameLuaApiNavMeshWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Net), ZFGameLuaApiNetWrap.__Register);
        
        }
        
        static void wrapInit1(LuaEnv luaenv, ObjectTranslator translator)
        {
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Scene), ZFGameLuaApiSceneWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Sdk), ZFGameLuaApiSdkWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Shaking), ZFGameLuaApiShakingWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Tweening), ZFGameLuaApiTweeningWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Update), ZFGameLuaApiUpdateWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.View), ZFGameLuaApiViewWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.PathExt), ZFGameLuaApiPathExtWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Cache), ZFGameLuaApiCacheWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Log), ZFGameLuaApiLogWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Debug), ZFGameLuaApiDebugWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Screen), ZFGameLuaApiScreenWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(ZF.Game.LuaApi.Util), ZFGameLuaApiUtilWrap.__Register);
        
        
        
        }
        
        static void Init(LuaEnv luaenv, ObjectTranslator translator)
        {
            
            wrapInit0(luaenv, translator);
            
            wrapInit1(luaenv, translator);
            
            
            translator.AddInterfaceBridgeCreator(typeof(ZF.Game.LuaContext), ZFGameLuaContextBridge.__Create);
            
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
