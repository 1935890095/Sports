--[[
--事件路由
--]]

local table_insert = table.insert
local table_remove = table.remove
local debug_traceback = debug.traceback
local tostring = tostring
local ipairs = ipairs
-- local xpcall = xpcall
local xpcall = G.print.xpcall
local class = class or require("core.Class").class

local M = class()

function M:ctor()
    self.name = "router"
	self.events = {}
end

function M:on(eid, handle)
	assert(eid ~= nil)
	assert(type(handle) == "function")
	
    self.events[eid] = self.events[eid] or {}
	local handles = self.events[eid]

	-- 重复注册检查
	for i = 1, #handles do 
		if handles[i] == handle then
			print(string.format('Router:On:%d, handle is repeat', eid))			
			return
        end
    end

    table_insert(handles, handle)
end

function M:off(eid, handle)
	assert(eid ~= nil)
	assert(type(handle) == "function")

    local handles = self.events[eid]
    if handles == nil then
        return
    end

    for i = 1, #handles do
        if handles[i] == handle then
            table_remove(handles, i)
            break
        end
    end

    if #handles == 0 then
        self.events[eid] = nil
    end
end

function M:route(eid, ...) 
	assert(eid ~= nil)

	local handles = self.events[eid]
	if handles == nil then
		return
	end

	--处理事件过程中，处理函数列表可能发生变化
	--先复制一份函数列表，每执行一次回调函数后重新检查函数是否还存在。

	--复制当前的事件处理函数列表
	local temp = {}
	for _, func in ipairs(handles) do
		table_insert(temp, func)
	end

    for idx = 1, #temp do
		--记录当前仍然存在的函数
		local currents = { }
		for _, func in ipairs(handles) do 
			currents[func] = true
		end

		--如果函数仍然存在则进行回调，否则不作处理。
		local handle = temp[idx]
		if currents[handle] ~= nil then
			xpcall(handle, ...)
		end
    end
end

return M