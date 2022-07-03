local base = require("core.Control")
local stringUtil = require("util.String")

local LoadingCtrl = class(base)

local onEnter

function LoadingCtrl:ctor(...)
    base.ctor(self, ...)
    self.progress = 0
    self.complete = 0
end

function LoadingCtrl:init()
    self.router:on(G.game.event.State.ENTER_LOADING, G.bind(self, onEnter))
end

--加载资源
onEnter = function(self)
    G.game:startGame()
end

return LoadingCtrl
