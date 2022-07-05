package models

import "xiao/proto/proto_node"

type Robot struct {
	Cache    `bson:"-" json:"-"`
	ID       int64       `bson:"_id" json:"ID"`
	Base     *PlayerBase `bson:"Base" json:"Base"`
	Room     *PlayerRoom `bson:"Room" json:"Room"`
	GamePlay *PlayerGame `bson:"Game" json:"Game"`
}

func (r *Robot) MarshalGame() *proto_node.Player {
	proto := &proto_node.Player{}
	proto.IsRobot = true
	proto.Id = r.ID
	proto.Name = r.Base.Name
	proto.Level = r.Base.Level
	proto.Money = r.Room.Money
	proto.Head = r.Base.Head
	return proto
}

func (r *Robot) Init(id int64) *Robot {
	// 初始化数据
	r.ID = id
	r.Base = new(PlayerBase)
	r.Room = new(PlayerRoom)
	r.Room.Options = make([]*RoomOptions, 0)
	r.GamePlay = &PlayerGame{
		GameInfoList: make([]*GameInfo, 0),
	}
	return r
}
