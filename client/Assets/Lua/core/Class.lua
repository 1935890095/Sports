--[[
	desc: lua模拟基类
 ]]

-- new()时不会自动依次调用继承链基类的ctor,只能使用base.ctor(self, ...)
-- 访问基类的属性调用使用self.xxx,如果本类中有同名的属性会覆盖掉基类的
-- 访问基类的方法调用使用base.func(self, ...)
-- 调用self:func()可以访问基类的方法,并依继承链序往顶层找,找到后就直接返回

local clone = function(t)
    local lookup_table = {}
    local function _copy(t)
        if type(t) ~= "table" then
            return t
        elseif lookup_table[t] then
            return lookup_table[t]
        end
        local new_table = {}
        lookup_table[t] = new_table
        for key, value in pairs(t) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(t))
    end
    return _copy(t)
end

local class = function(super)
    local superType = type(super)
    local cls

    if superType ~= "function" and superType ~= "table" then
        superType = nil
        super = nil
    end

    if super then
        cls = clone(super)
        cls.super = super
    else
        cls = {ctor = function() end}
    end

    cls.__index = cls
	cls.__name = tostring(cls)

    function cls.new(...)
        local instance = setmetatable({}, cls)
        instance.class = cls
        instance:ctor(...)
        return instance
    end    

    return cls
end

local const = function(t)
    local meta = {
        __index = t,
        __newindex = function() error("immutable table") end
    }
	return setmetatable({}, meta)
end

local _class = { }
local struct = function(data)
    local classtype = {}
    classtype.ctor = false    
    classtype.new = function(...) 
        local obj = {}
        do
            local create
            create = function(c, ...)
                if c.ctor then
                    c.ctor(obj, ...)
                end
            end
            create(classtype, ...)
        end
        setmetatable(obj, {__index = _class[classtype]})
        return obj
    end
    local vtbl = data or {}
    _class[classtype] = vtbl
 
    setmetatable(classtype, {__newindex =
        function(t, k, v)
            vtbl[k] = v
        end
    })
     
    return classtype
end

return  { 
    class = class, 
    struct = struct,
    const = const, 
    clone = clone
}