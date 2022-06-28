local base = require("core.Control")

local LoginCtrl = class(base)

local onEnter

function LoginCtrl:ctor(...)
    base.ctor(self, ...)
    self.loginOption = nil
end

function LoginCtrl:init()
    self.router:on(G.game.event.State.ENTER_LOGIN, G.bind(self, onEnter))

    self.router:on(G.game.event.Net.ON_CONNECTED, G.bind(self, onConnected))
    self.router:on(G.game.event.Net.ON_CONNECT_FAILED, G.bind(self, onConnectFailed))
    self.router:on(G.game.event.Net.ON_DISCONNECTED, G.bind(self, onDisconnected))

    G.game.proto:reg(G.game.protodef.Id.LoginResult, G.bind(self, onLoginResult))
end

function LoginCtrl:login(data)
    self.loginOption = data
    G.log.debug("* connecting to server {0}:{1}", G.config.server, G.config.port)
    G.log.debug("* check network reachability {0}", G.unity.Application.internetReachability)
    G.api.Net.Connect(G.config.server, G.config.port)
end

--连接成功
onConnected = function(self)
    --G.game.proto:send(G.game.protodef.Id.LoginRequest, self.loginOption)
end

--连接失败
onConnectFailed = function(self)

end

--断开连接
onDisconnected = function(self)

end

--进入登录
onEnter = function(self)
    self.router:route(G.game.event.View.OPEN, "LoginView")
end

--登录返回
onLoginResult = function(self, result)
    G.log.table(result)
end

return LoginCtrl
