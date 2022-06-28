--[[
  Id: View.lua
  Desc: View基类
  Author: figo
  Date: 2020-04-17 11:27:32
--]]
local string_lower = string.lower
local class = class or require("core.Class").class
local const = const or require("core.Class").const
local API = require("game.view.ViewApi")

local View = class()

-- 视图分组,分组越大显示越靠前
View.ViewGroup =
    const(
    {
        default = 0,
        hud = 1, -- 抬头显示
        window = 2, -- 窗体
        popup = 3, -- 弹出框(子窗体)
        msgbox = 4, -- 模态消息框
        topmost = 5 --
    }
)

-- 视图销毁模式
View.DestroyMode =
    const(
    {
        auto = 0, -- 管理器决定
        hide = 1, -- 关闭时销毁
        custom = 2 -- 用户决定
    }
)

-- helper function to define subclass
-- @res  the view res code
-- @ctrl the control of view
function View.class(res, ctrl)
    local cls = class(View)
    cls.__res = res
    cls.__ctrl = ctrl
    cls.name = res.__name
    return cls
end

function View:ctor(ctrl)
    self.group = View.ViewGroup.window
    self.mode = View.DestroyMode.auto
    self.planeDistance = 1
    self.loaded = false
    self.ctrl = ctrl
    self.ctx = nil -- 允许在视图显示透传数据
    self.setCamera = true
end

-- load view res, called as soon as prefab loaded.
function View:load()
    local res = self.__res
    self.view = res.load(self.name)
    self.loaded = true

    -- upvalue
    local ctrl = self.ctrl
    local view = self

    local onCreate = self.onCreate
    self.onCreate = function()
        if self.setCamera then
            self:Canvas(view.view.self, "init", self.planeDistance)
        end
        ctrl:onCreate(view)
        onCreate(view)
    end

    local onOpen = self.onEnabled
    self.onEnabled = function()
        self:Canvas(view.view.self, API.Canvas.sortingLayerName, G.unity.SortingLayerNames[view.group])
        ctrl:onOpen(view)
        onOpen(view)
    end

    local onClose = self.onDisable
    self.onDisable = function()
        ctrl:onClose(view)
        onClose(view)
    end

    local onDestroy = self.onDestroy
    self.onDestroy = function()
        ctrl:onDestroy(view)
        onDestroy(view)
    end
end

function View:show(ctx)
    self.ctx = ctx
    G.api.View.Open(self.name)
end

function View:isShow()
    return G.api.View.IsOpen(self.name)
end

function View:hide()
    self.ctx = nil
    self.__pendings = nil
    G.api.View.Close(self.name, false)
    self.ctrl.router:route(G.game.event.View.CLOSEOVER, self.name)
end

function View:destroy()
    G.api.View.Close(self.name, true)
end

--[virtual]
function View:onCreate()
end
function View:onStart()
end
function View:onEnabled()
end
function View:onDisable()
end
function View:onUpdate()
end
function View:onDestroy()
end

-- help function for recv notify from ctrl
function View:on(what, handle)
    self.ctrl:onNotify(self, what, handle)
end
function View:off(what, handle)
    self.ctrl:offNotify(self, what, handle)
end

-------------------------------------------------
-- 以下接入是对G.api.View接口的封装
-- (接口名与G.api.View同，且能更好的与本身接口相区分)
function View:GameObject(id, func, ...)
    return G.api.View.GameObject(self.name, id, func, ...)
end

function View:GameObjectOther(name, id, func, ...)
    return G.api.View.GameObject(name, id, func, ...)
end

function View:Transform(id, func, value)
    return G.api.View.Transform(self.name, id, func, value)
end

function View:SetEvent(id, type, action)
    G.api.View.SetEvent(self.name, id, type, action)
end

function View:ClearEvents(id)
    G.api.View.ClearEvents(self.name, id)
end

function View:RectTransform(id, func, ...)
    return G.api.View.RectTransform(self.name, id, func, ...)
end

function View:VerticalLayoutGroup(id, func, ...)
    return G.api.View.VerticalLayoutGroup(self.name, id, func, ...)
end

function View:RectTransformPos(name, id, func, ...)
    return G.api.View.RectTransform(name, id, func, ...)
end

function View:Button(id, func, value)
    return G.api.View.Button(self.name, id, func, value)
end

function View:ButtonCus(name, id, func, value)
    return G.api.View.Button(name, id, func, value)
end

function View:DropDown(id, func, ...)
    return G.api.View.DropDown(self.name, id, func, ...)
end

function View:Toggle(id, func, value)
    return G.api.View.Toggle(self.name, id, func, value)
end

function View:ToggleGroup(id, func, value)
    return G.api.View.ToggleGroup(self.name, id, func, value)
end

function View:Graphic(id, func, value)
    return G.api.View.Graphic(self.name, id, func, value)
end

function View:Text(id, func, value)
    return G.api.View.Text(self.name, id, func, value)
end

function View:Outline(id, func, value)
    return G.api.View.Outline(self.name, id, func, value)
end

function View:InputField(id, func, value)
    return G.api.View.InputField(self.name, id, func, value)
end

function View:Slider(id, func, value)
    return G.api.View.Slider(self.name, id, func, value)
end

function View:ScrollRect(id, func, value)
    return G.api.View.ScrollRect(self.name, id, func, value)
end

function View:Scrollbar(id, func, value)
    return G.api.View.Scrollbar(self.name, id, func, value)
end

function View:Tweener(id, func, ...)
    return G.api.View.Tween(self.name, id, func, ...)
end

function View:Image(id, func, ...)
    return G.api.View.Image(self.name, id, func, ...)
end

function View:RawImage(id, func, value)
    return G.api.View.RawImage(self.name, id, func, value)
end

function View:Canvas(id, func, value)
    return G.api.View.Canvas(self.name, id, func, value)
end

function View:CanvasGroup(id, func, value)
    return G.api.View.CanvasGroup(self.name, id, func, value)
end

function View:CreateEffect(id, filename)
    return G.api.View.CreateEffect(self.name, id, filename)
end

function View:CreateLive2DEffect(id, filename)
    return G.api.View.CreateLive2DEffect(self.name, id, filename)
end

function View:CreateULiteWebView()
    return G.api.View.CreateULiteWebView()
end

function View:CreateFaceEffect(id, filename)
    return G.api.View.CreateFaceEffect(self.name, id, filename)
end

function View:CreateRole(id, filename)
    return G.api.View.CreateRole(self.name, id, filename)
end

function View:CreateSpineGraphic(id, filename)
    return G.api.View.CreateSpineGraphic(self.name, id, filename)
end

function View:CreateCanvas(name, id, filename)
    if not name then
        name = self.name
    end
    return G.api.View.CreateCanvas(name, id, filename)
end

-- 使用音乐音效服务提供的接口进行播放
-- function View:CreateAudio(id, filename)
--     return G.api.View.CreateAudio(self.name, id, filename)
-- end

-- TODO: to be continue...

return View
