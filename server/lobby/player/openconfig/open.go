package openconfig

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/proto/proto_player"
)

func CheckFunctionOpen(ctx context.Player, plObj *models.Player) {
	checkCondition(ctx, plObj)
}

func GetFunctionStates(ctx context.Player, plObj *models.Player) {
	ret := &proto_player.S2CFuncOpen{}
	list := make([]*proto_player.FuncState, 0)

	for id, state := range plObj.Cache.FunctionOpen {
		list = append(list, &proto_player.FuncState{Id: id, State: state})
	}
	ret.List = list
	ctx.Send(ret)
}

func GetFunctionState(id int32, plObj *models.Player) bool {
	state, ok := plObj.Cache.FunctionOpen[id]
	return state == define.FUNCTION_OPEN || !ok
}
