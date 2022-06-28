--[[
    GameState
]]
local BaseState = require("game.state.BaseState")
local GameState = class(BaseState)

function GameState:enter(lastState)
    self.router:route(G.game.event.State.ENTER_GAME)
end

function GameState:leave(nextState)
    self.router:route(G.game.event.State.LEAVE_GAME)
end

return GameState
