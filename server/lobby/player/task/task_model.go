package task

import (
	"time"
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/bag"
	"xiao/lobby/player/mail"
	"xiao/proto/proto_player"
)

const (
	mail_title   = "UITask_Emailtitle"
	mail_content = "UITask_Emailcontent"
	mail_sender  = "UITask_Emailsender"
)

func getAllTask(ctx context.Player, plObj *models.Player) []*models.Task {
	plObj.InitTask()
	oldTasks := plObj.Task.Tasks

	m := make(map[int32]struct{})
	for _, task := range oldTasks {
		m[task.Id] = struct{}{}
	}

	newTasks := make([]*models.Task, 0)
	cfgs := define.GetTaskConfigs()
	for _, cfg := range cfgs {
		if _, ok := m[int32(cfg.Id)]; !ok {
			task := &models.Task{
				Id:       int32(cfg.Id),
				Type:     cfg.TaskType,
				TaskType: cfg.TaskSort,
				Target:   cfg.Param2,
				Param:    cfg.Param1,
				State:    define.StateOpen,
			}
			newTasks = append(newTasks, task)
		}
	}

	plObj.Task.Tasks = append(plObj.Task.Tasks, newTasks...)
	if checkRefresh(ctx, plObj) {
		refresh(ctx, plObj)
	}
	ctx.SavePlayer()
	return plObj.Task.Tasks
}

// 零点刷新每日任务
func RefreshTask(ctx context.Player, plObj *models.Player) {
	plObj.InitTask()
	if checkRefresh(ctx, plObj) {
		refresh(ctx, plObj)
		ctx.SavePlayer()
	}
}

func refresh(ctx context.Player, plObj *models.Player) {
	dailyTask := make([]*models.Task, 0)
	achieveTask := make([]*models.Task, 0)
	for _, task := range plObj.Task.Tasks {
		if task.Type == define.DailyTask {
			dailyTask = append(dailyTask, task)
		} else if task.Type == define.AchieveTask {
			achieveTask = append(achieveTask, task)
		}
	}

	sendAwardByMail(dailyTask, plObj.ID)
	cfgs := define.GetTaskConfigsByType(define.DailyTask)

	newDailytasks := make([]*models.Task, 0)
	for _, cfg := range cfgs {
		task := &models.Task{
			Id:       int32(cfg.Id),
			Type:     cfg.TaskType,
			TaskType: cfg.TaskSort,
			Target:   cfg.Param2,
			Param:    cfg.Param1,
			State:    define.StateOpen,
		}
		newDailytasks = append(newDailytasks, task)
	}

	plObj.Task.RefreshTime = time.Now().Unix()
	plObj.Task.Tasks = append(achieveTask, newDailytasks...)
}

func checkRefresh(ctx context.Player, plObj *models.Player) bool {
	now := time.Now()
	if plObj.Task.RefreshTime != 0 {
		lastRefreshTime := time.Unix(plObj.Task.RefreshTime, 0)
		if lastRefreshTime.YearDay() != now.YearDay() || lastRefreshTime.Year() != now.Year() {
			return true
		}
	} else {
		plObj.Task.RefreshTime = now.Unix()
		ctx.SavePlayer()
	}
	return false
}

// 任务刷新发奖励
func sendAwardByMail(tasks []*models.Task, playerId int64) {
	awards := make([]models.AccessoryInfo, 0)
	for _, task := range tasks {
		if task.State == define.StateComplete {
			awards = append(awards, ParseAward(task.Id)...)
		}
	}
	if len(awards) > 1 {
		mail.SendPrivateMail(mail_title, mail_content, nil, nil, MergeAward(awards), playerId, mail_sender)
	}
}

// 抛出事件更新任务状态
func notify(ctx context.Player, plObj *models.Player, key string, args map[string]interface{}) {
	getAllTask(ctx, plObj)
	changeTask := make([]*models.Task, 0)
	for _, task := range plObj.Task.Tasks {
		taskKey, ok := TaskTypeMap[task.TaskType]
		if ok {
			if taskKey == key && task.State == define.StateOpen {
				taskAdd(key, task, args)
				completeTask(task, args)
				changeTask = append(changeTask, task)
			}
		}
	}

	ctx.SavePlayer()
	DispatchTask(ctx, changeTask)
}

// 领取任务奖励
func receiveTaskAward(ctx context.Player, plObj *models.Player, taskId int32) (bool, []models.AccessoryInfo) {
	for _, task := range plObj.Task.Tasks {
		if task.Id == taskId {
			if task.State == define.StateComplete {
				awards := ParseAward(task.Id)
				taskReward(ctx, plObj, awards)
				task.State = define.StateClose

				DispatchTask(ctx, []*models.Task{task})
				ctx.SaveNow()
				return true, awards
			}
			break
		}
	}

	return false, nil
}

func taskReward(ctx context.Player, plObj *models.Player, accessorys []models.AccessoryInfo) {
	for _, accessory := range accessorys {
		bag.AddItem(ctx, plObj, accessory.ItemId, accessory.ItemNumber, define.ItemEventType_Mail)
	}
}

func DispatchTask(ctx context.Player, changeTask []*models.Task) {
	protoTask := make([]*proto_player.Task, 0)
	for _, task := range changeTask {
		protoTask = append(protoTask, &proto_player.Task{
			Id:       task.Id,
			Type:     task.Type,
			TaskType: task.TaskType,
			Count:    task.Count,
			Target:   task.Target,
			State:    int32(task.State),
		})
	}

	if len(protoTask) > 0 {
		ctx.Send(&proto_player.S2CTaskUpdate{
			TaskList: protoTask,
		})
	}
}
