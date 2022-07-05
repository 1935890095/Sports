package endpoint

import (
	"xiao/pkg/agent"
	"xiao/pkg/module"

	"github.com/gogo/protobuf/proto"
)

type Player struct {
	pkg  int
	pgc  int
	mpkg int

	playType int
	gameId   int64
	gamePID  agent.PID
}

func (c *Player) Enter(agent module.Agent, playerId int64) {
}

func (c *Player) Match(agent module.Agent, playerId int64, playType int) {
	/*
	   1. 如何获得匹配服
	   2. 匹配
	   3. 进入
	   3. 退出
	   4. 消息推送
	   5. 终止
	*/
	// request := proto_game.MatchRequest{
	// 	PlayerId: playerId,
	// }
	// agent.Invoke("Nodes")
}

func (c *Player) Create(playerId int64, playType int) {

}

func (c *Player) Tell(msg proto.Message) {
}
