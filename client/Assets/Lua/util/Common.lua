local tb_insert = table.insert
local tb_remove = table.remove
local math_random = math.random

local Common = {}

local Math = {}

function Math.distance(v1, v2)
    local dx = v2.x - v1.x
    local dy = v2.y - v1.y
    return math.sqrt(dx * dx + dy * dy)
    -- return G.api.Misc.Distance(v1, v2)
end

function Math.bezier(p0, p1, t)
    return (1 - t) * p0 + t * p1
end

function Math.bezier2(p0, p1, p2, t)
    local p0p1 = Math.bezier(p0, p1, t)
    local p1p2 = Math.bezier(p1, p2, t)
    return Math.bezier(p0p1, p1p2, t)
end

function Math.bezier3(p0, p1, p2, p3, t)
    return Math.bezierN({p0, p1, p2, p3}, t)
end

function Math.bezierN(pArr, t)
    local pCount = #pArr
    if pCount == 0 then
        return nil
    end
    if pCount == 1 then
        return pArr[1]
    end

    local deepArr = {}
    for i = 1, pCount - 1 do
        tb_insert(deepArr, Math.bezier(pArr[i], pArr[i + 1], t))
    end
    return Math.bezierN(deepArr, t)
end

function Math.splitNum(num)
    local result = {}
    while true do
        tb_insert(result, 1, num % 10)
        if num < 10 then
            break
        end
        num = num // 10
    end
    return result
end

function Common.forward(camp)
    local c = camp // 100
    if c == 1 then
        return G.game.vector3:forward()
    elseif c == 2 then
        return G.game.vector3:back()
    else
        return G.game.vector3:zero()
    end
end

local Time = {}

function Time.countdown(seconds)
    local hour = math.floor(seconds % 86400 / 3600)
    local minute = math.floor(seconds % 86400 % 3600 / 60)
    local second = math.floor(seconds % 86400 % 3600 % 60 % 60)
    return string.format("%02d:%02d:%02d", hour, minute, second)
end

function Time.countdownH(seconds)
    local hour = math.floor(seconds / 3600)
    local minute = math.floor(seconds % 3600 / 60)
    local second = math.floor(seconds % 3600 % 60 % 60)
    return string.format("%02d:%02d:%02d", hour, minute, second)
end

function Time.countdownMS(seconds)
    local minute = math.floor(seconds % 86400 % 3600 / 60)
    local second = math.floor(seconds % 86400 % 3600 % 60 % 60)
    return string.format("%02d:%02d", minute, second)
end

function Time.countdownFl(seconds)
    local day = math.floor(seconds / 86400)
    local hour = math.floor(seconds % 86400 / 3600)
    local minute = math.floor(seconds % 86400 % 3600 / 60)
    local second = math.floor(seconds % 86400 % 3600 % 60 % 60)
    return {["day"] = day, ["hour"] = hour, ["minute"] = minute, ["second"] = second}
end

-- math functions
Common.math = Math
Common.time = Time

function Common.camp(camp)
    local v = camp // 100
    if v ~= 1 and v ~= 2 then
        return 3
    end
    return v
end

function Common.createWorldData(lv)
    local config = G.game.config.levels[lv]
    if nil == config or nil == config.data then
        G.log.error("level data can`t found, lv: " .. lv)
        return
    end
    local lvData = require(config.data)

    local mapData = {}
    local strategyData = {}
    local strategy = nil

    mapData.name = config.res
    mapData.data = lvData.map
    mapData.cameraBeziers = lvData.cameraBeziers
    strategyData.params = lvData.params
    strategyData.place = lvData.place
    if config.type == G.game.const.BattleType.HOME then
        strategy = G.game.registry.strategies.Home
    elseif config.type == G.game.const.BattleType.PVE then
        strategy = G.game.registry.strategies.PVE
    elseif config.type == G.game.const.BattleType.RESCUE then
        strategy = G.game.registry.strategies.Rescue
    elseif config.type == G.game.const.BattleType.DEFENSIVE then
        strategy = G.game.registry.strategies.Defensive
    end

    return {["mapData"] = mapData, ["strategyData"] = strategyData, ["strategy"] = strategy}
end

-- view
-- tmp: 模板
-- list：容器
-- count
function Common.clone(view, tmp, res, list, count, name)
    if #list == 0 then
        list[1] = res.load(view.name, tmp.self)
    end
    for i = 1, count do
        local item = list[i]
        if item then
            view:GameObject(item.self, "active", true)
        else
            local name = name and name .. i or res.__name .. i
            local go = view:GameObject(tmp.self, "clone", name)
            local v = res.load(view.name, go)
            tb_insert(list, v)
        end
    end
    for i = count + 1, #list do
        view:GameObject(list[i].self, "active", false)
    end
