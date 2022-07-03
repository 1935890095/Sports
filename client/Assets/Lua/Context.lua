--[[
  @Desc Lua全局上下文
  @Author: xfx
--]]
local json = require("dkjson")
local Context = {}
Context.modules = {}

local profile = false

-- 垃圾收集的时间间隔
local MAX_GC_TICK = 700
-- GC帧计数
local gc_tick = 1
-- GC的步数
local gc_step = 0
-- 是否进行GC的标记
local gc_flag = false

-- make sure able to get context in cs
_G.Context = Context

function Context:load()
    -- for short
    local cscc = require("core.Class")
    _G.class = cscc.class
    _G.struct = cscc.struct
    _G.clone = cscc.clone
    _G.G = {}
    _G.G.class = cscc.class
    _G.G.struct = cscc.struct
    _G.G.const = cscc.const
    _G.G.clone = cscc.clone
    _G.G.C = Context
    _G.Context = nil -- disable accesss context from cs

    -- json
    G.json = json

    -- lua api
    G.api = CS.XFX.Game.LuaApi

    -- util
    G.common = require("util.Common")
    G.unity = require("util.Unity")
    G.log = require("util.Log")
    G.print = G.log.print
    G.bind = require("util.Bind")
    G.timer = require("util.Timer")
    G.sequence = require("util.Sequence")
    G.string = require("util.String")
    G.config = require("resource.config.game")
    G.config.version = self:version()

    -- 先初始化一次
    local args = {G.unity.Time.deltaTime, G.unity.Time.unscaledDeltaTime, G.unity.Time.realtimeSinceStartup}
    self.deltaTime, self.unscaledDeltaTime, self.realtimeSinceStartup = args[1], args[2], args[3]

    -- game
    G.game = require("game.Game")
    G.game:start()

    if profile then
        G.profiler = require("util.perf.Profiler")
        G.profiler.start()
    end

    print("* context loaded")
end

local function gc(self)
    if self.deltaTime < 0.07 then
        if gc_flag == false then
            if gc_tick > MAX_GC_TICK then
                --GC标记，在循环中分帧分步进行收集
                gc_flag = true
                gc_tick = 1
            end
            gc_tick = gc_tick + 1
        end

        -- 分步进行垃圾收集
        if gc_flag then
            gc_step = gc_step + 1
            if collectgarbage("step", 20) then
                --log.debug(">>>>>>> collectgarbage done total, step %d", gc_step)
                gc_step = 0
                gc_flag = false
            end
        end
    end
end

local print_profiler_interval = 30
local print_profiler_time = 0

-- 驱动
function Context:loop(...)
    local args = {...}
    if #args == 0 then
        args = {G.unity.Time.deltaTime, G.unity.Time.unscaledDeltaTime, G.unity.Time.realtimeSinceStartup}
    end
    self.deltaTime, self.unscaledDeltaTime, self.realtimeSinceStartup = args[1], args[2], args[3]
    if nil == self.totalDeltaTime then
        self.totalDeltaTime = 0
    end
    self.totalDeltaTime = self.totalDeltaTime + self.deltaTime

    G.timer.process(self.deltaTime, self.unscaledDeltaTime)
    gc(self)

    if profile then
        print_profiler_time = print_profiler_time + self.unscaledDeltaTime
        if print_profiler_time >= print_profiler_interval then
            print_profiler_time = 0
            local info = G.profiler.report("AVERAGE")
            G.log.error(info)
        end
    end
end

function Context:unload()
    local dummy = nil
    dummy = G.game and G.game:stop()
    print("* context unloaded")
end

function Context:invoke(type, id, func, ...)
    if debug then
        -- elapse = G.unity.Time.realtimeSinceStartup - elapse
        -- local low = (1/(G.game.rate - 5)) * 0.5
        -- if elapse > low then
        --     elapse = elapse * 1000;
        --     elapse = elapse - elapse % 0.1
        --     local params = ""
        --     for i = 1, select('#', ...) do
        --         params = params .. tostring(select(i, ...)) .. ","
        --     end
        --     G.log.error("low invoke({0}ms): {1}(type) {2}(id) {3}(func) {4}", elapse, type, id, func, params)
        -- end
        -- G.log.info("invoke type: {0} id: {1}, func: {2}", type, id, func)
        local elapse = G.unity.Time.realtimeSinceStartup
        G.game:invoke(type, id, func, ...)
    else
        G.game:invoke(type, id, func, ...)
    end
end

function Context:print_func_ref_by_csharp()
    local registry = debug.getregistry()
    for k, v in pairs(registry) do
        if type(k) == "number" and type(v) == "function" and registry[v] == k then
            local info = debug.getinfo(v)
            print(string.format("%s:%d", info.short_src, info.linedefined))
        end
    end
end

function Context:version()
    local version = nil
    local asset = G.unity.Resources.Load("version")
    if asset then
        version = asset.text
        G.unity.Resources.UnloadAsset(asset)
    end

    local cacheVersion = G.api.Cache.Get("version.manifest")

    if version and not cacheVersion then
        return version
    end

    if cacheVersion and not version then
        return cacheVersion
    end

    local versions = string.split(version, ".")
    local versionNum = tonumber(versions[1]) * 10000 + tonumber(versions[2]) * 10 + tonumber(versions[3])

    local cacheVersions = string.split(cacheVersion, ".")
    local cacheVersionNum = tonumber(cacheVersions[1]) * 10000 + tonumber(cacheVersions[2]) * 10 + tonumber(cacheVersions[3])

    if versionNum > cacheVersionNum then
        return version
    else
        return cacheVersion
    end
end

return Context
