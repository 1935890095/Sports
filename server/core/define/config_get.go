package define

import (
	"fmt"
	"xiao/pkg/config"
	"xiao/pkg/log"
)

var misc *MiscConfig

func GetRoomConfig(id int32) *RoomConfig {
	cfg := new(RoomConfig)
	if err := config.Bind(fmt.Sprintf("room.%d", id), cfg); err != nil {
		return nil
	}

	return cfg
}

func GetAllRobotConfigs() []*RobotConfig {
	cfgs := make([]*RobotConfig, 0)
	if err := config.Bind("robot", &cfgs); err != nil {
		return nil
	}

	return cfgs
}

func GetAllRobotAIConfigs() []*RobotAiConfig {
	cfgs := make([]*RobotAiConfig, 0)
	if err := config.Bind("robot_ai", &cfgs); err != nil {
		return nil
	}

	return cfgs
}

func GetAllHandTypeConfigs() []*HandTypeConfig {
	cfgs := make([]*HandTypeConfig, 0)
	if err := config.Bind("hand_type", &cfgs); err != nil {
		return nil
	}

	return cfgs
}

func GetItemConfig(id int32) *ItemConfig {
	cfg := new(ItemConfig)
	if err := config.Bind(fmt.Sprintf("item.%d", id), cfg); err != nil {
		return nil
	}

	return cfg
}

func GetHeadConfigs() []HeadConfig {
	cfg := make([]HeadConfig, 0)
	if err := config.Bind("head", &cfg); err != nil {
		return nil
	}

	return cfg
}

func GetHeadConfig(id string) *HeadConfig {
	cfg := new(HeadConfig)
	if err := config.Bind(fmt.Sprintf("head.%s", id), cfg); err != nil {
		return nil
	}

	return cfg
}

func GetTaskConfigs() []TaskConfig {
	cfg := make([]TaskConfig, 0)
	if err := config.Bind("task", &cfg); err != nil {
		return nil
	}

	return cfg
}

func GetTaskConfigsByType(t int) []TaskConfig {
	cfgs := GetTaskConfigs()

	list := make([]TaskConfig, 0)
	for _, cfg := range cfgs {
		if int(cfg.TaskType) == t {
			list = append(list, cfg)
		}
	}

	return list
}

func GetTaskConfig(id int32) *TaskConfig {
	cfg := new(TaskConfig)
	if err := config.Bind(fmt.Sprintf("task.%d", id), cfg); err != nil {
		return nil
	}

	return cfg
}

func Misc() *MiscConfig {
	if misc == nil {
		misc = new(MiscConfig)
		if config.Bind("misc", misc) != nil {
			log.Error("misc config load failed")
		}
	}
	return misc
}