end

function Common.merelyClone(view, tmp, list, count)
    if not list then
        return
    end

    tb_insert(list, tmp)

    for i = 1, count do
        local item = list[i]
        if not item then
            item = view:GameObject(tmp, "clone")
            tb_insert(list, item)
        end
        view:GameObject(item, "active", true)
    end
    for i = count + 1, #list do
        view:GameObject(list[i].self, "active", false)
    end
end

function Common.tblContains(tbl, value)
    for k, v in pairs(tbl) do
        if v == value then
            return true
        end
    end
    return false
end

function Common.tblCount(tbl)
    local count = 0
    if tbl then
        for k, v in pairs(tbl) do
            count = count + 1
        end
    end
    return count
end

function Common.tblFind(tab, val)
    for k, v in pairs(tab) do
        if v == val then
            return k
        end
    end
    return nil
end

function Common.tblRemove(tab, val)
    for k, v in pairs(tab) do
        if v == val then
            tb_remove(tab, k)
            break
        end
    end
end

function Common.isSameDay(timeTick) --####
    return os.date("%d", timeTick) == os.date("%d")
end

function Common.checkRate(rate)
    local r = math_random(1, G.game.const.MAX_RATE)
    return r <= rate
end

-- 六位16进制数转rgb
function Common.sixNumToRGB(str_num)
    local str_num_six
    if string.len(str_num) == 6 then
        str_num_six = str_num
    else
        str_num_six = string.sub(str_num, 1, 7)
    end

    local r = tonumber(string.sub(str_num_six, 1, 2), 16)
    local g = tonumber(string.sub(str_num_six, 3, 4), 16)
    local b = tonumber(string.sub(str_num_six, 5, 6), 16)

    return r, g, b
end

function Common.sixNumToColor(str_num, a)
    local r, g, b = Common.sixNumToRGB(str_num)
    a = a or 1
    return G.unity.Color(r / 255, g / 255, b / 255, a)
end

function Common.floatRandom(min, max)
    local offset = max - min
    if offset <= 0 then
        return min
    end

    local val = math_random()
    return min + val * offset
end

function Common.uiSize()
    local width = G.unity.Screen.width
    local height = G.unity.Screen.height
    local ratio = 1080 / 1920
    local curRatio = width / height
    if math.abs(curRatio - ratio) < 0.01 then -- 实际宽高比 比 设计宽高比 相等，直接使用设计分辨率
        return 1080, 1920
    elseif curRatio < ratio then -- 实际宽高比 比 设计宽高比 更小，固定宽度，将高度按实际宽高比进行缩放
        return 1080, 1080 / curRatio
    else -- 实际宽高比 比 设计宽高比 更小，固定高度，将宽度按实际宽高比进行缩放
        return 1920 * curRatio, 1920
    end
end

--[[
    @desc: 获取货币图标
    @type: 货币类型（1， 2， 3...）
]]
function Common.getCurrencyIcon(type_)
    if type_ and type(type_) == "number" then
        if type_ == 1 then -- 钻石
            return {atlas = "icon", name = "icon_diamond"}
        elseif type_ == 2 then -- 金币
            return {atlas = "icon", name = "icon_gold"}
        elseif type_ == 3 then -- 兑换券
            return {atlas = "icon", name = "icon_diamond"}
        end
    end
end

