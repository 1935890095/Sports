local tb_insert = table.insert
local str_format = string.format

local AudiosSvc = class()

local onSwitchChanged
local play
local update
local getName
local setvolume
local onEnter
local onLeave

local Type = {
    Music = 1,
    Audio = 2
}
local AUDIO_FORMAT = "res/audio/%s.audio"

function AudiosSvc:ctor(game)
    self.game = game
    self.router = game.router

    -- self.root = G.api.Asset.CreateRoot("[audios]", G.api.Asset.root)
    -- self.listener = G.api.Asset.CreatePrimitive3D("res/sg/audiolistener.sg", self.root)

    self.bgm = nil
    self.counter = 0
    self.audios = {}
    self.targevolume = 0

    self.timer = G.timer.new(G.bind(self, update), 0.1, 0, true)
    self.timer:start()
end

function AudiosSvc:shutdown()
    self.listener:Destroy()
    self.listener = nil
    self.root:Destroy()
    self.root = nil
end

-- 播放bgm
function AudiosSvc:playBGM(fileName)
end

--暂停BGM
function AudiosSvc:pauseBGM()
end

--继续播放BGM
function AudiosSvc:continueBGM()
end

-- 播放音乐
function AudiosSvc:playMusic(fileName, loop, cb)
end

-- 播放音效
function AudiosSvc:playAudio(fileName, loop, cb)
end

--关闭音效
function AudiosSvc:stopAudio()
    for id, audio in pairs(self.audios) do
        local asset = audio.audio
        if not asset.isPlaying then
            if audio.loop >= 0 then
                if nil == finished then
                    finished = {}
                end
                tb_insert(finished, id)
            end
        end
    end

    if finished then
        for _, id in ipairs(finished) do
            return self:stop(id)
        end
    end
end

-- 停止音乐或音效，bgm不考虑停止
function AudiosSvc:stop(id)
    local audio = self.audios[id]
    if audio then
        audio.audio:Destroy()
        self.audios[id] = nil
        if audio.cb then
            return audio.cb()
        end
        return -1
    end
end

--音乐、音效的状态改变
onSwitchChanged = function(self, on, type)
    if not on then
        if type == Type.Music then
            if self.bgm then
                self.bgm:Pause()
            end
        end

        local audios
        for id, audio in pairs(self.audios) do
            if audio.type == type then
                if nil == audios then
                    audios = {}
                end
                tb_insert(audios, id)
            end
        end
        if audios then
            for _, id in ipairs(audios) do
                self:stop(id)
            end
        end
    else
        if type == Type.Music then
            if self.bgm then
                if not self.bgm.isPlaying then
                    self.bgm:Play()
                else
                    self.bgm:Continue()
                end
            end
        end
    end
end

--播放
play = function(self, fileName, loop, cb, type, volume)
    local filename = str_format(AUDIO_FORMAT, fileName)

    local on
    if type == Type.Music then
        on = G.game.config.settings.music
    else
        on = G.game.config.settings.sound
    end

    if not on then
        if cb then
            cb()
        end
        return
    end

    local audio = G.api.Asset.CreateAudio(filename, self.root)

    if not audio then
        return
    end

    if fileName == "choose" or fileName == "click" or fileName == "close" then
        audio.duration = 0.3
    end

    audio.volume = volume or 1
    loop = loop or 1
    if loop ~= 1 or cb then -- 需要循环或者需要回调
        self.counter = self.counter + 1
        audio.dontDestroyOnStop = true
        if loop <= 0 then
            audio.loop = true
        end -- 无限循环

        self.audios[self.counter] = {
            audio = audio,
            loop = loop,
            cb = cb,
            type = type
        }
        audio:Play()
        return self.counter
    else -- 默认为不需要循环也不需要回调的，不需要stop
        audio:Play()
        return -1
    end
end

update = function(self)
    local finished
    for id, audio in pairs(self.audios) do
        local asset = audio.audio
        if not asset.isPlaying then -- todo:猜测这个标记可以用作播放完成
            if audio.loop > 0 then
                audio.loop = audio.loop - 1
                if audio.loop == 0 then -- 循环结束
                    if nil == finished then
                        finished = {}
                    end
                    tb_insert(finished, id)
                else -- 继续播放
                    asset:Play()
                end
            end
        end
    end

    if finished then
        for _, id in ipairs(finished) do
            return self:stop(id)
        end
    end
end

return AudiosSvc
