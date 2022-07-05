package messages

import (
	"xiao/core/models"
	"xiao/pkg/agent"
	"xiao/proto/proto_login"
)

// 创建玩家actor
type CreatePlayer struct {
	Session agent.PID
	Model   models.Player
}

type Login struct {
	Session agent.PID
	Request *proto_login.LoginRequest
}

// 玩家登录
type LoginResult struct {
	PlayerId  int64
	PlayerPID agent.PID
	Response  *proto_login.LoginResponse
}

type LoginSuccess struct {
}

type LoginReplace struct {
	Session agent.PID
}

type Logout struct{}

type Disconnect struct{}

type SysMessage struct {
	Content interface{}
}

type DispatchMessage struct {
	Content interface{}
}

type NewAnnouncement struct {
	Type      int32
	OpenTime  int64
	CloseTime int64
	Priority  int32
	Channel   int32
	Count     int32
	Title     string
	Content   string
	Version   []string
	Language  string
}

type GetAnnouncement struct {
}

type NewNotice struct {
	Type     int32    // 0全局 1大厅 2房间
	Priority int32    // 优先级
	Opentime int64    // 开启时间
	Times    int32    // 播放次数
	Timespan int32    // 播放间隔
	Content  string   // 内容Key、原始内容
	Params   []string // 内容参数
}
