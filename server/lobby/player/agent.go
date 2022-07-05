package player

import (
	"time"
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/player/game"
	"xiao/lobby/player/openconfig"
	"xiao/lobby/player/task"
	"xiao/pkg/agent"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/pkg/module/modules"
)

const SAVE_TICK = 10 * 10

type PlayerAgent struct {
	modules.BaseAgent
	model *models.Player
}

func New(model *models.Player, session agent.PID) *PlayerAgent {
	a := &PlayerAgent{
		model: model,
	}
	a.model.Cache.Session = session
	return a
}

func (pl *PlayerAgent) OnStart(ctx agent.Context) {
	pl.BaseAgent.OnStart(ctx)
	log.Info("=============== login")
	misc := define.Misc()
	log.Error("== %v", misc)
	openconfig.Init(pl.model)
}

func (pl *PlayerAgent) OnStop() {
}

func (pl *PlayerAgent) OnTerminated(pid agent.PID, reason int) {
	pidStr := agent.Address(pid)
	if pidStr == agent.Address(pl.model.Session) {
		game.OnDisconnected(pl, pl.model)
	} else if pidStr == agent.Address(pl.model.Game.PID) {
		game.OnGameClosed(pl, pl.model)
	}
}

func (pl *PlayerAgent) Send(msg interface{}) {
	if pl.model.Cache.Session != nil {
		pl.Context.Cast(pl.model.Cache.Session, msg)
	}
}

func (pl *PlayerAgent) Call(pid agent.PID, msg interface{}) (interface{}, error) {
	return pl.Context.Call(pid, msg)
}

func (pl *PlayerAgent) Cast(pid agent.PID, msg interface{}) {
	pl.Context.Cast(pid, msg)
}

func (pl *PlayerAgent) SaveNow() {
	pl.model.SaveTick = 1
}

func (pl *PlayerAgent) SavePlayer() {
	if pl.model.SaveTick <= 0 {
		pl.model.SaveTick = SAVE_TICK
	}
}

func (pl *PlayerAgent) OnTick(delta time.Duration) {
	if pl.model.SaveTick > 0 {
		pl.model.SaveTick--
		if pl.model.SaveTick <= 0 {
			if err := mongo.NewIQ("game", "player").Where("_id", pl.model.ID).Update(pl.model).Error(); err != nil {
				log.Error("save player data error: %v", err)
			}
			pl.model.SaveTick = -1
		}
	}

	task.RefreshTask(pl, pl.model)
}

func (pl *PlayerAgent) Stop() {
	pl.Context.Stop()
}

func (pl *PlayerAgent) OnMessage(msg interface{}) interface{} {
	return dispatch(pl, pl.model, msg)
}

func (pl *PlayerAgent) Watch(pid agent.PID) {
	pl.Context.Watch(pid)
}

func (pl *PlayerAgent) Unwatch(pid agent.PID) {
	pl.Context.Unwatch(pid)
}
