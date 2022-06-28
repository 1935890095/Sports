local base = require("core.Control")
local stringUtil = require("util.String")

local LoadingCtrl = class(base)

local onEnter

function LoadingCtrl:ctor(...)
    base.ctor(self, ...)
    self.progress = 0
    self.complete = 0
end

function LoadingCtrl:init()
    self.router:on(G.game.event.State.ENTER_LOADING, G.bind(self, onEnter))
end

function LoadingCtrl: onCompleteFunc()
    G.game:startGame()
    self.router:route(G.game.event.View.CLOSE, "LoadingView")
end

--加载资源
onEnter = function(self)
    self.router:route(G.game.event.View.OPEN, "LoadingView")
    self.router:route(G.game.event.View.CLOSE, "LaunchView")
    local url = string.format(G.config.consul, G.config.server).."client?keys=true"
    G.game.http:get(
        url,
        function(configs)
            self.progress = #configs - 1
            for i,_ in pairs(configs) do
                local keys = stringUtil.split(configs[i], "client/")
                if #keys > 1 and string.len(keys[2]) > 0 then
                    local conurl = string.format(G.config.consul.."client/%s?raw=true", G.config.server, keys[2])
                    local names = stringUtil.split(keys[2], "/")
                    G.game.http:get(
                        conurl,
                        function(config)
                            G.game.config:Save(names[#names], config)
                            G.log.warn(string.format("*************************load %s config success", names[#names]))

                            self.complete = self.complete + 1
                            self:notify("complete", self.complete / self.progress)
                        end,
                        function(error)
                            G.log.warn(string.format("load %s config faild, reason is %s", names[names.length], error))
                        end
                    )
                end
            end
        end,
        function(error)
            G.log.warn(string.format("http request %s config faild, reason is %s", url, error))
        end
    )
end

return LoadingCtrl
