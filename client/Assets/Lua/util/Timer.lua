--[[
  Id: Timer.lua
  Desc: 定时器
    使用方式：
        local Timer = require("Timer")
        ....
        self.ltimer = Timer.new(timer_process, 1)
        self.timer:start()

    注：创建一个定时器，匆必持有定时器对象以防止定时器对象未被引用而被回收，
    导致不再执行。
        
  Author: figo
  Date: 
--]]
local runnings_ = {}
setmetatable(runnings_, {__mode = "v"})

local Timer = class()

--@func:        执行函数
--@duration:    每次执行间隔时间
--@loop:        大于0时为循环的次数，小于等于0时为无限循环
--@unscaled:    false采用deltaTime计时；true采用unscaledDeltaTime计时，注意Unity在pause时unscaled会一直累加，导致resume时值会很大，所以慎用
-- @delay:      延迟时间
function Timer:ctor(func, duration, loop, unscaled, delay)
    duration = duration or 0
    unscaled = unscaled or false and true
    loop = loop or 1
    delay = delay or 0
    self.func = func
    self.delay = delay
    self.duration = duration
    self.time = duration
    self.loop = loop
    self.unscaled = unscaled
    self.running = false

    self.process = nil
    table.insert(runnings_, self)
end

function Timer:start()
    self.running = true
end

function Timer:pause()
    self.running = false
end

function Timer:stop()
    self.running = false
    for i, v in ipairs(runnings_) do
        if (v == self) then
            runnings_[i] = nil
            break
        end
    end
end

local function update(self, delta, unscaled_delta)
    if not self.running then
        return
    end

    if self.unscaled then
        delta = unscaled_delta
    end

    if self.delay > 0 then
        self.delay = self.delay - delta
        if self.delay < 0 then
            self.time = self.time + self.delay
        end
        return
    end

    self.time = self.time - delta

    if self.time <= 0 then
        G.print.xpcall(self.func)
        self.time = self.time + self.duration
        -- 大于0为循环固定次数，反之为无限循环
        if self.loop > 0 then
            self.loop = self.loop - 1
            if self.loop <= 0 then
                self:stop()
            end
        end
    end
end

--local last_n = 0
-- run timer
function Timer.process(delta, unscaled_delta)
    for _, t in pairs(runnings_) do
        if t ~= nil then
            update(t, delta, unscaled_delta)
        end
    end
end

return Timer
