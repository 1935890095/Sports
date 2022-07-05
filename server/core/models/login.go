package models

// 登录表
type Login struct {
	// LoginCode     string `bson:"_id" json:"id"`
	Account       string `bson:"_id" json:"_id"`
	Password      string `bson:"password" json:"password"`
	UserId        int64  `bson:"UserId" json:"UserId"`
	Type          int32  `bson:"Type" json:"Type"`
	Channel       int32  `bjson:"Channel" json:"Channel"`
	LastLoginTime int64  `bson:"LastLoginTime" json:"LastLoginTime"`
}
