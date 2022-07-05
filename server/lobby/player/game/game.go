package game

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/messages"
	"xiao/lobby/player/openconfig"
	"xiao/lobby/player/resource"
	"xiao/lobby/player/task"
	"xiao/pkg/agent"
	"xiao/pkg/log"
	"xiao/proto/proto_game"
	"xiao/proto/proto_node"
	"xiao/proto/proto_player"

	"github.com/gogo/protobuf/proto"
	"github.com/gogo/protobuf/types"
)

// 处理和game相关的消息
func Process(ctx context.Player, pl *models.Player, msg interface{}) interface{} {
	switch msg := msg.(type) {
	case *proto_node.G2LGameMessage:
		return onGameMessage(ctx, pl, msg)
	case *proto_game.C2SRoomOptions:
		onSetOptions(ctx, pl, msg)
	case *proto_game.C2SEnter:
		onEnter(ctx, pl, msg)
	case *proto_game.C2SLeave:
		onLeave(ctx, pl, true)
	case *proto_game.C2SGetNumOfPlayers:
		onGetNumOfPlayers(ctx, pl, msg)
	case *proto_game.C2SOp:
		toGame(ctx, pl, msg)
	case *proto_game.C2SStandUp:
		toGame(ctx, pl, msg)
	case *proto_game.C2SSitDown:
		toGame(ctx, pl, msg)
	case *proto_game.C2STips:
		toGame(ctx, pl, msg)
	case *proto_game.C2SBuy:
		onBuy(ctx, pl, msg)
	case *proto_game.C2SCheckEnter:
		onCheckEnter(ctx, pl)
	}

	return nil
}

func OnDisconnected(ctx context.Player, pl *models.Player) {
	if pl.InGame() {
		toGame(ctx, pl, &proto_node.L2GPlayerDisconnected{})
	}
}

func OnGameClosed(ctx context.Player, pl *models.Player) {
	if pl.InGame() {
		ctx.Unwatch(pl.Game.PID)
		pl.Game.GameId = 0
		pl.Game.LastGameId = 0
		pl.Game.PID = nil
		checkReturnMoney(ctx, pl)
		ctx.Send(&proto_game.S2CLeave{})
	}
}

func onGameMessage(ctx context.Player, pl *models.Player, msg *proto_node.G2LGameMessage) interface{} {
	var realMsg types.DynamicAny
	err := types.UnmarshalAny(msg.RealMessage, &realMsg)
	if err != nil {
		log.Error("game message unmarshal error:%v", err)
		return nil
	}

	switch msg := realMsg.Message.(type) {
	case *proto_node.G2LCostRequest:
		return onCost(ctx, pl, msg)
	case *proto_node.G2LAutoBuyRequest:
		return onAutoBuy(ctx, pl, msg)
	case *proto_game.S2CGameStart:
		onGameStart(ctx, pl, msg)
		ctx.Send(msg)
	case *proto_game.S2CGameOver:
		onGameOver(ctx, pl, msg)
		ctx.Send(msg)
	case *proto_node.G2LKickPlayer:
		onKickPlayer(ctx, pl, msg)
	default:
		ctx.Send(msg)
	}
	return nil
}

// 房间设置
func onSetOptions(ctx context.Player, pl *models.Player, msg *proto_game.C2SRoomOptions) {
	if msg.Id <= 0 {
		return
	}
	find := false
	for _, opt := range pl.Room.Options {
		if opt.Id == msg.Id {
			opt.AutoBuy = msg.AutoBuy
			opt.AutoBuyMax = msg.AutoBuyMax
			opt.Buy = msg.Buy
			find = true
			break
		}
	}

	if !find {
		pl.Room.Options = append(pl.Room.Options, &models.RoomOptions{
			Id:         msg.Id,
			AutoBuy:    msg.AutoBuy,
			AutoBuyMax: msg.AutoBuyMax,
			Buy:        msg.Buy,
		})
	}

	ctx.SavePlayer()

	ctx.Send(&proto_game.S2CRoomOptions{
		Id:         msg.Id,
		AutoBuy:    msg.AutoBuy,
		AutoBuyMax: msg.AutoBuyMax,
		Buy:        msg.Buy,
	})
}

func onCheckEnter(ctx context.Player, pl *models.Player) {
	if pl.InGame() {
		ok := enterGame(ctx, pl, pl.Cache.Game.PID)
		if !ok {
			checkReturnMoney(ctx, pl)
			ctx.Send(&proto_game.S2CLeave{})
		}
	}
}

