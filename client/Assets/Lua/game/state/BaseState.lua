--[[
    State base class
]]
local BaseState = class()

function BaseState:ctor(svc, router)
    self.svc = svc
    self.router = router
end

function BaseState:enter(lastState, data) end
function BaseState:leave(nextState) end
function BaseState:loop() end

return BaseState
