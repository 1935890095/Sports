--[[
  Id: Const.lua
  Desc: 定义常量
--]]
local Const = {}
Const.DEFAULT_NOTCH_HEIGHT = 60 -- 默认的刘海屏高度

Const.InputType = {
  Down = 1, -- id，pos
  Up = 2, -- id, pos
  Drag = 3, -- id, curPos, oldPos
  SingleTouch = 4, -- pos
  DoubleTouch = 5, -- second pos, first pos
  LongTouch = 6, -- end pos, begin pos
  Zoom = 7, -- offsetX, offsetY, offsetDistance(只需要x轴方向的根据offsetX的正负值和绝对值去判断是放大或者缩小以及缩放值，offsetY同理，offsetDistance用于不考虑方向的缩放)
  KeyCode = 8 -- 按键
}
return Const