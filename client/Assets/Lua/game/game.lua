local Router = require("core.Router")
local Language = require("game.Language")
local Localization = require("game.Localization")
local InputComponent = require("game.component.InputComponent")
local Game = {}

-- forward declares
local setupEnv = nil
local onLevelWasLoaded = nil
local onAppQuit = nil
local onSdk = nil
local onView = nil
local onComponent = nil
local onNet
local initServices = nil
local checkUpdate = nil

-- 游戏启动
function Game:start()
    setupEnv(self)
    self.router = Router.new()
    self.const = require("game.Const")
    self.event = require("game.Event")
    self.registry = require("game.Registry")
    self.config = require("game.Config")
    self.language = Language.new(self)
    self.localization = Localization.new(self)
    self.http = require("game.Http").new(self.router)
    self.time = require("game.Time").new()

    initServices(self, self.registry.preloadServices)
    -- checkUpdate(self)         //暂时不考虑更新
    self.states:change("loading")
end

-- 驱动
function Game:loop()
    -- 计划不进行主动驱动，各自使用定时器去驱动如果有需要的话。
end

-- 游戏结束
function Game:stop()
    self.views:shutdownall()
    self.audios:shutdown()
    print("* game stop")
end

-- invoke from cs
function Game:invoke(type, id, func, ...)
    -- G.log.trace("[Game].invoke {0} {1} {2}", type, id, func)
    if type == "game" then
        if id == "unity" then
            if func == "LevelWasLoaded" then
                onLevelWasLoaded(self, ...)
            elseif func == "ApplicationQuit" then
                onAppQuit(self, ...)
            end
        elseif id == "net" then
            onNet(self, func, ...)
        end
    elseif type == "sdk" then
        onSdk(self, id, func, ...)
    elseif type == "view" then
        onView(self, id, func, ...)
    elseif type == "component" then
        onComponent(self, id, func, ...)
    end
end

-- 开始检查更新
checkUpdate = function(self)
    self.states:change("update")
end

-- 更新完成，正式进入游戏
function Game:startGame()
    self.protodef = require("game.ProtoDefine")
    self.proto = require("game.Proto").new()
    self.player = require("game.model.Player").new()
    self.ctrls:initGameCtrls()
    G.api.Asset.viewroot:AddComponent(InputComponent)
    G.api.Tweening.Init(false, true, 500, 50)
    self.config:initScreens()
    self.states:change("login")
    print("* start game")                              
end

-- 设置游戏环境
setupEnv = function(self)
    local rate = G.unity.Application.isMobilePlatform and 60 or 60
    G.api.SetFrameRate(rate)

    if G.config.debug then
        G.api.Debug.FpsStart("screen")

        if G.unity.Application.isMobilePlatform then
            G.api.Debug.ShowConsole()
        end
    end
end

initServices = function(self, services)
    for name, cls in pairs(services) do
        name = string.lower(name)
        self[name] = cls.new(self)
    end
end

onLevelWasLoaded = function(self, ...)
    self.world:onCommand("world", "levelWasLoaded")
end

onAppQuit = function(self, ...)
    self.router:route(self.event.App.QUIT, ...)
    G.api.Net.Disconnect()
end

onSdk = function(self, id, func, ...)
    G.log.trace("[Game].invoke sdk {0} {1}", id, func)
    self.sdks:invoke(id, func, ...)
end

onView = function(self, id, func, ...)
    self.views:invoke(id, func, ...)
end

onComponent = function(self, _, func, com, cmd, ...)
    G.print.xpcall(com[func], com, cmd, ...)
end

onNet = function(self, func, ...)
    if func == "FireProto" then
        self.proto:fire(...)
    elseif func == "OnConnected" then
        self.router:route(self.event.Net.ON_CONNECTED)
    elseif func == "OnConnectFailed" then
        self.router:route(self.event.Net.ON_CONNECT_FAILED)
    elseif func == "OnDisconnected" then
        self.router:route(self.event.Net.ON_DISCONNECTED)
    end
end

return Game
