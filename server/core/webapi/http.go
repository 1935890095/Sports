package webapi

import (
	"context"
	ghttp "net/http"

	"xiao/core/webapi/http"
	"xiao/pkg/log"
)

type HttpServer interface {
	Use(...http.HttpMiddleware)
	Stop(ctx context.Context)
	Start(ctx context.Context)
}

type server struct {
	middlewares []http.HttpMiddleware
	handler     http.HttpHandler
	server      *ghttp.Server
	option      *HttpOption
}

func NewHttpServer(option *HttpOption) HttpServer {
	return &server{
		option: option,
	}
}

func (s *server) Use(middlewares ...http.HttpMiddleware) {
	s.middlewares = append(s.middlewares, middlewares...)
}

func (s *server) build() {
	for i, j := 0, len(s.middlewares)-1; i < j; i, j = i+1, j-1 {
		s.middlewares[i], s.middlewares[j] = s.middlewares[j], s.middlewares[i]
	}

	next := func(context *http.HttpContext) {
		context.Response.Close()
	}
	for i := 0; i < len(s.middlewares); i++ {
		next = s.middlewares[i](next)
	}
	s.handler = next
}

func (s *server) Start(ctx context.Context) {
	if s.option == nil {
		log.Error("http server listen urls is nil")
		return
	}

	s.build()
	log.Info("http server start")
	go func() {
		server := &ghttp.Server{
			Addr:         s.option.Url,
			ReadTimeout:  s.option.ReadTimeout,
			WriteTimeout: s.option.WriteTimeout,
			Handler: ghttp.HandlerFunc(func(w ghttp.ResponseWriter, r *ghttp.Request) {
				s.handler(&http.HttpContext{
					Request:  http.NewHttpRequest(r),
					Response: http.NewHttpResponse(w),
				})
			}),
		}

		s.server = server
		log.Info("http server listen at %s", s.option.Url)
		if err := server.ListenAndServe(); err != nil {
			log.Error("http server start error: %v", err)
		}
	}()

}

func (s *server) Stop(ctx context.Context) {
	if err := s.server.Close(); err != nil {
		log.Error("http server close err:", err)
	}
}
