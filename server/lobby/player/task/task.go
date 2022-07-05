package task

import (
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/resource"
	"xiao/proto/proto_player"
)

// 事件触发
func Notify(ctx context.Player, plObj *models.Player, key string, args map[string]interface{}) {
	notify(ctx, plObj, key, args)
}

// 获取任务奖励
func ReceiveTaskAward(ctx context.Player, plObj *models.Player, req *proto_player.C2SReceiveTaskAward) {
	ret := &proto_player.S2CReceiveTaskAward{}
	success, awards := receiveTaskAward(ctx, plObj, req.Id)
	if success {
		ret.Success = true
		m := make([]*proto_player.AccessoryInfo, 0)
		for _, award := range awards {
			m = append(m, &proto_player.AccessoryInfo{
				ItemId:     award.ItemId,
				ItemNumber: award.ItemNumber,
			})
		}

		ret.Award = m
	}

	ctx.Send(ret)
	resource.Sync(ctx, plObj)
}

// 获取所有任务
func GetAllTask(ctx context.Player, plObj *models.Player, req *proto_player.C2SGetAllTask) {
	ret := &proto_player.S2CGetAllTask{}
	tasks := getAllTask(ctx, plObj)

	taskList := make([]*proto_player.Task, 0)
	for _, task := range tasks {
		taskList = append(taskList, &proto_player.Task{
			Id:       task.Id,
			Type:     task.Type,
			TaskType: task.TaskType,
			Count:    task.Count,
			Target:   task.Target,
			State:    int32(task.State),
		})
	}

	ret.TaskList = taskList
	ctx.Send(ret)
}
