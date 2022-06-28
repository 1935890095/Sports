--[[
  Author: nzh
  Desc: vector3定义
  Date: 2020-11-05 20:40:51
--]]
local Vector3 = class()

function Vector3:ctor(x, y, z)
    self.x = 0
    self.y = 0
    self.z = 0
    self:set(x, y, z)
end

function Vector3:set(x, y, z)
    if type(x) == "table" or type(x) == "userdata" then
        self.x = x.x or 0
        self.y = x.y or 0
        self.z = x.z or 0
    else
        self.x = x or 0
        self.y = y or 0
        self.z = z or 0
    end
end

function Vector3:toUnity()
    return G.unity.Vector3(self.x, self.y, self.z)
end

function Vector3.__add(v1, v2)
    return Vector3.new(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z)
end

function Vector3.__sub(v1, v2)
    return Vector3.new(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z)
end

function Vector3.__mul(v1, v2)
    if type(v1) == "number" then
        return Vector3.new(v1 * v2.x, v1 * v2.y, v1 * v2.z)
    end

    if type(v2) == "number" then
        return Vector3.new(v1.x * v2, v1.y * v2, v1.z * v2)
    end

    return Vector3.new(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z)
end

function Vector3:magnitude()
    return math.sqrt(self:dot(self))
end

function Vector3:normalize()
    local m = self:magnitude()
    if not m or m <= 0 then
        m = 1
        G.log.warn("normalize is 0")
    end

    return Vector3.new(self.x / m, self.y / m, self.z / m)
end

function Vector3:dot(v)
    return self.x * v.x + self.y * v.y + self.z * v.z
end

function Vector3:zero()
    return Vector3.new(0, 0, 0)
end

function Vector3:forward()
    return Vector3.new(0, 0, 1)
end

function Vector3:back()
    return Vector3.new(0, 0, -1)
end

function Vector3:up()
    return Vector3.new(0, 1, 0)
end

function Vector3:signedAngle(forward, tarPos, up)
    local f = G.unity.Vector3(forward.x, forward.y, forward.z)
    local p = G.unity.Vector3(tarPos.x, tarPos.y, tarPos.z)
    local u = G.unity.Vector3(up.x, up.y, up.z)
    return G.unity.Vector3.SignedAngle(f, p, u)
end

return Vector3
