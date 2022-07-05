package http

import (
	"encoding/json"
	"time"
	"xiao/core/models"
	"xiao/core/webapi/http"
	"xiao/lobby/messages"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func (http *HttpModule) getAnnounce(context *http.HttpContext) {
	list := &proto_player.S2CAnnouncement{
		Datas: make([]*proto_player.Announcement, 0),
	}

	result, err := http.Call("Announce", &messages.GetAnnouncement{})
	if err != nil || result == nil {
		log.Info("call announc error:", err)
		bytes, _ := json.Marshal(list)
		context.Response.Write(bytes)
	}

	announcements := result.([]*models.Announcement)
	for _, v := range announcements {
		list.Datas = append(list.Datas, &proto_player.Announcement{
			Id:        v.Id,
			Type:      v.Type,
			OpenTime:  v.OpenTime,
			CloseTime: v.CloseTime,
			Count:     v.Count,
			Priority:  v.Priority,
			Channel:   v.Channel,
			Title:     v.Title,
			Content:   v.Content,
			Version:   v.Version,
			Language:  v.Language,
		})
	}

	bytes, _ := json.Marshal(list)
	context.Response.Write(bytes)
}

func (http *HttpModule) newAnnounce(context *http.HttpContext) {
	http.Cast("Announce", &messages.NewAnnouncement{
		Type:      3,
		Priority:  2,
		Count:     10,
		Title:     "test title",
		Content:   "test content",
		OpenTime:  time.Now().Unix(),
		CloseTime: time.Now().Unix() + 36000,
	})
	context.Response.Write([]byte("success..."))
}
