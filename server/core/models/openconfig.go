package models

type FunctionOpen struct {
	Id    int `json:"id" bson:"_id"`
	State int `json:"state" bson:"state"`
}
