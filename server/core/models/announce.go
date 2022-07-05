package models

type Announcement struct {
	Id        int64    `json:"_id" bson:"_id"`
	Type      int32    `json:"type" bson:"type"`
	OpenTime  int64    `json:"open_time" bson:"open_time"`
	CloseTime int64    `json:"close_time" bson:"close_time"`
	Priority  int32    `json:"priority" bson:"priority"`
	Channel   int32    `json:"channel" bson:"channel"`
	Count     int32    `json:"count" bson:"count"`
	Title     string   `json:"title" bson:"title"`
	Content   string   `json:"content" bson:"content"`
	Version   []string `json:"version" bson:"version"`
	Language  string   `json:"language" bson:"language"`
}
