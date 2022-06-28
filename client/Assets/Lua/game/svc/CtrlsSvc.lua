--[[
  Id: CtrsSvc.lua
  Desc: 视图控制器服务
  Author: figo
  Date: 2020-04-23 19:45:02
--]]
local CtrlsSvc = class()

-- forward declares
local initCtrls

function CtrlsSvc:ctor(game)
    self.game = game
    self.router = game.router
    self.ctrls = {}
    initCtrls(self, G.game.registry.preloadCtrls)
end

function CtrlsSvc:get(name)
    return self.ctrls[name]
end

function CtrlsSvc:initGameCtrls()
    initCtrls(self, G.game.registry.ctrls)
end

initCtrls = function(self, ctrls)
    for name, cls in pairs(ctrls) do
        cls.name = name --（重要）创建视图时，要通名称去找
        self.ctrls[name] = cls.new(self.router)
    end

    self.day = G.game.time:getDays() -- os.date("%d")
    if not self.timer then
        self.timer =
            G.timer.new(
            function(...)
                local day = G.game.time:getDays() -- os.date("%d")
                if day ~= self.day then
                    self.day = day
                    self.router:route(G.game.event.Time.DAY_CHANGE)
                    G.log.warn("****** day changed to {0}", self.day)
                end
            end,
            1,
            -1,
            true
        )
        self.timer:start()
    end
end

return CtrlsSvc
