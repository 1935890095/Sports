local Time = class()

function Time:ctor()
    self.serverTime = os.time() * 1000
    self.lastSyncTime = G.C.realtimeSinceStartup or G.unity.Time.realtimeSinceStartup
    self.serverZoneOffset = 3 * 3600
end

function Time:onServerTime(time)
    self.serverTime = time
    self.lastSyncTime = G.C.realtimeSinceStartup
end

-- 服务器utc时间
function Time:getServerTime()
    local diff = G.C.realtimeSinceStartup - self.lastSyncTime
    local t, _ = math.modf((self.serverTime + diff * 1000) / 1000)
    return t
end

-- 服务器utc时间天数，如20201217
function Time:getDays()
    local time = self:getServerTime()
    local date = os.date("*t", time)
    return date.year * 10000 + date.month * 100 + date.day
end

-- 本地时区
function Time.getLocalTimeZone()
    local now = os.time()
    local localTimeZone = os.difftime(now, os.time(os.date("!*t", now)))
    return math.floor(localTimeZone)
end

function Time:updateServerZone(offset)
    self.serverZoneOffset = offset or self.serverZoneOffset
end

-- 服务器时区
function Time:getServerTimeZone()
    return self.serverZoneOffset
end

-- 时区修正值
function Time:getFixTime()
    return self:getLocalTimeZone() - self:getServerTimeZone()
end

function Time:getOpenTime(t)
    -- local serverDay = self:getDays()
    -- local year = serverDay // 10000
    -- local month = (serverDay - year * 10000) // 100
    -- local day = serverDay - year * 10000 - month * 100

    local s = self:getServerTime() + self:getServerTimeZone()
    s = s - s % 86400
    local _, _, _hour, _min, _sec = string.find(t, "(%d+):(%d+):(%d+)")
    return s + _hour * 3600 + _min * 60 + _sec - self:getServerTimeZone()

    -- return os.time({year=year, month = month, day = day, hour = _hour, min = _min, sec = _sec}) + self:getFixTime()
end

return Time
