--[[
    协议名称、消息号定义
--]]
local ProtoDefine = {
    Id = {
        Ping = 10000,
        Pong = 10001,

        LoginRequest = 10002,
        LoginResult = 10003
    },
    Name = {
        [10000] = "Proto.Login.C2SPing",
        [10001] = "Proto.Login.S2CPong",

        [10002] = "Proto.Login.C2SLoginRequest",
        [10003] = "Proto.Login.S2CLoginResult"
    } 
}
return ProtoDefine
