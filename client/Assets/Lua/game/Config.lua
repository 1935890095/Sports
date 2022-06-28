--[[
  Id: Config.lua
  Desc: 配置读取
--]]
local json = require("dkjson")

local Config = {}


--配置保存
function Config:Save(key, value)
    assert(type(key) == "string")
    if not next(value) then 
        return 
    end

    Config[key] = value
end

--配置读取
function Config:Get(key)
    return Config[key]
end

-- 配置读取脏代码统一写这里
function Config:saveSettings()
    G.api.Cache.Save("settings", json.encode(self.settings))
end

local str = G.api.Cache.Get("settings")
if str then
    Config.settings = str and json.decode(str)
else
    Config.settings = {
        lang = nil, -- 默认根据系统语言来定
        music = true,
        sound = true,
        adaptive = false,
        server = 1 --服务器
    }
    Config:saveSettings()
end


function Config:initScreens()
    local screens = {}
    for key, value in pairs(self.screen or {}) do
        screens[string.lower(key)] = value
    end
    self.screen = screens
end

function Config:getNotchHeight(name)
    local height = G.api.Screen.NotchHeight
    if height > 0 then
        return height
    end

    name = string.lower(name)
    height = self.screen[name]
    if height ~= nil and type(height) == "number" then
        if height <= 0 then
            height = G.game.const.DEFAULT_NOTCH_HEIGHT
        end
    end
    return height
end

return Config
