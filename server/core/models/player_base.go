package models

import (
	"fmt"
	"math/rand"
	"strconv"
	"time"
	"xiao/core/define"
	"xiao/proto/proto_player"
)

// 玩家基础信息
type PlayerBase struct {
	Name       string `bson:"Name" json:"Name"`
	Level      int32  `bson:"Level" json:"Level"`
	Money      int32  `bson:"Money" json:"Money"`
	Coin       int32  `bson:"Coin" json:"Coin"`
	Head       string `bson:"Head" json:"Head"`
	CreateTime int64  `bson:"CreateTime" json:"CreateTime"`
}

func (pl *Player) InitBase() {
	pl.Base = new(PlayerBase)
	pl.Base.Name = randomName()
	pl.Base.Level = 1
	pl.Base.Head = randomHead()
	pl.Base.CreateTime = time.Now().Unix()
}

func (base *PlayerBase) Marshal() *proto_player.PlayerBase {
	proto := &proto_player.PlayerBase{}
	proto.Name = base.Name
	proto.Level = base.Level
	proto.Money = base.Money
	proto.Coin = base.Coin
	proto.Head = base.Head
	return proto
}

func randomName() string {
	rand.Seed(time.Now().UnixNano())
	return fmt.Sprintf("Gues%06v", rand.Int31n(1000000))
}

func randomHead() string {
	headCfgs := define.GetHeadConfigs()
	index := rand.Intn(len(headCfgs))
	return strconv.Itoa(int(headCfgs[index].Id))
}
