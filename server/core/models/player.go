package models

import (
	"xiao/core/define"
	"xiao/pkg/agent"
	"xiao/proto/proto_node"
	"xiao/proto/proto_player"
)

type Cache struct {
	Session           agent.PID // 网络session pid
	Disconnect        bool      // 是否被动断开
	Game              Game      // 游戏相关缓存数据
	ChangedProperties map[define.ResourceType]bool
	SaveTick          int
	FunctionOpen      map[int32]int32 // 功能开启
}

type Game struct {
	PID        agent.PID
	GameId     int64
	LastGameId int64

	RoomId       int32 // 房间配置id
	NumOfPlayers int32 // 房间人数
}

// 玩家数据
type Player struct {
	Cache    `bson:"-" json:"-"`
	ID       int64       `bson:"_id" json:"ID"`
	Base     *PlayerBase `bson:"Base" json:"Base"`
	Room     *PlayerRoom `bson:"Room" json:"Room"`
	Mail     *PlayerMail `bson:"Mail" json:"Mail"`
	GamePlay *PlayerGame `bson:"Game" json:"Game"`
	Bag      *PlayerBag  `bson:"Bag" json:"Bag"`
	Task     *PlayerTask `bson:"Task" json:"Task"`
}

// 后续所有系统单独定义到一个结构中
func (pl *Player) Init(id int64) *Player {
	// 初始化数据
	pl.ID = id
	pl.Room = new(PlayerRoom)
	pl.Room.Options = make([]*RoomOptions, 0)
	pl.InitBase()
	pl.InitBag()
	pl.InitGame()
	pl.InitMail()
	pl.InitTask()
	return pl
}

func (pl *Player) Marshal() *proto_player.PlayerData {
	proto := &proto_player.PlayerData{}
	proto.Id = pl.ID
	proto.Base = pl.Base.Marshal()
	proto.Room = pl.Room.Marshal()
	proto.Bag = pl.Bag.Marshal()
	proto.Game = pl.GamePlay.Marshal()
	return proto
}

func (pl *Player) InGame() bool {
	return pl.Game.PID != nil
}

func (pl *Player) MarshalGame() *proto_node.Player {
	proto := &proto_node.Player{}
	proto.Id = pl.ID
	proto.Name = pl.Base.Name
	proto.Level = pl.Base.Level
	proto.Money = pl.Room.Money
	proto.Head = pl.Base.Head
	return proto
}
