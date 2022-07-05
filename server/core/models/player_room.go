package models

import "xiao/proto/proto_player"

// 房间设置
type RoomOptions struct {
	Id         int32 `bson:"Id" json:"Id"`
	AutoBuy    bool  `bson:"AutoBuy" json:"AutoBuy"`
	AutoBuyMax bool  `bson:"AutoBuyMax" json:"AutoBuyMax"`
	Buy        int32 `bson:"Buy" json:"Buy"`
}

// 房间相关数据
type PlayerRoom struct {
	Money   int32          `bson:"Money" json:"Money"` // 房间内携带的钱
	Options []*RoomOptions `bson:"Options" json:"Options"`
}

func (room *PlayerRoom) GetOption(roomId int32) *RoomOptions {
	for _, opt := range room.Options {
		if opt.Id == roomId {
			return opt
		}
	}
	return nil
}

func (room *PlayerRoom) Marshal() *proto_player.PlayerRoom {
	proto := &proto_player.PlayerRoom{}
	proto.Options = make([]*proto_player.RoomOptions, 0)
	for _, opt := range room.Options {
		proto.Options = append(proto.Options, &proto_player.RoomOptions{
			Id:         opt.Id,
			AutoBuy:    opt.AutoBuy,
			AutoBuyMax: opt.AutoBuyMax,
		})
	}
	return proto
}
