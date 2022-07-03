--[[
  Id: Registry.lua
  Desc: 游戏内所有服务类型定义
--]]
local Registry = {}

-- 游戏状态
Registry.states = {
    Login = require("game.state.LoginState"),
    Game = require("game.state.GameState"),
    Update = require("game.state.UpdateState"),
    Loading = require("game.state.LoadingState")
}

-- 更新阶段启动的服务
Registry.preloadServices = {
    Sdks = require("game.svc.SdksSvc"),
    States = require("game.svc.StatesSvc"),
    Views = require("game.svc.ViewsSvc"),
    Ctrls = require("game.svc.CtrlsSvc"),
    Audios = require("game.svc.AudiosSvc"),
}


Registry.preloadCtrls = {
    LoadingCtrl = require("game.ctrl.LoadingCtrl")
    -- UpdateCtrl = require("game.ctrl.UpdateCtrl"),
    -- MsgBoxCtrl = require("game.ctrl.MsgBoxCtrl"),
}

-- 视图控制器
Registry.ctrls = {
    LoginCtrl = require("game.ctrl.LoginCtrl")
}

-- 视图
Registry.views = {
    LoginView = require("game.view.LoginView")
}

return Registry
