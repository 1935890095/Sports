package mail

import (
	"sync"
	"time"
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/bag"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/pkg/utils/id"
)

const (
	PRIVATE_EXPRIATION = 86400 * 30
	GLOBAL_EXPRIATION  = 86400 * 30 * 3
)

type Header struct {
	mailId     int64 // 邮件id
	createTime int64 // 邮件创建时间
	expiration int64 // 过期时间
}

var locker sync.Mutex
var globalMailHeaders []Header

//加载所有未过期的公共邮件
func loadAllGlobalMail() {
	DB := mongo.NewIQ("game", "global_mail")

	now := time.Now().Unix()
	var mailList []models.GlobalMail

	if err := DB.Find(&mailList).Error(); err != nil {
		log.Error("load public mail error")
		return
	}

	for _, mail := range mailList {
		if mail.Expiration == 0 || (mail.Expiration != 0 && now <= mail.Expiration) { // ExpirationTime为0不过期
			globalMailHeaders = append(globalMailHeaders, Header{
				mailId:     mail.MailId,
				expiration: mail.Expiration,
				createTime: mail.CreateTime,
			})
		}
	}
}

// 获取所有私人邮件
func getPrivateMail(plObj *models.Player) []models.PrivateMail {
	DB := mongo.NewIQ("game", "private_mail")
	DB.Where("player_id", plObj.ID).Where("is_delete", false)
	var mailList []models.PrivateMail
	if err := DB.Find(&mailList).Error(); err != nil {
		log.Error("get private mail error:", err)
		return nil
	}

	now := time.Now().Unix()
	list := make([]models.PrivateMail, 0)
	for _, mail := range mailList {
		if mail.Expiration != 0 && now > mail.Expiration {
			deleteMail(plObj, mail.MailId)
		} else {
			list = append(list, mail)
		}
	}
	return list
}

//领取全服邮件，如果在玩家账号创建之前的邮件则不领取
func receiveGlobalMail(ctx context.Player, plObj *models.Player) []models.PrivateMail {
	if globalMailHeaders == nil {
		loadAllGlobalMail()
	}

	receiveMap := make(map[int64]int)
	now := time.Now().Unix()
	newReceiveList := make([]models.ReceiveMail, 0)
	// 玩家领取不过期的公共邮件 也不删除 只删除过期了的
	for _, info := range plObj.Mail.ReceiveMails {
		if now < info.Expiration || info.Expiration == 0 {
			newReceiveList = append(newReceiveList, info)
			receiveMap[info.MailId] = 1
		}
	}
	addList := make([]int64, 0)
	for _, header := range globalMailHeaders {
		_, ok := receiveMap[header.mailId]
		if !ok && (now <= header.expiration || header.expiration == 0) && plObj.Base.CreateTime <= header.createTime {
			addList = append(addList, header.mailId)
			newReceiveList = append(newReceiveList, models.ReceiveMail{MailId: header.mailId, Expiration: header.expiration})
		}
	}

	addPublicMails := getMailFromGlobal(addList)
	createMails := make([]models.PrivateMail, 0)

	for _, addMail := range addPublicMails {
		mailId, _ := id.General()
		newMail := models.PrivateMail{
			MailId:        mailId,
			MailTitle:     addMail.MailTitle,
			TitleParams:   addMail.TitleParams,
			MailContent:   addMail.MailContent,
			ContentParams: addMail.ContentParams,
			Sender:        addMail.Sender,
			Accessory:     addMail.Accessory,
			CreateTime:    addMail.CreateTime,
			PlayerId:      plObj.ID,
		}
		if len(addMail.Accessory) <= 0 {
			newMail.Expiration = now + PRIVATE_EXPRIATION
		}
		createMails = append(createMails, newMail)
	}

	DB := mongo.NewIQ("game", "private_mail")
	if len(createMails) > 0 {
		if err := DB.Insert(createMails).Error(); err != nil {
			log.Error("recive public mail error:", err)
		}
	}

	plObj.Mail.ReceiveMails = newReceiveList
	ctx.SaveNow()
	return createMails
}

// 获取个人邮件详细信息
func getMailFromPrivate(mailId int64) (models.PrivateMail, bool) {
	DB := mongo.NewIQ("game", "private_mail")

	mailInfo := models.PrivateMail{}
	if err := DB.Where("_id", mailId).Find(&mailInfo).Error(); err != nil {
		return mailInfo, false
	}

	return mailInfo, mailInfo.MailId != 0
}

// 获取公共邮件详细信息
func getMailFromGlobal(mailList []int64) []models.GlobalMail {
	DB := mongo.NewIQ("game", "global_mail")

	DB.WhereIn("_id", mailList)
	var list []models.GlobalMail
	if err := DB.Find(&list).Error(); err != nil {
		return nil
	}

	return list
}

