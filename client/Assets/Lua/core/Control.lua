--[[
  Id: Control.lua
  Desc: 控制器
  Author: figo
  Date: 2020-04-22 19:00:59
--]]

local table_insert = table.insert
local table_remove = table.remove
local type = type
local class = class or require("core.Class").class

local Router = require("core.Router")
local Control = class()

function Control:ctor(router)
    -- 注：禁止在视图中使用router
    self.router = router
    self.erouter = Router.new() -- 视图事件路由
    self.estubs = {}            -- 视图事件存根
    self.name = nil

    if type(self.init) == "function" then
        self:init()
    end
end

-- 通知视图事件
function Control:notify(what, ...) 
    self.erouter:route(what, ...)
end

-- 视图注册事件
function Control:onNotify(view, what, handle)
    assert(type(view) == "table")
    local name = view.name
    assert(type(name) == "string")
    if self.estubs[name] == nil then
        self.estubs[name] =  {}
    end
    table_insert(self.estubs[name], {what, handle})
    self.erouter:on(what, handle)
end

-- 视图注销事件
function Control:offNotify(view, what, handle)
    assert(type(view) == "table")
    local name = view.name
    assert(type(name) == "string")
    local stubs = self.estubs[name]
    if stubs ~= nil then
        for i = 1, #stubs do
            local stub = stubs[i]
            if (stub[1] == what) and (stub[2] == handle) then
                table_remove(stubs, i)
                break;
            end
        end
        if #stubs == 0 then
            self.estubs[name] = nil
        end
    end
    self.erouter:off(what, handle)
end

-- 视图创建/显示/关闭/销毁(如果重载，一定要记得调用基类的!!!)
function Control:onCreate(view) end
function Control:onOpen(view) end
function Control:onClose(view) end
function Control:onDestroy(view) 
    -- off all events when view destroy
    local name = view.name
    local stubs = self.estubs[name]
    if stubs ~= nil then
        for i = 1, #stubs do
            local stub = stubs[i]
            local type, handle = stub[1], stub[2]
            self.erouter:off(type, handle)
        end
        self.estubs[name] = nil
    end
end

function Control:hint(message, ...)
    self.router:route(G.game.event.Msg.HINT, message, ...)
end

return Control