--[[
  Id: InputComponent.lua
  Desc: 输入组件
  Author: figo
  Date: 2020-07-31 18:26:46
--]]
local tb_insert = table.insert
local math_sqrt = math.sqrt

local base = require("core.Component")

local InputComponent = class(base)

local MAX_FINGER = 5
local DOUBLE_DURATION = 0.4
local LONG_TIME = 0.5
local KEYCODE_DURATION = 0.2

local onUpdate
local fire
local init
local reset
local getKeyboard
local down
local drag
local up
local scroll
local clean
local updateZoom
local removeZoom
local distance

local MouseAxis = "Mouse ScrollWheel"

function InputComponent:ctor()
    base.ctor(self)
    init(self)
end

function InputComponent:onCreate()
    if not G.unity.Application.isMobilePlatform then
        -- 所有需要监听的按键
        self.keyCodeListener = {
            G.unity.KeyCode.Alpha0,
            G.unity.KeyCode.Alpha1,
            G.unity.KeyCode.Alpha2,
            G.unity.KeyCode.Escape,
            G.unity.KeyCode.Equals,
            G.unity.KeyCode.Minus
        }
    end

    self.vector1 = G.unity.Vector3(0, 5, 0)
    self.vector2 = G.unity.Vector3(0, -5, 0)
    self.vector3 = G.unity.Vector3(-5, 0, 0)
    self.vector4 = G.unity.Vector3(5, 0, 0)

    self.timer = G.timer.new(G.bind(self, onUpdate), nil, 0, true)
    self.timer:start()
end

function InputComponent:onDestroy()
    self.timer:stop()
    self.timer = nil
end

-- function InputComponent:onUpdate()
onUpdate = function(self)
    if G.unity.Application.isMobilePlatform then
        if G.unity.Input.touchCount > 0 then
            for i = 0, G.unity.Input.touchCount - 1 do
                local touch = G.unity.Input.GetTouch(i) -- G.unity.Input.touches[i]
                local finger = touch.fingerId
                local pos = G.unity.Vector3(touch.position.x, touch.position.y, 0)
                local state = touch.phase

                if state == G.unity.TouchPhase.Began then
                    down(self, finger, pos)
                elseif state == G.unity.TouchPhase.Moved then
                    drag(self, finger, pos)
                elseif state == G.unity.TouchPhase.Ended or state == G.unity.TouchPhase.Canceled then
                    up(self, finger, pos)
                end
            end
        end
    else
        getKeyboard(self)

        local isDown = G.unity.Input:GetMouseButtonDown(0)
        local isDrag = G.unity.Input:GetMouseButton(0)
        local isUp = G.unity.Input:GetMouseButtonUp(0)

        if isDown then
            down(self, 0, G.unity.Input.mousePosition)
        elseif isDrag then
            drag(self, 0, G.unity.Input.mousePosition)
        elseif isUp then
            up(self, 0, G.unity.Input.mousePosition)
        end

        scroll(self, G.unity.Input.GetAxis(MouseAxis))
    end

    clean(self)
end

fire = function(self, isUI, type, ...)
    if isUI then
        G.game.router:route(G.game.event.Input.UI_INPUT, type, ...)
    else
        G.game.router:route(G.game.event.Input.INPUT, type, ...)
    end
end

init = function(self)
    self.touchs = {}
    self.finishedTouches = {}
    self.zooms = {}
end

reset = function(self)
    if not self.doing then
        return
    end
    init(self)
end

getKeyboard = function(self)
    ------------------------------
    -- 快捷键:
    -- ←/a  镜头向左平移
    -- →/d  镜头向右平移
    -- ↑/w  镜头向上平移
    -- ↓/s  镜头向下平移
    if G.unity.Input.GetKey(G.unity.KeyCode.W) or G.unity.Input.GetKey(G.unity.KeyCode.UpArrow) then
        fire(self, false, G.game.const.InputType.Drag, 0, self.vector1, G.unity.Vector3.zero)
    elseif G.unity.Input.GetKey(G.unity.KeyCode.S) or G.unity.Input.GetKey(G.unity.KeyCode.DownArrow) then
        fire(self, false, G.game.const.InputType.Drag, 0, self.vector2, G.unity.Vector3.zero)
    elseif G.unity.Input.GetKey(G.unity.KeyCode.A) or G.unity.Input.GetKey(G.unity.KeyCode.LeftArrow) then
        fire(self, false, G.game.const.InputType.Drag, 0, self.vector3, G.unity.Vector3.zero)
    elseif G.unity.Input.GetKey(G.unity.KeyCode.D) or G.unity.Input.GetKey(G.unity.KeyCode.RightArrow) then
        fire(self, false, G.game.const.InputType.Drag, 0, self.vector4, G.unity.Vector3.zero)
    elseif G.unity.Input.GetKey(G.unity.KeyCode.BackQuote) then
    end

    if self.keyCodeListener then
        if nil == self.keyCodeTime then
            self.keyCodeTime = {}
        end
        local time = G.C.realtimeSinceStartup

        for _, keyCode in ipairs(self.keyCodeListener) do
            if G.unity.Input.GetKey(keyCode) then
                local last = self.keyCodeTime[keyCode] or 0
                if time - last > KEYCODE_DURATION then
                    fire(self, false, G.game.const.InputType.KeyCode, keyCode)
                    self.keyCodeTime[keyCode] = time
                end
            end
        end
    end
