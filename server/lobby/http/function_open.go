package http

import (
	"xiao/core/models"
	"xiao/core/webapi/http"
	"xiao/lobby/invoke"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
)

type FunctionMessage struct {
	Id    int `json:"id"`
	State int `json:"state"`
}

func (http *HttpModule) functionOpen(context *http.HttpContext) {
	msg := &models.FunctionOpen{}
	if err := context.Request.Form(msg); err != nil {
		log.Debug("get form data err:", err)
		return
	}

	if err := mongo.NewIQ("game", "openconfig").Where("_id", msg.Id).Delete().Insert(msg).Error(); err != nil {
		log.Debug("update openconfig db err:", err)
		return
	}

	invoke.CastAgents(http, msg)
	context.Response.Write([]byte("success..."))
}
