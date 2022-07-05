package models

//	个人邮件信息
type PrivateMail struct {
	MailId        int64           `bson:"_id" json:"_id"`                       // 邮件id
	MailTitle     string          `bson:"mail_title" json:"mail_title"`         // 邮件标题
	TitleParams   []string        `bson:"title_params" json:"title_params"`     // 标题参数
	MailContent   string          `bson:"mail_content" json:"mail_content"`     // 邮件内容
	ContentParams []string        `bson:"content_params" json:"content_params"` // 内容参数
	Accessory     []AccessoryInfo `bson:"accessory" json:"accessory"`           // 附件物品
	CreateTime    int64           `bson:"create_time" json:"create_time"`       // 创建时间
	Expiration    int64           `bson:"expiration" json:"expiration"`         // 超时时间
	PlayerId      int64           `bson:"player_id" json:"player_id"`           // 玩家id
	Sender        string          `bson:"sender" json:"sender"`                 // 发送者
	IsRead        bool            `bson:"is_read" json:"is_read"`               // 是否已读
	IsReceive     bool            `bson:"is_receive" json:"is_receive"`         // 是否接受附件
	IsDelete      bool            `bson:"is_delete" json:"is_delete"`           // 是否删除
}

//	公共邮件信息
type GlobalMail struct {
	MailId        int64           `bson:"_id" json:"_id"`                       // 邮件id
	MailTitle     string          `bson:"mail_title" json:"mail_title"`         // 邮件标题
	TitleParams   []string        `bson:"title_params" json:"title_params"`     // 标题参数
	MailContent   string          `bson:"mail_content" json:"mail_content"`     // 邮件内容
	ContentParams []string        `bson:"content_params" json:"content_params"` // 内容参数
	Sender        string          `bson:"sender," json:"sender,"`               // 发送者
	Accessory     []AccessoryInfo `bson:"accessory" json:"accessory"`           // 附件物品
	Expiration    int64           `bson:"expiration" json:"expiration"`         // 过期时间
	CreateTime    int64           `bson:"create_time" json:"create_time"`       // 创建时间
}

// 邮件附件信息
type AccessoryInfo struct {
	ItemId     int32 `bson:"item_id" json:"item_id"`
	ItemNumber int32 `bson:"item_number" json:"item_number"`
}

type PlayerMail struct {
	ReceiveMails []ReceiveMail `bson:"receive_mails" json:"receive_mails"`
}

type ReceiveMail struct {
	MailId     int64 `bson:"mail_id" json:"mail_id"`
	Expiration int64 `bson:"expiration" json:"expiration"`
}

func (pl *Player) InitMail() {
	if pl.Mail == nil {
		pl.Mail = &PlayerMail{
			ReceiveMails: make([]ReceiveMail, 0),
		}
	}
}
