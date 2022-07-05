package mail

import (
	"sort"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/resource"
	"xiao/proto/proto_player"
)

// 获取邮件列表
func GetMailList(ctx context.Player, plObj *models.Player, req *proto_player.C2SGetMailList) {
	plObj.InitMail()
	ret := &proto_player.S2CGetMailList{}
	privateMails := getPrivateMail(plObj)
	receiveMails := receiveGlobalMail(ctx, plObj)
	privateMails = append(privateMails, receiveMails...)

	sort.Stable(mailSort(privateMails))

	if len(privateMails) >= int(req.Start+req.Count) {
		privateMails = privateMails[req.Start : req.Start+req.Count]
	} else if len(privateMails) >= int(req.Start) {
		privateMails = privateMails[req.Start:]
	} else {
		return
	}

	mails := make([]*proto_player.Mail, 0)
	for _, mail := range privateMails {
		var award *proto_player.AccessoryInfo
		if len(mail.Accessory) > 0 {
			award = &proto_player.AccessoryInfo{
				ItemId:     mail.Accessory[0].ItemId,
				ItemNumber: mail.Accessory[0].ItemNumber,
			}
		}
		mails = append(mails, &proto_player.Mail{
			Id:             mail.MailId,
			Title:          mail.MailTitle,
			IsRead:         mail.IsRead,
			IsReceiveAward: mail.IsReceive,
			CreateTime:     mail.CreateTime,
			Sender:         mail.Sender,
			WithAward:      len(mail.Accessory) > 0,
			Award:          award,
		})
	}

	ret.MailList = mails
	ctx.Send(ret)
}

//获取邮件详细信息
func GetMailDetail(ctx context.Player, plObj *models.Player, req *proto_player.C2SGetMailDetail) {
	ret := &proto_player.S2CGetMailDetail{}
	if mail, ok := getMailFromPrivate(req.Id); ok {
		if mail.PlayerId == plObj.ID {
			setMailToRead(mail.MailId, plObj.ID)

			accessory := []*proto_player.AccessoryInfo{}
			for _, award := range mail.Accessory {
				accessory = append(accessory, &proto_player.AccessoryInfo{ItemId: award.ItemId,
					ItemNumber: award.ItemNumber})
			}

			ret.MailTitle = mail.MailTitle
			ret.MailContent = mail.MailContent
			ret.Id = mail.MailId
			ret.IsReceiveAward = mail.IsReceive
			ret.Accessory = accessory
		}
	}

	ctx.Send(ret)
}

// 领取附件邮件
func ReceiveMailAward(ctx context.Player, plObj *models.Player, req *proto_player.C2SReceiveMailAward) {
	ret := &proto_player.S2CReceiveMailAward{Access: false}

	if _, ok := receiveMailAward(ctx, plObj, req.Id); ok {
		ret.Access = true
		// var ssAccessories []string
		// for _, accessory := range reward {
		// 	ssAccessories = append(ssAccessories, fmt.Sprintf("%v", accessory.ItemId))
		// 	ssAccessories = append(ssAccessories, fmt.Sprintf("%v", accessory.ItemNumber))
		// }
		// TODO: 数数埋点
	}

	ctx.Send(ret)

	resource.Sync(ctx, plObj)
}

// 删除邮件
func DeleteMail(ctx context.Player, plObj *models.Player, req *proto_player.C2SDeleteMail) {
	success := deleteMail(plObj, req.Id)

	ret := &proto_player.S2CDeleteMail{Access: success}
	ctx.Send(ret)
}
