--[[
  UpdateState
]]
local base = require("game.state.BaseState")
local UpdateState = class(base)

function UpdateState:enter(lastState, data)
    self.router:route(G.game.event.State.ENTER_UPDATE)
end

function UpdateState:leave(nextState)
    self.router:route(G.game.event.State.LEAVE_UPDATE)
end

return UpdateState
