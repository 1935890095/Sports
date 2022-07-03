--[[
  Id: Unity.lua
  Desc: 所有Unity相关的定义（都在这个模块中)
  Author: figo
  Date: 2020-04-21 14:10:56
--]]
local Unity = {}

-- unity需要导出的组件
Unity.Vector2 = CS.UnityEngine.Vector2
Unity.Vector3 = CS.UnityEngine.Vector3
Unity.RectTransform = CS.UnityEngine.RectTransform
Unity.Color = CS.UnityEngine.Color
Unity.Color32 = CS.UnityEngine.Color32
Unity.Time = CS.UnityEngine.Time

Unity.Resources = CS.UnityEngine.Resources
Unity.TextAsset = CS.UnityEngine.TextAsset
Unity.QualitySettings = CS.UnityEngine.QualitySettings
Unity.ShadowQuality = CS.UnityEngine.ShadowQuality
Unity.AnisotropicFiltering = CS.UnityEngine.AnisotropicFiltering
Unity.ShadowProjection = CS.UnityEngine.ShadowProjection
Unity.BlendWeights = CS.UnityEngine.BlendWeights
Unity.Shader = CS.UnityEngine.Shader
Unity.PlayerPrefs = CS.UnityEngine.PlayerPrefs
Unity.Input = CS.UnityEngine.Input
Unity.Debug = CS.UnityEngine.Debug
Unity.KeyCode = CS.UnityEngine.KeyCode
Unity.TouchPhase = CS.UnityEngine.TouchPhase
Unity.SystemLanguage = CS.UnityEngine.SystemLanguage
Unity.Screen = CS.UnityEngine.Screen
Unity.SystemInfo = CS.UnityEngine.SystemInfo
Unity.SortingLayer = CS.UnityEngine.SortingLayer
Unity.NetworkReachability = CS.UnityEngine.NetworkReachability

-- platform
Unity.RuntimePlatform = {
    OSXEditor = CS.UnityEngine.RuntimePlatform.OSXEditor,
    OSXPlayer = CS.UnityEngine.RuntimePlatform.OSXPlayer,
    WindowsPlayer = CS.UnityEngine.RuntimePlatform.WindowsPlayer,
    WindowsEditor = CS.UnityEngine.RuntimePlatform.WindowsEditor,
    IPhonePlayer = CS.UnityEngine.RuntimePlatform.IPhonePlayer,
    Android = CS.UnityEngine.RuntimePlatform.Android
}

-- application
Unity.Application = {
    persistentDataPath = CS.UnityEngine.Application.persistentDataPath,
    streamingAssetsPath = CS.UnityEngine.Application.streamingAssetsPath,
    dataPath = CS.UnityEngine.Application.dataPath,
    platform = CS.UnityEngine.Application.platform,
    isEditor = CS.UnityEngine.Application.isEditor,
    isMobilePlatform = CS.UnityEngine.Application.isMobilePlatform,
    systemLanguage = CS.UnityEngine.Application.systemLanguage,
    openURL = CS.UnityEngine.Application.OpenURL,
    version = CS.UnityEngine.Application.version,
    internetReachability = CS.UnityEngine.Application.internetReachability
}

if Unity.Application.isMobilePlatform then
    Unity.DeviceUniqueIdentifier = ""
else
    Unity.DeviceUniqueIdentifier = CS.UnityEngine.SystemInfo.deviceUniqueIdentifier
end