// 设置邮件为已读
func setMailToRead(mailId, playerId int64) {
	DB := mongo.NewIQ("game", "private_mail")

	var mail models.PrivateMail
	err := DB.Where("_id", mailId).Find(&mail).Error()
	if err != nil || mail.PlayerId != playerId {
		return
	}

	mail.IsRead = true
	mongo.NewIQ("game", "private_mail").Where("_id", mailId).Update(mail)
}

// 领取邮件附件
func receiveMailAward(ctx context.Player, plObj *models.Player, mailId int64) ([]models.AccessoryInfo, bool) {
	DB := mongo.NewIQ("game", "private_mail")

	var mail models.PrivateMail
	if err := DB.Where("_id", mailId).Find(&mail).Error(); err != nil {
		return nil, false
	}
	if mail.PlayerId == plObj.ID {
		if len(mail.Accessory) >= 1 && !mail.IsReceive {
			mail.IsReceive = true
			DB = mongo.NewIQ("game", "private_mail")
			if err := DB.Where("_id", mailId).Update(mail).Error(); err != nil {
				return nil, false
			}

			mailReward(ctx, plObj, mail.Accessory)
			return mail.Accessory, true
		}
	}

	return nil, false
}

//删除邮件
func deleteMail(plObj *models.Player, mailId int64) bool {
	var mail models.PrivateMail
	DB := mongo.NewIQ("game", "private_mail")

	if err := DB.Where("_id", mailId).Where("player_id", plObj.ID).Find(&mail).Error(); err != nil {
		log.Error("delete private mail error :", err)
		return false
	}

	if mail.MailId == 0 {
		log.Info("delete mail: no mail")
		return false
	}

	if mail.IsDelete {
		log.Info("delete mail: mail has delete")
		return false
	}

	// 如果有附件不能删除
	if len(mail.Accessory) > 0 && !mail.IsReceive {
		return false
	}

	mail.IsDelete = true
	if err := mongo.NewIQ("game", "private_mail").Where("_id", mail.MailId).Update(mail).Error(); err != nil {
		log.Error("update mail delete tag error :", err)
		return false
	}

	return true
}

//发送单人邮件  // 创建一个过期时间
func SendPrivateMail(title string, content string, tparams []string,
	cparams []string, accessorys []models.AccessoryInfo, playerId int64, sender string) error {
	mailId, _ := id.General()
	now := time.Now().Unix()

	mail := models.PrivateMail{
		MailId:      mailId,
		MailTitle:   title,
		MailContent: content,
		Accessory:   accessorys,
		CreateTime:  now,
		PlayerId:    playerId,
		Sender:      sender,
	}

	if len(accessorys) <= 0 {
		mail.Expiration = now + PRIVATE_EXPRIATION
	}

	DB := mongo.NewIQ("game", "private_mail")

	if err := DB.Insert(mail).Error(); err != nil {
		log.Error("insert into private mail error : ", err)
		return err
	}

	return nil
}

// 发送公共邮件
func SendGlobalMail(title string, content string, sender string, effectiveTime int64,
	tparams []string, cparams []string, accessorys []models.AccessoryInfo) error {
	mailId, _ := id.General()
	now := time.Now().Unix()

	if effectiveTime == 0 {
		effectiveTime = GLOBAL_EXPRIATION
	}

	var expiration int64
	if effectiveTime != 0 && len(accessorys) <= 0 {
		expiration = now + effectiveTime
	}
	mailInfo := models.GlobalMail{
		MailId:        mailId,
		MailTitle:     title,
		TitleParams:   tparams,
		MailContent:   content,
		ContentParams: cparams,
		Sender:        sender,
		Accessory:     accessorys,
		CreateTime:    now,
		Expiration:    expiration,
	}

	DB := mongo.NewIQ("game", "global_mail")

	if err := DB.Insert(&mailInfo).Error(); err != nil {
		log.Error("insert into public mail error : ", err)
		return err
	}

	newHeaders := make([]Header, 0)
	for _, m := range globalMailHeaders {
		if m.expiration == 0 || now < m.expiration {
			newHeaders = append(newHeaders, m)
		}
	}

	locker.Lock()
	globalMailHeaders = append(newHeaders, Header{mailId: mailInfo.MailId, createTime: mailInfo.CreateTime, expiration: expiration})
	locker.Unlock()
	return nil
}

func mailReward(ctx context.Player, plObj *models.Player, accessorys []models.AccessoryInfo) {
	for _, accessory := range accessorys {
		bag.AddItem(ctx, plObj, accessory.ItemId, accessory.ItemNumber, define.ItemEventType_Mail)
	}
}
