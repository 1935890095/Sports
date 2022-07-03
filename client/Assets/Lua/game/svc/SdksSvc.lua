
local json = require("dkjson")
local string_lower = string.lower

local SdksSvc = class()

function SdksSvc:ctor(game)
    self.router = game.router
end

function SdksSvc:invoke(func, ...)
    G.print.debug("SdksSvc:invoke")
end

return SdksSvc
