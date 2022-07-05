package http

import (
	"bytes"
	"encoding/json"
	ghttp "net/http"

	"xiao/pkg/log"
)

type HttpResponse struct {
	w      ghttp.ResponseWriter
	buffer bytes.Buffer
}

//ResponseParam 响应参数模板
type ResponseParam struct {
	Msg   string      `json:"msg"`
	Error int         `json:"error"`
	Data  interface{} `json:"data"`
}

func (this *HttpResponse) Raw() ghttp.ResponseWriter {
	return this.w
}

func (this *HttpResponse) WriterHeader(statusCode int) {
	if !this.hasContent() {
		this.w.WriteHeader(statusCode)
	}
}

func (this *HttpResponse) Write(data []byte) {
	_, err := this.buffer.Write(data)
	if err != nil {
		log.Error("http response error: %v", err)
		this.w.WriteHeader(ghttp.StatusInternalServerError)
	}
}

func (this *HttpResponse) Close() {
	if this.hasContent() {
		this.buffer.WriteTo(this.w)
		this.buffer.Reset()
	} else {
		this.w.WriteHeader(ghttp.StatusNotFound)
	}
}

func (this *HttpResponse) JsonResult(data interface{}) {
	bytes, err := json.Marshal(data)
	if err != nil {
		log.Error("http response error: %v", err)
		return
	}
	this.Write(bytes)
}

func (this *HttpResponse) JsonSuccess(msg string, data ...interface{}) {
	param := ResponseParam{
		Msg:   msg,
		Error: 0,
	}
	if dataLen := len(data); dataLen == 1 {
		param.Data = data[0]
	} else if dataLen > 1 {
		param.Data = data
	}

	bytes, err := json.Marshal(param)
	if err != nil {
		log.Error("http response error: %v", err)
		return
	}
	this.Write(bytes)
}

func (this *HttpResponse) JsonFail(msg string, data ...interface{}) {
	param := ResponseParam{
		Msg:   msg,
		Error: 1,
	}
	if dataLen := len(data); dataLen == 1 {
		param.Data = data[0]
	} else if dataLen > 1 {
		param.Data = data
	}
	bytes, err := json.Marshal(param)
	if err != nil {
		log.Error("http response error: %v", err)
		return
	}
	this.Write(bytes)
}

func (this *HttpResponse) hasContent() bool {
	return this.buffer.Len() > 0
}

func NewHttpResponse(w ghttp.ResponseWriter) HttpResponse {
	return HttpResponse{
		w: w,
	}
}
