package openconfig

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/pkg/config"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func Init(plObj *models.Player) {
	var cfgs []define.OpenConfig
	plObj.Cache.FunctionOpen = make(map[int32]int32)
	m := plObj.Cache.FunctionOpen
	if err := config.Bind("open", &cfgs); err != nil {
		log.Info("get openconfig error:", err)
		return
	}
	for _, cfg := range cfgs {
		if len(cfg.Type) != len(cfg.Operator) || len(cfg.Type) != len(cfg.Value) {
			m[cfg.Id] = define.FUNCTION_CLOSE
			continue
		}

		var flag bool
		for index, cType := range cfg.Type {
			if !check(plObj, cType, cfg.Operator[index], cfg.Value[index]) {
				m[cfg.Id] = define.FUNCTION_CLOSE
				flag = true
				break
			}
		}
		if !flag {
			m[cfg.Id] = define.FUNCTION_OPEN
		}
	}

	// form db
	list := make([]models.FunctionOpen, 0)
	if err := mongo.NewIQ("game", "openconfig").Find(&list).Error(); err != nil {
		log.Debug("get gm function state err:", err)
		return
	}
	for _, GMState := range list {
		m[int32(GMState.Id)] = int32(GMState.State)
	}
}

func checkCondition(ctx context.Player, plObj *models.Player) {
	var cfgs []define.OpenConfig
	if err := config.Bind("open", &cfgs); err != nil {
		return
	}
	m := plObj.Cache.FunctionOpen
	ret := &proto_player.S2CFuncOpen{}
	list := make([]*proto_player.FuncState, 0)

	for _, cfg := range cfgs {
		state, ok := m[cfg.Id]
		if state == define.FUNCTION_CLOSE || !ok {
			if len(cfg.Type) != len(cfg.Operator) || len(cfg.Type) != len(cfg.Value) {
				m[cfg.Id] = define.FUNCTION_CLOSE
				continue
			}
			var flag bool
			for index, cType := range cfg.Type {
				if !check(plObj, cType, cfg.Operator[index], cfg.Value[index]) {
					m[cfg.Id] = define.FUNCTION_CLOSE
					flag = true
					break
				}
			}
			if !flag {
				list = append(list, &proto_player.FuncState{Id: cfg.Id, State: define.FUNCTION_OPEN})
				m[cfg.Id] = define.FUNCTION_OPEN
			}
		}
	}

	if len(list) > 0 {
		ret.List = list
		ctx.Send(ret)
	}
}

func Gm(ctx context.Player, plObj *models.Player, msg *models.FunctionOpen) {
	list := make([]*proto_player.FuncState, 0)
	m := plObj.Cache.FunctionOpen
	state, ok := m[int32(msg.Id)]
	m[int32(msg.Id)] = int32(msg.State)

	if state != int32(msg.State) || !ok {
		list = append(list, &proto_player.FuncState{Id: int32(msg.Id), State: int32(msg.State)})
	}

	if len(list) > 0 {
		ret := &proto_player.S2CFuncOpen{}
		ret.List = list
		ctx.Send(ret)
	}
}

func check(plObj *models.Player, cType int32, cOperator int32, param int32) bool {
	switch define.ConditionType(cType) {
	case define.MatchLevelCondition:
		// TODO:联赛等级
		return true
	case define.GameCountCondition:
		info := plObj.GamePlay.GetInfo(define.GameTypeNormal)
		return Process(info.TotalCount, cOperator, param)
	default:
		log.Info("condition type error")
	}

	return false
}

func Process(val int32, cOperator int32, param int32) bool {
	switch cOperator {
	case 1:
		return val > param
	case 2:
		return val < param
	case 3:
		return val == param
	default:
	}
	return false
}
