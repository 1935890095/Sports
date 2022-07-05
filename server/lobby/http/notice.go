package http

import (
	"time"
	"xiao/core/webapi/http"
	"xiao/lobby/messages"
)

func (http *HttpModule) newNotice(context *http.HttpContext) {
	http.Cast("Announce", &messages.NewNotice{
		Type:     3,
		Priority: 2,
		Opentime: time.Now().Unix(),
		Times:    10,
		Timespan: 3,
		Content:  "test content",
		Params:   nil,
	})
	context.Response.Write([]byte("success..."))
}
