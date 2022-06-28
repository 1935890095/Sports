--[[
  Id: Localization.lua
  Desc: 本地化
--]]
local tb_insert = table.insert
local str_format = string.format

local Localization = class()

function Localization:ctor(game)
    self.files = {
    }
end

function Localization:change(lang)
    self.content = {}

    local files = self.files[lang]
    if files then
        for _, file in ipairs(files) do
            tb_insert(self.content, require(file))
        end
    end
end

function Localization:get(key, ...)
    if nil == key then
        return nil
    end

    local format
    for _, c in ipairs(self.content) do
        format = c[key]
        if format then
            break
        end
    end

    if nil == format then
        -- G.log.warn(key .. " can`t found")
        return key
    end

    local args = {...}
    if #args > 0 then
        return str_format(format, ...)
    else
        return format
    end
end

return Localization
