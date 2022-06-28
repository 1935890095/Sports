--[[
  Id: Event.lua
  Desc: 游戏内逻加事件定义
--]]
local Event = {}

-- 游戏状态
Event.State = {
    CHANGE = "state:change", -- 切换状态
    ENTER_LOADING = "state:enterloading",
    LEAVE_LOADING = "state:leaveloading",
    ENTER_LOGIN = "state:enterlogin",
    LEAVE_LOGIN = "state:leavelogin",
    ENTER_GAME = "state:entergame", -- 进入游戏
    LEAVE_GAME = "state:leavegame", -- 离开游戏
    ENTER_EDITOR = "state:entereditor",
    LEAVE_EDITOR = "state:leaveeditor",
    ENTER_UPDATE = "state:enterupdate",
    LEAVE_UPDATE = "state:leaveupdate"
}

Event.Net = {
    ON_CONNECTED = "net:onconnected", -- 连接成功
    ON_CONNECT_FAILED = "net:onconnectfailed", -- 连接失败
    ON_DISCONNECTED = "net:ondisconnected", -- 断开连接
    ON_GETDELAYTIME = "net:ongetdelaytime", -- 获取延迟时间
}

Event.App = {
    QUIT = "app:quit"
}

-- View类事件
Event.View = {
    OPEN = "view:open", -- 打开
    CHECKOPEN = "view:checkopen", -- 检查后再打开
    CLOSEOVER = "view:closeover", -- 关闭
    CLOSE = "view:close", -- 关闭
    CHECK = "view:check" -- 检查
}

--统一处理
Event.Unite = {
    ENTER_GAME = "unite:entergame",       --进入游戏
}

Event.Time = {
    DAY_CHANGE = "time:daychange"
}

Event.Input = {
    UI_INPUT = "input:uiinput",
    INPUT = "input:input"
}

return Event