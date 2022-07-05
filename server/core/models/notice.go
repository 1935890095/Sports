package models

type NoticeContent struct {
	Type     int32    `json:"type" bson:"type"`         // 0全局 1大厅 2房间
	Priority int32    `json:"priority" bson:"priority"` // 优先级
	Opentime int64    `json:"opentime" bson:"opentime"` // 开启时间
	Times    int32    `json:"times" bson:"times"`       // 播放次数
	Timespan int32    `json:"timespan" bson:"timespan"` // 播放间隔
	Content  string   `json:"content" bson:"content"`   // 内容Key、原始内容
	Params   []string `json:"params" bson:"params"`     // 内容参数
}
