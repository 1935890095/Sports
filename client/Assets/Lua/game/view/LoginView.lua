local View = require("game.view.LogicView")
local ViewCtrl = require("game.ctrl.LoginCtrl")
local Res = require("game.ui.res.LoginView")

local LoginView = View.class(Res, ViewCtrl)


function LoginView:ctor(...)
    View.ctor(self, ...)
end

function LoginView:onCreate()
    self:Button(self.view.enter, View.API.Button.onClick, G.bind(self, onLogin))
end

function LoginView:onEnabled()
end

--登录
onLogin = function(self)
    local account = self:InputField(self.view.account, View.API.InputField.text)
    local password = self:InputField(self.view.password, View.API.InputField.text)
    if string.len(account)<= 0 or string.len(password)<= 0 then
        return
    end
    local option = {
        Account = account, 
        Password = password,
        Version = G.unity.Application.version,
        DeviceId = "",
        Os = G.unity.Os,
        OsVersion = G.unity.OsVersion
    }
    self.ctrl:login(option)
end

return LoginView
