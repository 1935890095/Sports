--[[
	desc: log helper
]]

-- local CSLOG = CS.LOG
-- local Application = CS.UnityEngine.Application
-- local IsMobilePlatform = Application.isMobilePlatform
-- local Debug = CS.UnityEngine.Debug
local string_format = string.format
local string_len = string.len
local string_rep = string.rep
local string_sub = string.sub
local string_gsub = string.gsub
local string_find = string.find
local table_insert = table.insert
local debug_traceback = debug.traceback
local table_sort = table.sort

local tostring = tostring
local type = type
local pairs = pairs
local xpcall = xpcall
local tblunpack = table.unpack
local logger = G.api.Log

local Log = {}
Log.print = {}
local cprint = Log.print

function Log.trace(fmt, ...)
	logger.Trace(fmt, ...)
end
function Log.info(fmt, ...)
	logger.Info(fmt, ...)
end
function Log.debug(fmt, ...)
	logger.Debug(fmt, ...)
end
function Log.warn(fmt, ...) 
	fmt = fmt .. debug_traceback("")
	logger.Warn(fmt, ...)
end
function Log.error(fmt, ...) 
	fmt = fmt .. debug_traceback("")
	logger.Error(fmt, ...) 
end

function Log.table(tbl, ...)
	local fmt = cprint.table(tbl, ...)
	logger.Debug(fmt)
end

--[[
	cprint
]]

function cprint.trace(...)
	local str = string_format(...)
	if str == nil then return end
	Log.trace(str)
end

function cprint.info(...)
	local str = string_format(...)
	if str == nil then return end
	Log.info(str)
end
function cprint.debug(...)
	local str = string_format(...)
	if str == nil then return end
	Log.debug(str)
end

function cprint.warn(...)
	local str = string_format(...)
	if str == nil then return end
	Log.warn(debug_traceback(tostring(str)))
end

function cprint.error(...)
	local str = string_format(...)
	if str == nil then return end
	Log.error(debug_traceback(str))
end

local function tracebackerror(errmsg)
    cprint.error(debug_traceback(tostring(errmsg)))
end

function cprint.xpcall(func, ...)
	return xpcall(func, tracebackerror, ...)
end

function cprint.xpcall2(fun1, arges1, fun2, arges2)
	local t1, t2 = xpcall(function ( ... )
			if arges1 then
				return fun1(tblunpack(arges1))
			else
				return fun1()
			end
		end,
		function ( ... )
			tracebackerror(...)
			if fun2 then
				if arges2 then
					return fun2(tblunpack(arges2))
				else
					return fun2()
				end
			end
		end)
	return t1,t2
end


--[[
打印一个table
@value [table] 必须参数  
@desciption [string] [非必要参数] 描述标记 
@nesting [number] [非必要参数] table最大嵌套 若不设置 默认为3层

example:
	local testLog = {a=1,b=2,c={c1="3",c2="4",c3={c31=5,c32={6,7}}}}
	cprint.table(testLog,"测试",2)--只打印到第二层
	
	打印结果:
	 dump from: ...Init.lua:229: in main chunk
	 - "测试" = {
	 -     "a" = 1
	 -     "b" = 2
	 -     "c" = {
	 -         "c1" = "3"
	 -         "c2" = "4"
	 -         "c3" = *MAX NESTING*
	 -     }
	 - }
]]--
function cprint.table(value, desciption, nesting)
	if type(nesting) ~= "number" then nesting = 3 end
	
	local lookup = {}
	local result = {}

	local function trim(input)
		input = string_gsub(input, "^[ \t\n\r]+", "")
		return string_gsub(input, "[ \t\n\r]+$", "")
	end
	local function split(input, delimiter)
		input = tostring(input)
		delimiter = tostring(delimiter)
		if (delimiter=='') then return false end
		local pos,arr = 0, {}
		for st,sp in function() return string_find(input, delimiter, pos, true) end do
			table_insert(arr, string_sub(input, pos, st - 1))
			pos = sp + 1
		end
		table_insert(arr, string_sub(input, pos))
		return arr
	end
	local traceback = split(debug_traceback("", 2), "\n")
	local tLen = #traceback
	local fromStr = trim(traceback[3].."\n")
	if tLen > 3 then
		fromStr = trim(traceback[4].."\n")
	end
	local logStr = "dump from: " .. fromStr
	local function _dump_value(v)
		if type(v) == "string" then
			v = "\"" .. v .. "\""
		end
		return tostring(v)
	end
	local function _dump(value, desciption, indent, nest, keylen)
		desciption = desciption or "<var>"
		local spc = ""
		if type(keylen) == "number" then
			spc = string_rep(" ", keylen - string_len(_dump_value(desciption)))
		end
		if type(value) ~= "table" then
			result[#result +1 ] = string_format("%s%s%s = %s", indent, _dump_value(desciption), spc, _dump_value(value))
		elseif lookup[tostring(value)] then
			result[#result +1 ] = string_format("%s%s%s = *REF*", indent, _dump_value(desciption), spc)
		else
			lookup[tostring(value)] = true
			if nest > nesting then
				result[#result +1 ] = string_format("%s%s = *MAX NESTING*", indent, _dump_value(desciption))
			else
				result[#result +1 ] = string_format("%s%s = {", indent, _dump_value(desciption))
				local indent2 = indent.."    "
				local keys = {}
				local keylen = 0
				local values = {}
				for k, v in pairs(value) do
					keys[#keys + 1] = k
					local vk = _dump_value(k)
					local vkl = string_len(vk)
					if vkl > keylen then keylen = vkl end
					values[k] = v
				end
				table_sort(keys, function(a, b)
					if type(a) == "number" and type(b) == "number" then
						return a < b
					else
						return tostring(a) < tostring(b)
					end
				end)
				for i = 1, #keys do
					local k = keys[i]
					_dump(values[k], k, indent2, nest + 1, keylen)
				end
				result[#result +1] = string_format("%s}", indent)
			end
		end
	end
	_dump(value, desciption, "", 1)
		
	local maxLine = #result
	for i = 1, maxLine do
		logStr = logStr .."\n" .. result[i]
		if string.len(logStr) > 13000 or i == maxLine then
			print(logStr)
			logStr = ""
		end
	end
	return result
end

-- 返回
return Log
