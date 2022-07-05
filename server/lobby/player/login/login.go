package login

import (
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/game"
	"xiao/lobby/player/openconfig"
	"xiao/lobby/player/task"
	"xiao/pkg/agent"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func Login(ctx context.Player, pl *models.Player) {
}

func Replace(ctx context.Player, pl *models.Player, session agent.PID) interface{} {
	if pl.Cache.Session != nil {
		pl.Cache.Disconnect = true
		ctx.Cast(pl.Cache.Session, &proto_player.S2CKick{})
	}
	pl.Cache.Session = session
	return nil
}

func Logout(ctx context.Player, pl *models.Player) {
	log.Info("====================logout")
	ctx.SavePlayer()
	ctx.Stop()
}

func Disconnect(ctx context.Player, pl *models.Player) {
	log.Info("====================disconnect")
	if !pl.Cache.Disconnect {
		pl.Cache.Session = nil
	}
	pl.Cache.Disconnect = false
	game.OnDisconnected(ctx, pl)
}

func GetPlayerData(ctx context.Player, pl *models.Player) {
	ctx.Send(&proto_player.GetPlayerData{
		Data: pl.Marshal(),
	})

	onLogin(ctx, pl)
}

func onLogin(ctx context.Player, pl *models.Player) {
	task.Notify(ctx, pl, "login", nil)
	openconfig.GetFunctionStates(ctx, pl)
}