--表元素去重。
function Common.removeRepeat(a)
    local b = {}
    for k, v in ipairs(a) do
        if (#b == 0) then
            b[1] = v
        else
            local index = 0
            for i = 1, #b do
                if (v == b[i]) then
                    break
                end
                index = index + 1
            end
            if (index == #b) then
                b[#b + 1] = v
            end
        end
    end
    return b
end


-- local __chars = {
--   '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
--   'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
--   'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
-- }

function Common.encodeID(num)
    -- local res = {}
    -- while num > 0 do
    --     mod = num % 62
    --     -- 必须得这么处理，不然会由于LUA本身的精度丢失，得出与服务器不一样的结果
    --     print("", num, "=>", __chars[mod+1])
    --     num = math.modf(num/62)
    --     pos = mod + 1
    --     table.insert(res, __chars[pos])
    -- end

    -- local text = table.concat(res)
    -- return string.reverse(text)
    return G.api.Util.EncodeID(num)
end

--电话格式判断:country 国家1:俄罗斯 2：其他国家
function Common.Phone(phonenum, country)
    local char = string.byte(phonenum)
    if country == 1 then
        if string.len(phonenum) == 11 and (tonumber(char[1]) == 7 or tonumber(char[1]) == 8) then
            return phonenum
        end
        return 
    end
end

--[[
    @desc: 格式化货币，从1000开始，保留两位小数
    @isNotShowZero: 是否在小数为零时不显示小数
]]
function Common.formatMoneyStartK(value, isNotShowZero)
    if nil == isNotShowZero then
        isNotShowZero = true
    end
    if value < 1000 then
        return tostring(value)
    else
        if isNotShowZero then
            local formtStr = string.format("%.2f", math.floor((value / 1000)))
            local strs = string.split(formtStr, '.')
            if strs and #strs == 2 then
                local num = tonumber(strs[2])
                if num > 0 then
                    if num % 10 == 0 then
                        local inte = string.format("%.0f", num /10)
                        return strs[1] .. "." .. inte .. "K"
                    else
                        return formtStr .. "K"
                    end
                else
                    return strs[1] .. "K"
                end
            else
                return formtStr .. "K"
            end
        else
            return string.format("%.2fK", math.floor((value / 1000)))
        end
    end
end

-- 格式化货币显示
-- 向下取整保留两位小数
-- @isNotShowZero: 是否在小数为零时不显示小数
function Common.formatMoney(value, isNotShowZero)
    if nil == isNotShowZero then
        isNotShowZero = true
    end
    if value < 100000 then
        return tostring(value)
    elseif value >= 100000 and value < 1000000 then
        if isNotShowZero then
            local formtStr = string.format("%.2f", math.floor((value / 1000) * 100) / 100)
            local strs = string.split(formtStr, '.')
            if strs and #strs == 2 then
                local num = tonumber(strs[2])
                if num > 0 then
                    if num % 10 == 0 then
                        local inte = string.format("%.0f", num /10)
                        return strs[1] .. "." .. inte .. "K"
                    else
                        return formtStr .. "K"
                    end
                else
                    return strs[1] .. "K"
                end
            else
                return formtStr .. "K"
            end
        else
            return string.format("%.2fK", math.floor((value / 1000) * 100) / 100)
        end
    elseif value >= 1000000 and value < 1000000000 then
        if isNotShowZero then
            local formtStr = string.format("%.2f", math.floor((value / 1000000) * 100) / 100)
            local strs = string.split(formtStr, '.')
            if strs and #strs == 2 then
                local num = tonumber(strs[2])
                if num > 0 then
                    if num % 10 == 0 then
                        local inte = string.format("%.0f", num /10)
                        return strs[1] .. "." .. inte .. "M"
                    else
                        return formtStr .. "M"
                    end
                else
                    return strs[1] .. "M"
                end
            else
                return formtStr .. "M"
            end
        else
            return string.format("%.2fM", math.floor((value / 1000000) * 100) / 100)
        end
    elseif value >= 1000000000 then
        if isNotShowZero then
            local formtStr = string.format("%.2f", math.floor((value / 1000000000) * 100) / 100)
            local strs = string.split(formtStr, '.')
            if strs and #strs == 2 then
                if tonumber(strs[2]) > 0 then
                    return formtStr .. "B"
                else
                    return strs[1] .. "B"
                end
            else
                return formtStr .. "B"
            end
        else
            return string.format("%.2fB", math.floor((value / 1000000000) * 100) / 100)
        end
    end
end

--[[
    @desc: 格式化数字显示，没三位加“，”
]]
function Common.formatNum(num)
    if num then
        assert(type(num) == "number")

        local strTab = {}
        local resTab = {}

        if num < 0 then
            num = -num
            table.insert(resTab, "-")
        end

        for v in string.gmatch(num, ".") do
            table.insert(strTab, v)
        end

        local rem = #strTab % 3
        for i=1, #strTab do
            table.insert(resTab, strTab[i])
            if i == #strTab then
                break
            end
            if i == rem then
                table.insert(resTab, ",")
            elseif (i - rem) % 3 == 0 then
                table.insert(resTab, ",")
            end
        end
        
        return table.concat(resTab)
    end
end

--[[

local repeatTime = 1
function Common.GetTextureByUrl(url, isCash, texName, hight, width, callBack, errorCallBack)
    if url then
        G.api.Http.GetTextureByUrl(
            url,
            isCash,
            texName,
            hight,
            width,
            function(tex)
                repeatTime = 1
                callBack(tex)
            end,
            function(error)
                errorCallBack(error)

                -- repeatTime = repeatTime - 1
                -- if repeatTime < 0 then
                --     repeatTime = 1
                --     errorCallBack(error)
                -- else
                --     Common.GetTextureByUrl(url, isCash, texName, hight, width, callBack, errorCallBack)
                -- end
            end
        )
    end
end

--]]
-- 不用
-- table转sdk传入参数string
function Common.table2SdkStr(data)
    local dataStr = ""
    for k, v in pairs(data) do
        dataStr = dataStr .. "&" .. k .. "=" .. tostring(v)
    end
    return string.sub(dataStr, 2)
end

return Common
