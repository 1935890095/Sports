local View = require("game.view.LogicView")
local ViewCtrl = require("game.ctrl.LoadingCtrl")
local Res = require("game.ui.res.LoadingView")

local LoadingView = View.class(Res, ViewCtrl)


function LoadingView:ctor(...)
    View.ctor(self, ...)
end

function LoadingView:onCreate()
    self:on("complete", G.bind(self, onProgress))
end

function LoadingView:onEnabled()
    self:Slider(self.view.pregress_bg, View.API.Slider.value, (self.ctrl.complete / self.ctrl.progress) or 0)
end

onProgress = function(self, value)
    self:Slider(self.view.pregress_bg, View.API.Slider.value, value)

    if value == 1 then
        self.ctrl:onCompleteFunc()
    end
end

return LoadingView