end

down = function(self, finger, pos)
    local result = G.api.View.RaycastUI(pos)
    self.touchs[finger] = {
        ["beginTime"] = G.C.realtimeSinceStartup,
        ["pos"] = pos,
        ["isUI"] = result
    }
    fire(self, result, G.game.const.InputType.Down, finger, pos)
    updateZoom(self, finger)
end

drag = function(self, finger, pos)
    if nil == self.touchs[finger] then
        return
    end

    local touch = self.touchs[finger]
    if distance(pos, touch.pos) > 0 then
        fire(self, touch.isUI, G.game.const.InputType.Drag, finger, pos, touch.pos)
        updateZoom(self, finger)
        touch.pos = pos
    end
end

up = function(self, finger, pos)
    if nil == self.touchs[finger] then
        return
    end

    local time = G.C.realtimeSinceStartup
    local touch = self.touchs[finger]
    if time - touch.beginTime >= LONG_TIME then
        fire(self, touch.isUI, G.game.const.InputType.LongTouch, pos, touch.pos)
    end

    touch.pos = pos
    self.touchs[finger] = nil

    local finishedTouch = self.finishedTouches[finger]
    if finishedTouch and time - finishedTouch.beginTime <= DOUBLE_DURATION then
        fire(self, touch.isUI, G.game.const.InputType.DoubleTouch, pos, finishedTouch.pos)
    else
        fire(self, touch.isUI, G.game.const.InputType.SingleTouch, pos)
    end
    fire(self, touch.isUI, G.game.const.InputType.Up, finger, pos)

    self.finishedTouches[finger] = touch

    removeZoom(self, finger)
end

scroll = function(self, val)
    if val > 0 then
        fire(self, false, G.game.const.InputType.Zoom, -1, -1, -1.4)
    elseif val < 0 then
        fire(self, false, G.game.const.InputType.Zoom, 1, 1, 1.4)
    end
end

clean = function(self)
    local time = G.C.realtimeSinceStartup
    local cleanArr
    for finger = 0, MAX_FINGER do
        local touch = self.finishedTouches[finger]
        if touch and time - touch.beginTime > DOUBLE_DURATION then
            if nil == cleanArr then
                cleanArr = {}
            end
            tb_insert(cleanArr, finger)
        end
    end

    if cleanArr then
        for _, finger in ipairs(cleanArr) do
            self.finishedTouches[finger] = nil
        end
    end
end

updateZoom = function(self, finger)
    local touch = self.touchs[finger]
    if nil == touch then
        return
    end

    for f = 0, MAX_FINGER do
        local t = self.touchs[f]
        if t and f ~= finger then
            local key
            local vec
            if f > finger then
                key = f * 100 + finger
                vec = t.pos - touch.pos
            else
                key = finger * 100 + f
                vec = touch.pos - t.pos
            end

            local x, y = vec.x, vec.y
            local old = self.zooms[key]
            if old then
                local dis = math_sqrt(x * x + y * y)
                local oldDis = math_sqrt(old.x * old.x + old.y * old.y)
                fire(self, touch.isUI, G.game.const.InputType.Zoom, x - old.x, y - old.y, dis - oldDis)
            end

            self.zooms[key] = {
                ["x"] = x,
                ["y"] = y
            }
        end
    end
end

removeZoom = function(self, finger)
    local removeArr = {}
    for k, _ in pairs(self.zooms) do
        local a, b = k // 100, k % 100
        if a == finger or b == finger then
            tb_insert(removeArr, k)
        end
    end

    for _, k in ipairs(removeArr) do
        self.zooms[k] = nil
    end
end

distance = function(p1, p2)
    local dx = p1.x - p2.x
    local dy = p1.y - p2.y
    return math_sqrt(dx * dx + dy * dy)
end

return InputComponent
