--[[
  Author: nzh
  Desc: lua层component驱动对象
  Date: 2020-11-10 11:49:07
--]]
local class = class or require("core.Class").class
local tblpack = table.pack
local tblunpack = table.unpack
local base = require("core.Component")
local Render = class(base)

function Render:ctor()
    self.render = nil -- renderObject
    self.this = nil -- luaComponent
    self.components = {}
    self.cmds = {}

    self.renders = {}

    self.created = false
    self.started = false
    self.enabled = false
end

-------------- lua ----------------------------

function Render:getAsset()
    return self.render
end

function Render:destroy()
    if self.render then
        self.render:Destroy()
    end
end

function Render:addComponent(component, ...)
    local com = component.new(...)
    table.insert(self.components, com)

    if self.created then
        com.render = self.render
        com.this = self.this
        com:onCreate()
    end

    if self.started then
        com:onStart()
    end

    if self.enabled then
        com:onEnabled()
    end
end

function Render:addChildRender(render)
    table.insert(self.renders, render)
end

function Render:onUpdate(delta)
    for _, component in ipairs(self.components) do
        component:onUpdate(delta)
    end

    for _, render in ipairs(self.renders) do
        render:onUpdate(delta)
    end
end

function Render:onCommand(cmd, ...)
    if not self.created then
        table.insert(self.cmds, {cmd, tblpack(...)})
        return
    end
    for _, component in ipairs(self.components) do
        component:onCommand(cmd, ...)
    end

    for _, render in ipairs(self.renders) do
        render:onCommand(cmd, ...)
    end
end

-------------------------------------------------

function Render:onCreate()
    for _, component in ipairs(self.components) do
        component.render = self.render
        component.this = self.this
        component:onCreate()
    end

    if #self.cmds > 0 then
        for _, cmd in ipairs(self.cmds) do
            for _, component in ipairs(self.components) do
                component:onCommand(cmd[1], tblunpack(cmd[2]))
            end
        end
        self.cmds = {}
    end
    self.created = true
    self.render.oncmd = false
end

function Render:onStart()
    for _, component in ipairs(self.components) do
        component:onStart()
    end
    self.started = true
end

function Render:onDestroy()
    self.render = nil
    for _, component in ipairs(self.components) do
        component:onDestroy()
    end
end

function Render:onEnabled()
    for _, component in ipairs(self.components) do
        component:onEnabled()
    end
    self.this.enabled = false
    self.enabled = true
end

function Render:onDisable()
    for _, component in ipairs(self.components) do
        component:onDisable()
    end
end

return Render
