
local View = require("game.view.LogicView")
local table_insert = table.insert
local table_remove = table.remove

local ViewGroup = View.ViewGroup
local DestroyMode = View.DestroyMode

local ViewsSvc = class()

-- forward declares
local init
local check
local load
local push
local pop
local hidegroup
local languageChanged

function ViewsSvc:ctor(game)
    self.game = game
    self.router = game.router
    init(self)
end

-- 显示
function ViewsSvc:open(name, handle, ctx)
    if G.game.registry.views[name] == nil then
        G.log.error("invalid view: {0}", name)
        return
    end

    local loadhandle = function(self, handle, view, ctx)
        if handle ~= nil then
            handle(view, ctx)
        end
        view:show(ctx)
    end

    load(self, name, G.bind(self, loadhandle, handle), ctx)
end

-- 关闭
function ViewsSvc:close(name)
    local view = self.views[name]
    if view then
        if self.pendings[name] then
            self.pendings[name] = nil
        end
        view:hide()
    end
end

-- 销毁
function ViewsSvc:shutdown(name)
    if self.pendings[name] then
        self.pendings[name] = nil
    end
    local view = self.views[name]
    if view then
        view:destroy()
    end
end

function ViewsSvc:shutdownall()
    self.pendings = {}
    self.stacks = {lock = true}

    -- destroy all view
    local views = {}
    for name, view in pairs(self.views) do
        table_insert(views, view)
    end
    for _, view in ipairs(views) do
        view:destroy()
    end
    self.views = {}
    self.stacks.lock = false
end

function ViewsSvc:update()
end

function ViewsSvc:invoke(name, func, ...)
    local view = self.views[name]
    if not view then
        G.log.error("view invoke error, can't find view: {0} func: {1}", name, func)
        return
    end
    local group, mode = view.group, view.mode

    -- call view method safely
    G.print.xpcall(view[func], view, ...)

    if func == "load" then
        if view.__pendings then
            for i = 1, #view.__pendings do
                local pending = view.__pendings[i]
                if pending[1] then
                    pending[1](view, pending[2])
                end
            end
            view.__pendings = nil
        end
    elseif func == "onEnabled" then
        -- 互斥
        if self.settings.mutex[group] then
            hidegroup(self, group, name)
        end
        -- 压栈
        if self.settings.rollback[group] then
            push(self, view)
        end
    elseif func == "onDisable" then
        -- 弹栈
        if self.settings.rollback[group] then
            pop(self, view)
        end
        if mode == DestroyMode.hide then
            view:destroy()
        end
    elseif func == "onDestroy" then
        self.views[name] = nil
    end
end

init = function(self)
    self.views = {}
    self.pendings = {}

    self.settings = {mutex = {}, rollback = {}}
    -- 互斥视图组
    self.settings.mutex[ViewGroup.window] = true
    -- 压栈视图组
    self.settings.rollback[ViewGroup.window] = true
    -- 回退栈
    self.stacks = {lock = false}

    self.router:on(G.game.event.View.OPEN, G.bind(self, self.open))
    self.router:on(G.game.event.View.CLOSE, G.bind(self, self.close))
    self.router:on(G.game.event.View.CHECK, G.bind(self, check))
    -- self.router:on(G.game.event.Language.LANGUAGE_CHANGED, G.bind(self, languageChanged))
end

check = function(self, handle, name)
    assert(type(handle) == "function")
    if name then
        local view = self.views[name]
        if view then
            handle(view)
        end
        return
    end

    for _, view in pairs(self.views) do
        if not handle(view) then
            break
        end
    end
end

load = function(self, name, handle, data)
    local view = self.views[name]
    if view ~= nil and view.loaded then
        if handle ~= nil then
            handle(view, data)
        end
        return
    end

    local cls = G.game.registry.views[name]
    assert(type(cls) == "table")

    local ctrl = self.game.ctrls:get(cls.__ctrl.name)
    assert(ctrl)
    view = cls.new(ctrl)
    view.__pendings = view.__pendings or {}
    table.insert(view.__pendings, {handle, data})
    
    self.views[name] = view
    view:show()
end

-- 压栈
push = function(self, view)
    if self.stacks.lock then
        return
    end

    local group = view.group
    local list = {source = view, name = view.name}

    self.stacks.lock = true
    for name, v in pairs(self.views) do
        if name ~= view.name and v.group == group and v:isShow() then
            v:hide()
            table_insert(list, v)
        end
    end
    if #list > 0 then
        table_insert(self.stacks, list)
    end
    self.stacks.lock = false
end

-- 弹栈
pop = function(self, view)
    if self.stacks.lock or #self.stacks == 0 then
        return
    end
    self.stacks.lock = true

    local n = #self.stacks
    if self.stacks[n].source ~= view then
        self.stacks.lock = false
        return
    end

    local list = self.stacks[n]
    for _, v in ipairs(list) do
        v:show()
    end

    table_remove(self.stacks, n)
    self.stacks.lock = false
end

hidegroup = function(self, group, exclusion)
    for name, view in pairs(self.views) do
        if name ~= exclusion and view.group == group then
            view:hide()
        end
    end
end

languageChanged = function(self)
    for name, v in pairs(self.views) do
        if v:isShow() then
            v:refreshLanguage()
        end
    end
end

return ViewsSvc

--[[

显示1 ->互斥->压栈1
关闭1 ->退栈2

]]
