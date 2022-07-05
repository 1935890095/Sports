package player

import (
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/messages"
	"xiao/lobby/player/base"
	"xiao/lobby/player/game"
	"xiao/lobby/player/gm"
	"xiao/lobby/player/login"
	"xiao/lobby/player/mail"
	"xiao/lobby/player/openconfig"
	"xiao/lobby/player/task"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func dispatch(ctx context.Player, pl *models.Player, msg interface{}) interface{} {
	switch msg := msg.(type) {
	case *messages.LoginSuccess:
		login.Login(ctx, pl)
	case *messages.LoginReplace:
		return login.Replace(ctx, pl, msg.Session)
	case *messages.Logout:
		login.Logout(ctx, pl)
	case *messages.Disconnect:
		login.Disconnect(ctx, pl)
	case *proto_player.GetPlayerData:
		login.GetPlayerData(ctx, pl)
	case *proto_player.C2SGetMailList:
		mail.GetMailList(ctx, pl, msg)
	case *proto_player.C2SGetMailDetail:
		mail.GetMailDetail(ctx, pl, msg)
	case *proto_player.C2SReceiveMailAward:
		mail.ReceiveMailAward(ctx, pl, msg)
	case *proto_player.C2SDeleteMail:
		mail.DeleteMail(ctx, pl, msg)
	case *messages.SysMessage:
		onSysMessage(ctx, pl, msg.Content)
	case *messages.DispatchMessage:
		ctx.Send(msg.Content)
	case *proto_player.C2SGMCommand:
		gm.Process(ctx, pl, msg.Command)
	case *proto_player.C2SChangeHead:
		base.ChangeHead(ctx, pl, msg.Head)
	case *proto_player.C2SPlayerRename:
		base.ChangeName(ctx, pl, msg.NewName)
	case *proto_player.C2SGetAllTask:
		task.GetAllTask(ctx, pl, msg)
	case *proto_player.C2SReceiveTaskAward:
		task.ReceiveTaskAward(ctx, pl, msg)
	case *proto_player.C2SGetRoomPlayerInfo:
		base.GetRoomPlayerInfo(ctx, pl, msg)
	default:
		return game.Process(ctx, pl, msg)
	}
	return nil
}

func onSysMessage(ctx context.Player, pl *models.Player, msg interface{}) {
	switch msg := msg.(type) {
	case *models.FunctionOpen:
		openconfig.Gm(ctx, pl, msg)
	default:
		log.Debug("onSysMessage:", msg)
	}
}
