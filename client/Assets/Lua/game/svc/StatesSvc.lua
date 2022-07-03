

local LoadingState = require("game.state.LoadingState")
local LoginState = require("game.state.LoginState")
local GameState = require("game.state.GameState")

local string_lower = string.lower

local StatesSvc = class()

-- forward decleres
local init
local onChange

function StatesSvc:ctor(game)
    self.router = game.router
    init(self)
end

--[[
    @ignoreSame: 忽略相同state判断，即可在相同state间切换
]]
function StatesSvc:change(name, data, ignoreSame) 
    assert(type(name) == "string")
    name = string_lower(name)

    local state = self.states[name]
    if not state then
        G.log.error("state '{0}' is not registered", name)
        return
    end

    if not ignoreSame and state == self.current then
        -- 防止换切相同的state
        G.log.debug("* ignore change state {0}，already in", name)
        return
    end

    if self.current then
        self.current:leave(state) 
    end
    local last = self.current
    self.current = state
    state:enter(last, data)
end

init = function(self)
    self.states = { } 
    self.current = nil

    for name, cls in pairs(G.game.registry.states) do
        name = string_lower(name)
        self.states[name] = cls.new(self, self.router)
    end

    self.router:on(G.game.event.State.CHANGE, G.bind(self, onChange))
end

onChange = function(self, state, data, ignoreSame)
    self:change(state, data, ignoreSame)
end

return StatesSvc