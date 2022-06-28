--[[
	desc:语言控制
    time:2020-07-17 10:19:42
--]]
local Language = class()

local change

function Language:ctor(game)
    self.language = G.game.config.settings.lang

    self.router = game.router
end

function Language:current()
    return self.language
end

change = function(self, lang)
    if lang == self.language then
        return
    end

    self.language = lang
    G.game.localization:change(self.language)
    self.router:route(G.game.event.Language.LANGUAGE_CHANGED, self.language)

    G.game.config.settings.lang = self.language
    G.game.config:saveSettings()
end

return Language
