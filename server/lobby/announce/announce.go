package announce

import (
	"time"
	"xiao/core/models"
	"xiao/lobby/invoke"
	"xiao/lobby/messages"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
	"xiao/pkg/utils/id"
	"xiao/proto/proto_player"

	"go.mongodb.org/mongo-driver/bson"
)

const default_tick_time = 10 * 10

var Module = func() module.Module {
	announce := new(Announce)
	return announce
}

type Announce struct {
	modules.BaseModule
	announces []*models.Announcement
	notices   []*models.NoticeContent
	tickTime  int64
}

func (an *Announce) OnInit(app module.App) {
	an.BaseModule.OnInit(app)
	an.loadAnnounce()
	an.tickTime = default_tick_time
	an.Register("newAnnouncement", an.newAnnouncement)
	an.Register("getAnnouncement", an.getAnnouncement)
	an.Register("newNotice", an.newNotice)
}

func (announce *Announce) Version() string { return "1.0.0" }
func (announce *Announce) GetType() string { return "Announce" }

func (an *Announce) OnMessage(msg interface{}) interface{} {
	switch msg := msg.(type) {
	case *messages.GetAnnouncement:
		return an.getAnnouncement()
	case *messages.NewAnnouncement:
		an.newAnnouncement(msg)
	case *messages.NewNotice:
		an.newNotice(msg)
	default:
		break
	}

	return nil
}

func (an *Announce) newAnnouncement(msg *messages.NewAnnouncement) {
	id, _ := id.General()
	announce := &models.Announcement{
		Id:        id,
		Type:      msg.Type,
		OpenTime:  msg.OpenTime,
		CloseTime: msg.CloseTime,
		Priority:  msg.Priority,
		Channel:   msg.Channel,
		Count:     msg.Count,
		Title:     msg.Title,
		Content:   msg.Content,
		Version:   msg.Version,
		Language:  msg.Language,
	}
	an.announces = append(an.announces, announce)
	if err := mongo.NewIQ("game", "announce").Insert(announce).Error(); err != nil {
		log.Info("insert announce err:", err)
		return
	}

	invoke.Boardcast(an, &proto_player.S2CAnnouncement{
		Datas: []*proto_player.Announcement{
			{
				Id:        announce.Id,
				Type:      announce.Type,
				OpenTime:  announce.OpenTime,
				CloseTime: announce.CloseTime,
				Priority:  announce.Priority,
				Channel:   announce.Channel,
				Count:     announce.Count,
				Title:     announce.Title,
				Content:   announce.Content,
				Version:   announce.Version,
				Language:  announce.Language,
			},
		},
	})
}

func (an *Announce) getAnnouncement() []*models.Announcement {
	datas := make([]*models.Announcement, 0)
	now := time.Now().Unix()
	newAnnounce := make([]*models.Announcement, 0)
	for i := 0; i <= len(an.announces)-1; i++ {
		if now >= an.announces[i].CloseTime {
			// an.announces = append(an.announces[:i], an.announces[i+1:]...)
			continue
		}
		newAnnounce = append(newAnnounce, an.announces[i])
		datas = append(datas, an.announces[i])
	}

	an.announces = newAnnounce
	return datas
}

func (an *Announce) loadAnnounce() {
	now := time.Now().Unix()
	announces := make([]*models.Announcement, 0)
	if err := mongo.NewIQ("game", "announce").Where("close_time", bson.M{"$gt": now}).Find(&announces).Error(); err != nil {
		log.Error("loadAnnounce error %v", err)
		return
	}

	an.announces = announces
}

// 走马灯
func (an *Announce) newNotice(msg *messages.NewNotice) {
	notice := &models.NoticeContent{
		Type:     msg.Type,
		Priority: msg.Priority,
		Times:    msg.Times,
		Timespan: msg.Timespan,
		Opentime: msg.Opentime,
		Content:  msg.Content,
		Params:   msg.Params,
	}

	if msg.Opentime == 0 {
		an.boardcastNotice(notice)
	} else {
		an.notices = append(an.notices, notice)
	}
}

func (an *Announce) OnTick(delta time.Duration) {
	if an.tickTime >= 0 {
		an.tickTime--
	} else {
		now := time.Now().Unix()
		newNotices := make([]*models.NoticeContent, 0)
		for _, notice := range an.notices {
			if now >= notice.Opentime {
				an.boardcastNotice(notice)
				// an.notices = append(an.notices[:index], an.notices[index+1:]...)
			} else {
				newNotices = append(newNotices, notice)
			}
		}
		an.notices = newNotices
		an.tickTime = default_tick_time
	}
}

func (an *Announce) boardcastNotice(notice *models.NoticeContent) {
	msg := &proto_player.S2CNotice{
		Type:     notice.Type,
		Priority: notice.Priority,
		Times:    notice.Times,
		TimeSpan: notice.Timespan,
		Content:  notice.Content,
		Params:   notice.Params,
	}
	invoke.Boardcast(an, msg)
}
