
--[[
  InitState
]]
local BaseState = require("game.state.BaseState")
local LoadingState = class(BaseState)

function LoadingState:enter(lastState, data)
    self.router:route(G.game.event.State.ENTER_LOADING)
end

function LoadingState:leave(nextState)
    self.router:route(G.game.event.State.LEAVE_LOADING)
end

return LoadingState