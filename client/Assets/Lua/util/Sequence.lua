--[[
  Author: nzh
  Desc: 时间序列定时器
  Date: 2021-06-08 10:33:59
--]]
local Sequence = class()

local onUpdate

function Sequence:ctor()
    self.timer = nil
    self.time = 0
    self.sequences = {}
end

-- 添加一个序列
-- @duration 持续时间，必须大于0
-- @callback 回调函数，为nil时该序列只占用序列时间
-- @invokeIfStop 主动调用stop时，如果该序列未执行或者未结束时是否调用回调函数
function Sequence:add(duration, callback, invokeIfStop)
    if type(duration) ~= "number" or duration <= 0 then
        G.log.error("Sequence add duration error")
        return
    end

    if callback ~= nil and type(callback) ~= "function" then
        G.log.error("Sequence add callback error")
        return
    end
    table.insert(self.sequences, {duration = duration, callback = callback, invokeIfStop = invokeIfStop and true or false})
end

function Sequence:start()
    if self.timer then
        G.log.error("Sequence has started")
        return
    end

    if #self.sequences <= 0 then
        G.log.error("Sequence start error")
        return
    end

    self.time = self.sequences[1].duration
    self.timer = G.timer.new(G.bind(self, onUpdate), 0, 0)
    self.timer:start()
end

function Sequence:stop()
    if not self.timer then
        return
    end

    self.timer:stop()
    self.timer = nil

    for _, sequence in ipairs(self.sequences) do
        if sequence.invokeIfStop and sequence.callback ~= nil then
            sequence.callback()
        end
    end

    self.time = 0
    self.sequences = nil
end

onUpdate = function(self)
    self.time = self.time - G.C.deltaTime
    if self.time <= 0 then
        local sequence = table.remove(self.sequences, 1)
        if sequence.callback ~= nil then
            sequence.callback()
        end

        if #self.sequences <= 0 then
            self.timer:stop()
            self.timer = nil
            self.time = 0
            self.sequences = nil
        else
            self.time = self.sequences[1].duration
        end
    end
end

return Sequence
