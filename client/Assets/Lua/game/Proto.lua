local pb = require("pb")
local Proto = class()

local encode
local decode

local VALID_INTERVAL = 0.5

local UNBLOCKS = {
}

function Proto:ctor(router)
    self.router = router
    pb.option("enum_as_value")
    pb.option("no_default_values")
    pb.option("int64_as_number")
    local protos = {"pb_login", "pb_player"}
    for _, value in ipairs(protos) do
        local bytes = G.api.Cache.LoadProtobuf(string.format("res/proto/%s.pb", value))
        local result = pb.load(bytes)
        if not result then
            G.log.error("===== load protobuf file '{0}' error", value)
        end
    end

    self.funcs = {}
    self.timestamps = {}
end

function Proto:reg(pid, func)
    local list = self.funcs[pid]
    if not list then
        list = {}
        self.funcs[pid] = list
    end
    table.insert(list, func)
end

function Proto:send(pid, data)
    local now = G.C.realtimeSinceStartup
    local last = self.timestamps[pid]
    if not UNBLOCKS[pid] and last and now - last <= VALID_INTERVAL then
        G.log.warn("Send message packets {0} too frequently", pid)
        return
    end

    self.timestamps[pid] = now
    local name = G.game.protodef.Name[pid]
    local msg = encode(name, data)
    G.api.Net.Send(pid, msg)
end

function Proto:fire(pid, data)
    if not data then
        G.log.error("fire data is nil")
        return
    end
    local name = G.game.protodef.Name[pid]
    if not name then
        G.log.error("can not found name by pid {0}", pid)
        return
    end
    local msg = decode(name, data)
    local list = self.funcs[pid]
    if list then
        for _, func in ipairs(list) do
            func(msg)
        end
    end
end

encode = function(name, data)
    return pb.encode(name, data)
end

decode = function(name, data)
    return pb.decode(name, data)
end

return Proto