// 进入房间
func onEnter(ctx context.Player, pl *models.Player, msg *proto_game.C2SEnter) {
	if pl.InGame() {
		last := pl.Game.GameId
		onLeave(ctx, pl, false)
		pl.Game.LastGameId = last
	}

	if !pl.IsResourceEnough(define.ResourceType_Money, msg.Buy) {
		ctx.Send(&proto_game.S2CEnter{
			Result: proto_game.EnterNotEnoughMoney,
		})
		return
	}

	pidResp, err := ctx.Invoke("Match", "proxy")
	if err != nil {
		log.Error("get match pid error: %v", err)
		return
	}

	cfg := define.GetRoomConfig(msg.RoomId)
	if cfg == nil {
		return
	}

	if msg.Buy < cfg.StakesMax {
		ctx.Send(&proto_game.S2CEnter{
			Result: proto_game.EnterFail,
		})
		return
	}

	find := false
	for _, v := range cfg.Nums {
		if v == msg.NumOfPlayers {
			find = true
			break
		}
	}

	if !find {
		return
	}

	// 请求匹配
	matchPID, _ := pidResp.(agent.PID)
	matchResp, err := ctx.Call(matchPID, &proto_node.L2MRequest{
		New:          false,
		Player:       pl.MarshalGame(),
		RoomId:       msg.RoomId,
		NumOfPlayers: msg.NumOfPlayers,
		ExcludeId:    pl.Game.LastGameId,
	})

	if err != nil {
		log.Error("player: %d match error", pl.ID)
		return
	}

	resp, _ := matchResp.(*proto_node.L2MResponse)
	if !resp.Success {
		log.Error("player: %d match fail", pl.ID)
		return
	}

	resource.Change(ctx, pl, define.ResourceType_Money, -msg.Buy, define.ResourceEventType_BuyIn)
	pl.Room.Money = msg.Buy
	resource.Sync(ctx, pl)
	ctx.SavePlayer()

	// 请求进入房间
	roomPID := agent.NewPID(resp.RoomPID.Address, resp.RoomPID.Id)
	if enterGame(ctx, pl, roomPID) {
		pl.Game.RoomId = msg.RoomId
		pl.Game.NumOfPlayers = msg.NumOfPlayers
		ctx.Send(&proto_game.S2CEnter{
			Result: proto_game.EnterSuccess,
		})
	}
}

func enterGame(ctx context.Player, pl *models.Player, roomPID agent.PID) bool {
	enterResp, err := ctx.Call(roomPID, &proto_node.L2GEnterRequest{
		Player: pl.MarshalGame(),
		PID: &proto_node.PID{
			Address: ctx.Self().Address,
			Id:      ctx.Self().Id,
		},
	})

	if err != nil {
		log.Error("player: %d enter error: %v", pl.ID, err)
		return false
	}

	result, _ := enterResp.(*proto_node.L2GEnterResponse)
	if !result.Success {
		log.Error("player: %d enter fail", pl.ID)
		return false
	}

	pl.Cache.Game.GameId = result.GameId
	pl.Cache.Game.PID = roomPID
	ctx.Watch(roomPID)

	return true
}

// 离开房间
func onLeave(ctx context.Player, pl *models.Player, sync bool) {
	if pl.Cache.Game.PID == nil {
		return
	}

	ctx.Call(pl.Cache.Game.PID, &proto_node.L2GLeaveRequest{
		Player: pl.MarshalGame(),
	})
	ctx.Unwatch(pl.Game.PID)
	pl.Game.LastGameId = pl.Game.GameId
	pl.Game.GameId = 0
	pl.Game.PID = nil
	pl.Game.RoomId = 0
	pl.Game.NumOfPlayers = 0

	if sync {
		ctx.Send(&proto_game.S2CLeave{})
	}
	checkReturnMoney(ctx, pl)
}

// 获取房间在线人数
func onGetNumOfPlayers(ctx context.Player, pl *models.Player, msg *proto_game.C2SGetNumOfPlayers) {
	result, _ := ctx.Invoke("Match", "count", msg.RoomId)
	count := result.(int32)
	ctx.Send(&proto_game.S2CGetNumOfPlayers{
		RoomId:       msg.RoomId,
		NumOfPlayers: count,
	})
}

// 转发到game
func toGame(ctx context.Player, pl *models.Player, msg proto.Message) {
	if pl.Cache.Game.PID == nil {
		log.Error("to game error, player: %d not in game", pl.ID)
		return
	}

	real, err := types.MarshalAny(msg)
	if err != nil {
		log.Error("marshal any error: %v", err)
		return
	}

	ctx.Cast(pl.Cache.Game.PID, &proto_node.L2GPlayerMessage{
		PlayerId:    pl.ID,
		RealMessage: real,
	})
}

func onCost(ctx context.Player, pl *models.Player, msg *proto_node.G2LCostRequest) *proto_node.G2LCostResponse {
	ok := false
	if pl.Room.Money >= msg.Money {
		pl.Room.Money -= msg.Money
		ok = true
		ctx.SavePlayer()
	}

	return &proto_node.G2LCostResponse{
		Success: ok,
		Money:   pl.Room.Money,
	}
}

