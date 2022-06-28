--[[
  LoginState
]]
local BaseState = require("game.state.BaseState")
local LoginState = class(BaseState)

function LoginState:enter(lastState, data)
    self.router:route(G.game.event.State.ENTER_LOGIN)
end

function LoginState:leave(nextState)
    self.router:route(G.game.event.State.LEAVE_LOGIN)
end

return LoginState
