--[[
  Id: String.lua
  Desc: 对string不支持的功能执行扩展
  Author: 
  Date: 2020-04-21 14:29:21
  From: https://github.com/smilehao/xlua-framework/blob/master/Assets/LuaScripts/Common/StringUtil.lua
--]]

local unpack = unpack or table.unpack

-- 字符串分割
-- @split_string：被分割的字符串
-- @pattern：分隔符，可以为模式匹配
-- @init：起始位置
-- @plain：为true禁用pattern模式匹配；为false则开启模式匹配
local function split(split_string, pattern, search_pos_begin, plain)
	assert(type(split_string) == "string")
	assert(type(pattern) == "string" and #pattern > 0)
	search_pos_begin = search_pos_begin or 1
	plain = plain or true
	local split_result = {}

	while true do
		local find_pos_begin, find_pos_end = string.find(split_string, pattern, search_pos_begin, plain)
		if not find_pos_begin then
			break
		end
		local cur_str = ""
		if find_pos_begin > search_pos_begin then
			cur_str = string.sub(split_string, search_pos_begin, find_pos_begin - 1)
		end
		split_result[#split_result + 1] = cur_str
		search_pos_begin = find_pos_end + 1
	end

	if search_pos_begin <= string.len(split_string) then
		split_result[#split_result + 1] = string.sub(split_string, search_pos_begin)
	else
		split_result[#split_result + 1] = ""
	end

	return split_result
end

-- 字符串连接
function join(join_table, joiner)
	if #join_table == 0 then
		return ""
	end

	local fmt = "%s"
	for i = 2, #join_table do
		fmt = fmt .. joiner .. "%s"
	end

	return string.format(fmt, unpack(join_table))
end

-- 是否包含
-- 注意：plain为true时，关闭模式匹配机制，此时函数仅做直接的 “查找子串”的操作
function contains(target_string, pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, pattern, 1, plain)
	return find_pos_begin ~= nil
end

-- 以某个字符串开始
function startswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, 1, plain)
	return find_pos_begin == 1
end

-- 以某个字符串结尾
function endswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, -#start_pattern, plain)
	return find_pos_end == #target_string
end

function subStringUTF8(str, startIndex, endIndex)
    if startIndex < 0 then
        startIndex = subStringGetTotalIndex(str) + startIndex + 1;
    end

    if endIndex ~= nil and endIndex < 0 then
        endIndex = subStringGetTotalIndex(str) + endIndex + 1;
    end

    if endIndex == nil then 
        return string.sub(str, subStringGetTrueIndex(str, startIndex));
    else
        return string.sub(str, subStringGetTrueIndex(str, startIndex), subStringGetTrueIndex(str, endIndex + 1) - 1);
    end
end

--获取中英混合UTF8字符串的真实字符数量
function subStringGetTotalIndex(str)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = subStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(lastCount == 0);
    return curIndex - 1;
end

function subStringGetTrueIndex(str, index)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = subStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(curIndex >= index);
    return i - lastCount;
end

--返回当前字符实际占用的字符数
function subStringGetByteCount(str, index)
    local curByte = string.byte(str, index)
    local byteCount = 1;
    if curByte == nil then
        byteCount = 0
    elseif curByte > 0 and curByte <= 127 then
        byteCount = 1
    elseif curByte>=192 and curByte<=223 then
        byteCount = 2
    elseif curByte>=224 and curByte<=239 then
        byteCount = 3
    elseif curByte>=240 and curByte<=247 then
        byteCount = 4
    end
    return byteCount;
end

-- 污染到全局
string.split = split
string.join = join
string.contains = contains
string.startswith = startswith
string.endswith = endswith
string.sub2 = subStringUTF8

return {
	split = split,
	join = join,
	contains = contains,
	startswith = startswith,
	endswith = endswith,
    subStringGetTotalIndex = subStringGetTotalIndex,
    subStringUTF8 = subStringUTF8,
}