-- 事件触发类型(see: UnityEngine.EventSystems.EventTriggerType)
Unity.EventTriggerType = {
    -- 摘要: Intercepts a IPointerEnterHandler.OnPointerEnter.
    PointerEnter = 0,
    -- 摘要: Intercepts a IPointerExitHandler.OnPointerExit.
    PointerExit = 1,
    -- 摘要: Intercepts a IPointerDownHandler.OnPointerDown.
    PointerDown = 2,
    -- 摘要: Intercepts a IPointerUpHandler.OnPointerUp.
    PointerUp = 3,
    -- 摘要: Intercepts a IPointerClickHandler.OnPointerClick.
    PointerClick = 4,
    -- 摘要: Intercepts a IDragHandler.OnDrag.
    Drag = 5,
    -- 摘要: Intercepts a IDropHandler.OnDrop.
    Drop = 6,
    -- 摘要: Intercepts a IScrollHandler.OnScroll.
    Scroll = 7,
    -- 摘要: Intercepts a IUpdateSelectedHandler.OnUpdateSelected.
    UpdateSelected = 8,
    -- 摘要: Intercepts a ISelectHandler.OnSelect.
    Select = 9,
    -- 摘要: Intercepts a IDeselectHandler.OnDeselect.
    Deselect = 10,
    -- 摘要: Intercepts a IMoveHandler.OnMove.
    Move = 11,
    -- 摘要: Intercepts IInitializePotentialDrag.InitializePotentialDrag.
    InitializePotentialDrag = 12,
    -- 摘要: Intercepts IBeginDragHandler.OnBeginDrag.
    BeginDrag = 13,
    -- 摘要: Intercepts IEndDragHandler.OnEndDrag.
    EndDrag = 14,
    -- 摘要: Intercepts ISubmitHandler.Submit.
    Submit = 15,
    -- 摘要: Intercepts ICancelHandler.OnCancel.
    Cancel = 16
}

-- 游戏定义的layer
Unity.Layer = {
    default = 0,
    ui = 5,
    map = 8, -- 地图
    role = 9, -- 地图单位
    effect = 10, -- 特效
    mapstatic = 11, -- 可批处理的静态地图层
    Mask = {
        ui = 1 << 5,
        map = 1 << 8,
        role = 1 << 9,
        effect = 1 << 10,
        mapstatic = 1 << 11
    }
}

Unity.SortingLayerNames = {
    [0] = "Default",
    [1] = "Hud",
    [2] = "Window",
    [3] = "Popup",
    [4] = "MsgBox",
    [5] = "TopMost"
}

Unity.AligmentType = {
    UpperLeft = 0,
    UpperCenter = 1,
    UpperRight = 2,
    MiddleLeft = 3,
    MiddleCenter = 4,
    MiddleRight = 5,
    LowerLeft = 6,
    LowerCenter = 7,
    LowerRight = 8
}

-- 游戏定义的tag
Unity.Tag = {
    uiroot = "uiroot",
    maproot = "maproot", --
    audio = "audio", -- 音效
    mapgrid = "mapgrid" -- 地图格子
}

-- 地图可行走(导航)层
Unity.NavLayer = {
    invalid = -1,
    walkable = 0, -- built-in default
    notwalkable = 1, -- built-in
    jump = 2, -- built-in
    layer0 = 0, -- 与walkable同
    layer1 = 3, -- 自定义地面层1
    layer2 = 4, -- 自定义地面层2
    groundTrap = 5, -- 地面障碍物层
    air0 = 16, -- 自定义空中层1
    air1 = 17, -- 自定义空中层2
    air2 = 18, -- 自定义空中层3
    airTrap = 19, -- 空中障碍物层
    -- 掩码
    Mask = {
        walkable = 1,
        notwalkable = 1 << 1,
        jump = 1 << 2,
        layer0 = 1,
        layer1 = 1 << 3,
        layer2 = 1 << 4,
        groupTrap = 1 << 5,
        air0 = 1 << 16,
        air1 = 1 << 17,
        air2 = 1 << 18,
        airTrap = 1 << 19
    }
}

-- 忽略其他系统
if Unity.Application.platform == Unity.RuntimePlatform.Android then
    Unity.Os = "android"
elseif Unity.Application.platform == Unity.RuntimePlatform.IPhonePlayer then
    Unity.Os = "iOS"
elseif Unity.Application.platform == Unity.RuntimePlatform.OSXEditor or Unity.Application.platform == Unity.RuntimePlatform.OSXPlayer then
    Unity.Os = "macOS"
elseif Unity.Application.platform == Unity.RuntimePlatform.WindowsEditor or Unity.Application.platform == Unity.RuntimePlatform.WindowsPlayer then
    Unity.Os = "windows"
else
    Unity.Os = "other"
end
-- 当前版本附带操作系统
Unity.OsVersion = Unity.SystemInfo.operatingSystem

return Unity
