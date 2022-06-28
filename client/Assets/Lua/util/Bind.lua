--[[
  Id: Bind.lua
  Desc: Bind工具函数
  Author: figo
  Date: 2020-04-21 14:37:44
--]]

local pack = pack or table.pack
local unpack = unpack or table.unpack

-- 解决原生pack的nil截断问题，safe_pack与safe_unpack要成对使用
local function safe_pack(...)
    local params = {...}
    params.n = select('#', ...)
    return params
end

-- 解决原生pack的nil截断问题，safe_pack与safe_unpack要成对使用
local function safe_unpack(tbl) 
    return unpack(tbl, 1, tbl.n)
end


-- 对两个safe_pack的表执行连接
local function concat_safe_pack(pack1, pack2)
    local c = {}
    for i = 1, pack1.n do
        c[i] = pack1[i]
    end
    for i = 1, pack2.n do
        c[i+pack1.n] = pack2[i]
    end
    c.n = pack1.n + pack2.n
    return c
end

function bind(self, func, ...)
    if func == nil then
        G.log.error("===== bind func error")
        return
    end
    local params = nil
    if self == nil then
        params = safe_pack(...)
    else
        params = safe_pack(self, ...)
    end

    return function(...) 
        local args = concat_safe_pack(params, safe_pack(...))
        -- G.print.table(args, "test bind", 2)
        return func(safe_unpack(args))
    end
end

--[[
-- 回调绑定
-- 重载形式：
-- 1、成员函数、私有函数绑定：bind_callback(obj, callback, ...)
-- 2、闭包绑定：bindcallback(callback, ...)
function bind_callback(...)
    local bindFunc = nil
    local params = safe_pack(...)
    assert(params.n >= 1, "bind_callback : error params count!")
    if type(params[1]) == "table" and type(params[2]) == "function" then
        bindFunc = bind(...)
    elseif type(params[1]) == "function" then
        bindFunc = bind(nil, ...)
    else
        error("BindCallback : error params list!")
    end
    return bindFunc
end
--]]

--[[
function toboolean(s)
    local trans_map = {
        ["true"] = true,
        ["false"] = false
    }
    return trans_map[s]
end
]]

return bind