local tb_insert = table.insert
local tb_concat = table.concat
local str_format = string.format

local ConfigGenUtil = {}

local function exportstring(s)
    return str_format("%q", s)
end

local function isArrayTable(t)
    if type(t) ~= "table" then
        return false
    end
 
    local n = #t
    for i,v in pairs(t) do
        if type(i) ~= "number" then
            return false
        end
        
        if i > n then
            return false
        end 
    end
 
    return true 
end

local function exporttable(t, layer)
    if nil == layer then layer = 1 end
    local spaceArr = {}
    local spaceArr1 = {}
    for i = 1, layer do
        tb_insert(spaceArr, "\t")
        if i < layer then
            tb_insert(spaceArr1, "\t")
        end
    end
    local spaceStr = tb_concat(spaceArr)
    local spaceStr1 = tb_concat(spaceArr1)

    local result = "{"

    -- 数组
    if isArrayTable(t) then
        for _, v in ipairs(t) do
            result = result .. "\n" .. spaceStr
            local stype = type(v)
            if stype == "table" then
                result = result .. exporttable(v, layer + 1) .. ", "
            elseif stype == "string" then
                result = result .. exportstring(v) .. ", "
            elseif stype == "number" then
                result = result .. tostring(v) .. ", "
            else
                result = result .. tostring(v) .. ", "
            end
        end
    else -- 字典
        for k, v in pairs(t) do
            local needExport = false

            -- key
            local kstype = type(k)
            local str = ""
            if kstype == "string" then
                needExport = true
                result = result .. "\n" .. spaceStr
                local numKey = tonumber(k)
                if numKey then
                    str = "[" .. numKey .. "] = "
                else
                    -- str = exportstring(k).."="
                    str = k .. " = "
                end
            elseif kstype == "number" then
                str = "[" .. tostring(k) .. "] = "
                needExport = true
                result = result .. "\n" .. spaceStr
            end
            -- value
            if needExport then
                local stype = type(v)
                if stype == "table" then
                    result = result .. str .. exporttable(v, layer + 1) .. ", "
                elseif stype == "string" then
                    result = result .. str .. exportstring(v) .. ", "
                elseif stype == "number" then
                    result = result .. str .. tostring(v) .. ", "
                else
                    result = result .. str .. tostring(v) .. ", "
                end
            end
        end
    end

    return result .. "\n" .. spaceStr1 .. "}"
end

function ConfigGenUtil.GenConfig(cfgName, cfgData)
    return "local " .. cfgName .. " = " .. exporttable(cfgData) .. "\n\nreturn " .. cfgName
end

return ConfigGenUtil