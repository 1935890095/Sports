package webapi

import (
	"time"
)

type HttpOption struct {
	Url          string
	ReadTimeout  time.Duration
	WriteTimeout time.Duration
}

func NewHttpOption(url string, readTimeout time.Duration, writeTimeout time.Duration) *HttpOption {
	if readTimeout == 0 {
		readTimeout = time.Second * 5
	}
	if writeTimeout == 0 {
		writeTimeout = time.Second * 5
	}
	return &HttpOption{
		Url:          url,
		ReadTimeout:  readTimeout,
		WriteTimeout: writeTimeout,
	}
}
