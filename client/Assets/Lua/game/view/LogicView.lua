--[[
    desc: 视图业务层接口封装:多语言、红点等
    time:2020-09-18 17:29:57
]]
local unpack = unpack or table.unpack

local base = require("core.View")
local LogicView = class(base)

LogicView.API = require("game.view.ViewApi")


local LangNodeType = {
    PreText = 1,
    PreSprite = 2,
    PreTexture = 3,
    Text = 4,
    Sprite = 5,
    Texture = 6
}
LogicView.LangNodeType = LangNodeType

local LangAtlas = {
    Art = "art"
}
LogicView.LangAtlas = LangAtlas

local getLangNodes
local predetermineLanguage
local refreshText
local refreshSprite
local refreshTexture
local clearLanguage
local refreshRedDot
local destroyRedDot
local showPopupAnim
local checkShowCurrency
local adaptiveView
local onSortingOrder
local getResTab = {}

-- helper function to define subclass
-- @res  the view res code
-- @ctrl the control of view
function LogicView.class(res, ctrl)
    local cls = class(LogicView)
    cls.__res = res
    cls.__ctrl = ctrl
    cls.name = res.__name
    return cls
end

-- load view res, called as soon as prefab loaded.
function LogicView:load()
    base.load(self)
    local view = self

    local onCreate = self.onCreate
    self.onCreate = function()
        onCreate(view)
    end

    local onEnabled = self.onEnabled
    self.onEnabled = function()
        onEnabled(view)
        -- view:refreshLanguage()
    end

    local onDisable = self.onDisable
    self.onDisable = function()
        onDisable(view)
        -- clearLanguage(view) -- 清理业务逻辑刷新的多语言节点，避免下次onEnabled时不必要的开销
    end

    local onDestroy = self.onDestroy
    self.onDestroy = function()
        onDestroy(view)
    end
end

--@region<API>
function LogicView:text(id, key, ...)
    local args = {...}
    refreshText(self, id, key, args)
    local nodes = getLangNodes(self, LangNodeType.Text)
    nodes[id] = {key, args}
end

function LogicView:sprite(id, atlas, sprite)
    refreshSprite(self, id, atlas, sprite)
    local nodes = getLangNodes(self, LangNodeType.Sprite)
    nodes[id] = {atlas, sprite}
end

function LogicView:texture(id, tex)
    tex = string.lower(tex)
    refreshTexture(self, id, tex)
    local nodes = getLangNodes(self, LangNodeType.Texture)
    nodes[id] = tex
end

function LogicView:loadtexture(id, assetPath, assetName)
    local path = string.format(assetPath, assetName)
    self:RawImage(id, "asset", G.api.Asset.CreateTexture(path))
end

-- 播放音效
-- loop:循环次数，小于等于0无限循环，nil为1次
-- cb:播放完成的回调，特殊情况可能用到，可空
function LogicView:playAudio(filename, loop, cb)
    G.game.audios:playAudio(filename, loop, cb)
end

-- 播放音乐
-- loop:循环次数，小于等于0无限循环，nil为1次
-- cb:播放完成的回调，特殊情况可能用到，可空
function LogicView:playMusic(filename, loop, cb)
    -- 暂时屏蔽
    -- G.game.audios:playMusic(filename, loop, cb)
end

-- 播放bgm，会一直有一个bgm在播放，播放新的顶掉老的
function LogicView:playBGM(filename)
    G.game.audios:playBGM(filename)
end

-- 关闭音效，点击按钮跳转页面，关闭正在播放的音效
function LogicView:stopAudio()
    G.game.audios:stopAudio()
end

-- 由ViewSevc和onEnabled驱动
function LogicView:refreshLanguage()
    if self.langNodes then
        for t, nodes in pairs(self.langNodes) do
            if t == LangNodeType.PreText or t == LangNodeType.Text then
                for id, data in pairs(nodes) do
                    refreshText(self, id, data[1], data[2])
                end
            elseif t == LangNodeType.PreSprite or t == LangNodeType.Sprite then
                for id, data in pairs(nodes) do
                    refreshSprite(self, id, data[1], data[2])
                end
            else
                for id, data in pairs(nodes) do
                    refreshTexture(self, id, data)
                end
            end
        end
    end
end

predetermineLanguage = function(self)
    if self.preLanguages then
        for t, languages in pairs(self.preLanguages) do
            local nodes = getLangNodes(self, t)
            for id, data in pairs(languages) do
                nodes[id] = data
            end
        end
    end
end

getLangNodes = function(self, t)
    if not self.langNodes then
        self.langNodes = {}
    end
    local nodes = self.langNodes[t]
    if not nodes then
        nodes = {}
        self.langNodes[t] = nodes
    end
    return nodes
end

refreshText = function(self, id, key, args)
    if args and #args > 0 then
        local haveFn = false
        for _, arg in ipairs(args) do
            if type(arg) == "function" then
                haveFn = true
                break
            end
        end
        if haveFn then
            local tmp = args
            args = {}
            for i, arg in ipairs(tmp) do
                if type(arg) == "function" then
                    args[i] = arg()
                else
                    args[i] = arg
                end
            end
        end

        self:Text(id, "text", G.game.localization:get(key, unpack(args)))
    else
        self:Text(id, "text", G.game.localization:get(key))
    end
end

refreshSprite = function(self, id, atlas, sprite)
    local ext = G.game.language:current() or ""
    self:Image(id, "sprite", atlas .. ext, sprite, true)
end

refreshTexture = function(self, id, tex)
    local lang = G.game.language:current() or ""
    self:RawImage(id, "asset", G.api.Asset.CreateTexture("res/ui/tex/" .. tex .. lang .. ".tex"))
    -- self:Graphic(id, "SetNativeSize")
    local configLine = G.game.config.texturesize[tex..lang]
    if nil ~= configLine then
        self:RectTransform(id, self.API.RectTransform.sizeDelta, G.unity.Vector2(configLine.width, configLine.height))
    else
        G.log.error("can not find {0} texture size, please rebuild texturesize config", tex..lang)
    end

end

clearLanguage = function(self)
    if self.langNodes then
        self.langNodes[LangNodeType.Text] = nil
        self.langNodes[LangNodeType.Sprite] = nil
        self.langNodes[LangNodeType.Texture] = nil
    end
end

return LogicView
