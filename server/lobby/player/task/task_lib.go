package task

import (
	"strconv"
	"strings"
	"time"
	"xiao/core/define"
	"xiao/core/models"
)

var TaskTypeMap = map[int32]string{
	define.TaskType_LoginTimes:       "login",
	define.TaskType_JoinGameTimes:    "join_game",
	define.TaskType_JoinAuctionTimes: "join_auction",
	define.TaskType_WinGameCount:     "win_game",
	define.TaskType_TotalBetCount:    "bet"}

func ParseOpenCondition(s string) map[string]int {
	m := make(map[string]int)

	conditions := strings.Split(s, "#")
	for _, stringCondition := range conditions {
		condition := strings.Split(stringCondition, ",")
		i, err := strconv.Atoi(condition[1])
		if err != nil {
			return nil
		}
		m[condition[0]] = i
	}

	return m
}

func ParseAward(taskId int32) []models.AccessoryInfo {
	cfg := define.GetTaskConfig(taskId)
	awards := make([]models.AccessoryInfo, 0)

	if len(cfg.AwardNum1) == len(cfg.AwardType1) && len(cfg.AwardNum1) > 0 {
		for index, itemId := range cfg.AwardType1 {
			awards = append(awards, models.AccessoryInfo{
				ItemId:     itemId,
				ItemNumber: cfg.AwardNum1[index],
			})
		}
	}

	return awards
}

func MergeAward(awards ...[]models.AccessoryInfo) []models.AccessoryInfo {
	m := make(map[int32]int32)
	for _, award := range awards {
		for _, item := range award {
			if val, ok := m[item.ItemId]; ok {
				m[item.ItemId] = val + item.ItemNumber
			} else {
				m[item.ItemId] = item.ItemNumber
			}
		}
	}

	ret := make([]models.AccessoryInfo, 0)
	for k, v := range m {
		ret = append(ret, models.AccessoryInfo{
			ItemId:     k,
			ItemNumber: v,
		})
	}

	return ret
}

func keyfind(args map[string]interface{}, key string) interface{} {
	if args != nil {
		return args[key]
	}

	return nil
}

func taskAdd(key string, task *models.Task, args map[string]interface{}) {
	switch key {
	case "bet":
		bet, ok := keyfind(args, "bet").(int32)
		if ok {
			task.Count += bet
		}
	case "login":
		if checkDayTrigger(task) {
			task.Count++
			task.Trigger = time.Now().Unix()
		}
	default:
		task.Count++
	}
}

func completeTask(task *models.Task, args map[string]interface{}) {
	if task.State == define.StateOpen {
		if task.Count >= task.Target {
			task.Count = task.Target
			task.State = define.StateComplete
		}
	}
}

func checkDayTrigger(task *models.Task) bool {
	if task.Trigger == 0 {
		return true
	}
	now := time.Now()
	trigger := time.Unix(task.Trigger, 0)
	return trigger.YearDay() != now.Day() && trigger.Year() == now.Year()
}
