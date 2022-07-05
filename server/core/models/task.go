package models

type Task struct {
	Id       int32 `json:"_id" bson:"_id"`
	Type     int32 `json:"type" bson:"type"`
	TaskType int32 `json:"task_type" bson:"task_type"`
	State    int   `json:"state" bson:"state"`
	Count    int32 `json:"count" bson:"count"`
	Target   int32 `json:"target" bson:"target"`
	Param    int32 `json:"param" bson:"param"`
	Trigger  int64 `json:"trigger" bson:"trigger"`
}

type PlayerTask struct {
	RefreshTime int64   `json:"refresh_time"`
	Tasks       []*Task `json:"tasks"`
}

func (pl *Player) InitTask() {
	if pl.Task == nil {
		pl.Task = &PlayerTask{
			Tasks: make([]*Task, 0),
		}
	}
}