func onAutoBuy(ctx context.Player, pl *models.Player, msg *proto_node.G2LAutoBuyRequest) *proto_node.G2LAutoBuyResponse {
	resp := new(proto_node.G2LAutoBuyResponse)

	roomId := msg.RoomId
	if roomId <= 0 {
		ctx.Send(msg)
		return resp
	}

	option := pl.Room.GetOption(roomId)
	if option == nil || !option.AutoBuy {
		ctx.Send(msg)
		return resp
	}

	cfg := define.GetRoomConfig(roomId)
	if cfg == nil {
		return resp
	}

	buy := option.Buy
	if option.AutoBuyMax {
		buy = cfg.BuyMax
	}
	buy -= pl.Room.Money
	if !pl.IsResourceEnough(define.ResourceType_Money, buy) {
		buy = cfg.BuyMin - pl.Room.Money
		if !pl.IsResourceEnough(define.ResourceType_Money, buy) {
			ctx.Send(msg)
			return resp
		}
	}

	if resource.Change(ctx, pl, define.ResourceType_Money, -buy, define.ResourceEventType_BuyIn) {
		pl.Room.Money += buy
		resource.Sync(ctx, pl)
		resp.Success = true
		resp.Money = buy
	}

	return resp
}

func onGameStart(ctx context.Player, pl *models.Player, msg *proto_game.S2CGameStart) {
	task.Notify(ctx, pl, "join_game", nil)
}

func onGameOver(ctx context.Player, pl *models.Player, msg *proto_game.S2CGameOver) {
	if msg.PlayerId != pl.ID {
		return
	}
	info := pl.GamePlay.UpdateInfo(msg.Info)
	ctx.SavePlayer()
	sync := new(proto_player.S2CGameInfoChanged)
	sync.Info = info.Marshal()
	ctx.Send(sync)

	task.Notify(ctx, pl, "bet", map[string]interface{}{"bet": msg.Info.TotalBetMoney})
	openconfig.CheckFunctionOpen(ctx, pl)

	key := ""
	switch msg.Info.CardType {
	case proto_game.CardTypesRoyalFlush:
		key = define.Misc().RoyalFlash
	case proto_game.CardTypesStraightFlush:
		key = define.Misc().Flash
	case proto_game.CardTypesFourOfAKind:
		key = define.Misc().Four
	}

	if key != "" {
		ctx.Invoke("Announce", "newNotice", &messages.NewNotice{
			Type:     0,
			Priority: 0,
			Times:    1,
			Timespan: 1,
			Opentime: 0,
			Content:  key,
			Params:   []string{pl.Base.Name},
		})
	}
}

func onKickPlayer(ctx context.Player, pl *models.Player, msg *proto_node.G2LKickPlayer) {
	checkReturnMoney(ctx, pl)

	switch msg.Reason {
	case int32(define.KickReasonClose):
		ctx.Send(&proto_game.S2CGameKick{
			Type: proto_game.Closed,
		})
	}
}

func onBuy(ctx context.Player, pl *models.Player, msg *proto_game.C2SBuy) {
	if !pl.InGame() {
		ctx.Send(&proto_game.S2CBuy{
			Success: false,
		})
	}

	if !resource.Change(ctx, pl, define.ResourceType_Money, -msg.Buy, define.ResourceEventType_BuyIn) {
		ctx.Send(&proto_game.S2CBuy{
			Success: false,
		})
	}

	pl.Room.Money += msg.Buy
	resource.Sync(ctx, pl)
	ctx.Send(&proto_game.S2CBuy{
		Success: true,
	})
	ctx.SavePlayer()
	toGame(ctx, pl, msg)
}

func checkReturnMoney(ctx context.Player, pl *models.Player) {
	if pl.Room.Money > 0 {
		resource.Change(ctx, pl, define.ResourceType_Money, pl.Room.Money, define.ResourceEventType_Return)
		resource.Sync(ctx, pl)
		pl.Room.Money = 0
		ctx.SavePlayer()
	}
}

// func onMoneyNotEnough(ctx context.Player, pl *models.Player, msg *proto_game.S2CMoneyNotEnough) {
// 	roomId := msg.RoomId
// 	if roomId <= 0 {
// 		ctx.Send(msg)
// 		return
// 	}

// 	option := pl.Room.GetOption(roomId)
// 	if option == nil || !option.AutoBuy {
// 		ctx.Send(msg)
// 		return
// 	}

// 	cfg := define.GetRoomConfig(roomId)
// 	if cfg == nil {
// 		return
// 	}

// 	buy := option.Buy
// 	if option.AutoBuyMax {
// 		buy = cfg.BuyMax
// 	}
// 	buy -= pl.Room.Money
// 	if !pl.IsResourceEnough(define.ResourceType_Money, buy) {
// 		buy = cfg.BuyMin - pl.Room.Money
// 		if !pl.IsResourceEnough(define.ResourceType_Money, buy) {
// 			ctx.Send(msg)
// 			return
// 		}
// 	}

// 	if resource.Change(ctx, pl, define.ResourceType_Money, -buy, define.ResourceEventType_BuyIn) {
// 		pl.Room.Money += buy
// 		resource.Sync(ctx, pl)

// 		toGame(ctx, pl, &proto_game.C2SBuy{
// 			Buy:     buy,
// 			AutoBuy: true,
// 		})
// 	}
// }
