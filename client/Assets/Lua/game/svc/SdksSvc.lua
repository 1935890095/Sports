--[[
  Id: SdksSvc.lua
  Desc: Sdk服务
  Author: figo
  Date: 2020-04-28 18:48:59
--]]
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
