
local base = require("core.Control")
local UpdateCtrl = class(base)

local enter
local leave
local startUpdate
local clearTimer

local onTimer
local onFakeProgress

-- real update
local onComplete
local onProgress
local onError
local onDelayHandler

local checkForceUpdate
local checkResUpdate

local M = 1024 * 1024

local ErrorCode = {
    SUCCESS = 0,
    CHECK_FAILED = 1, -- 检测版本失败
    INCOMPATIBLE = 2, -- 版本兼容
    FILE_ERROR = 3 -- 下载文件错误
}

function UpdateCtrl:ctor(...)
    base.ctor(self, ...)
    self.hasError = false
    self.hasEnter = false
    self.updating = false
    self.timer = nil
    self.time = 0
    self.beginUpdate = false
end

function UpdateCtrl:init()
    self.router:on(G.game.event.State.ENTER_UPDATE, G.bind(self, enter))
    self.router:on(G.game.event.State.LEAVE_UPDATE, G.bind(self, leave))
end

function UpdateCtrl:onOpen(_)
    -- hide splash in android
    -- 当前帧UI可能没有显示出来
    self.hideTimer =
        G.timer.new(
        function()
            self.hideTimer = nil
            G.api.Sdk.Invoke("splash", "hideSplash")
            self.router:route(G.game.event.Sdk.Report.HideSplash)
        end,
        0.1,
        1
    )
    self.hideTimer:start()
    self:notify("showProgress", false)
    checkForceUpdate(self)
    -- checkResUpdate(self)
end

enter = function(self)
    self.router:route(G.game.event.View.OPEN, "LaunchView")
end

leave = function(self)
    clearTimer(self)
end

startUpdate = function(self)
    self.hasError = false
    G.api.Update.Begin(G.config.updateUrl, G.config.version, "", G.bind(self, onComplete), G.bind(self, onProgress), G.bind(self, onError))
    self.timer = G.timer.new(G.bind(self, onTimer), 0, 0)
    self.timer:start()
end

clearTimer = function(self)
    if self.timer then
        self.timer:stop()
        self.timer = nil
    end
end

onTimer = function(self)
    if self.hasError then
        return
    end
    G.api.Update.Run()
end

onFakeProgress = function(self, delta)
    delta = (not delta) and G.C.deltaTime or delta

    self.time = self.time + delta
    local progress = self.time / G.game.const.FAKE_LOADING_DURATION
    progress = math.min(progress, 1)
    self:notify("progress", progress)
    if self.time >= G.game.const.FAKE_LOADING_DURATION then
        clearTimer(self)
        G.game:startGame()
    end
end

onComplete = function(self, updated)
    print("*************************************** lua onComplete " .. tostring(updated))
    clearTimer(self)
    self.router:route(G.game.event.Sdk.Report.CheckUpdateSuccess)
    G.api.Update.End()
    if updated then
        self.router:route(G.game.event.Sdk.Report.EndUpdate)
        G.api.Reload()
    else
        self.timer = G.timer.new(G.bind(self, onFakeProgress), 0, 0)
        self.timer:start()
        self:notify("showProgress", true)
    end
end

onProgress = function(self, progress)
    if not self.beginUpdate then
        self.beginUpdate = true
    end
    self.updating = true
    self:notify("progress", progress)
    local downloadSize = G.api.Update.Get("downloadSize")
    local totalSize = G.api.Update.Get("totalSize")
    local downloadSpeed = G.api.Update.Get("downloadSpeed")
    totalSize = totalSize / M
    downloadSize = downloadSize / M
    downloadSpeed = downloadSpeed / M
    local content = string.format(G.game.localization:get("GameUpdateSpeed"), downloadSize, totalSize)
    self:notify("speed", content)
end

onError = function(self, error, message)
    G.log.warn("update error {0} {1}", error, message)
    self.hasError = true
    local data = {
        title = "CommonTips_Hint",
        content = "GameUpdate1",
        left = {
            text = "CommonTips_Exit",
            handler = function()
                G.api.Quit()
            end
        },
        right = {
            text = "CommonTips_Retry"
        },
        close = {
            hide = true
        },
        fullclose = {
            hide = true
        }
    }
    if error == ErrorCode.CHECK_FAILED then
        data.content = "GameUpdate1"
        data.right.handler = function()
            onDelayHandler(
                self,
                function()
                    self.hasError = false
                    G.api.Update.Continue()
                    -- startUpdate(self)
                end
            )
        end
        self.router:route(G.game.event.Sdk.Report.CheckUpdateFail)
    elseif error == ErrorCode.INCOMPATIBLE then
        data.content = "GameUpdate2"
        data.right.handler = function()
            G.api.Quit()
        end
    elseif error == ErrorCode.FILE_ERROR then
        data.content = "GameUpdate3"
        data.right.handler = function()
            onDelayHandler(
                self,
                function()
                    self.hasError = false
                    G.api.Update.Continue()
                end
            )
        end
    end
    self.router:route(G.game.event.Msg.SHOW, data)
end

onDelayHandler = function(self, func)
    if self.delayTimer then
        self.delayTimer:stop()
        self.delayTimer = nil
    end
    self.delayTimer = G.timer.new(func, 0.5, 1)
    self.delayTimer:start()
end

checkForceUpdate = function(self)
    local delayContinue = function()
        local data = {
            title = "CommonTips_Hint",
            content = "GameUpdate1",
            left = {
                text = "CommonTips_Exit",
                handler = function()
                    G.api.Quit()
                end
            },
            right = {
                text = "CommonTips_Retry",
                handler = function()
                    checkForceUpdate(self)
                end
            },
            close = {
                hide = true
            },
            fullclose = {
                hide = true
            }
        }
        self.router:route(G.game.event.Msg.SHOW, data)
    end

    local url = G.config.api .. "check_update"
    local body = {
        version = G.unity.Application.version,
        channel = G.game:getChannelId()
    }

    G.game.http:post(
        url,
        body,
        function(data)
            if data.Status == 0 then
                -- 无需强更
                checkResUpdate(self)
            elseif data.Status == 1 and data.Url and data.Url ~= "" then
                local msgData = {
                    title = "CommonTips_Hint",
                    content = "GameUpdate2",
                    left = {
                        text = "CommonTips_Ok",
                        handler = function()
                            G.unity.Application.openURL(data.Url)
                            G.api.Quit()
                        end
                    },
                    close = {
                        hide = true
                    },
                    fullclose = {
                        hide = true
                    }
                }
                self.router:route(G.game.event.Msg.SHOW, msgData)
            else
                -- 检查失败
                onDelayHandler(self, delayContinue())
            end
        end,
        function(err)
            G.log.error("check force update {0} error {1}", url, err)
            onDelayHandler(self, delayContinue)
        end
    )
end

checkResUpdate = function(self)
    startUpdate(self)
end

return UpdateCtrl
