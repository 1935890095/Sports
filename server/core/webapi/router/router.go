package router

import (
	"strings"

	"xiao/core/webapi/http"
)

const (
	get  = "GET"
	post = "POST"
)

type Router struct {
	prefix       string
	getHandlers  map[string]http.HttpHandler
	postHandlers map[string]http.HttpHandler
}

func NewRouter() *Router {
	return &Router{
		getHandlers:  make(map[string]http.HttpHandler),
		postHandlers: make(map[string]http.HttpHandler),
	}
}

func (router *Router) Prefix(prefix string) {
	if !strings.HasPrefix(prefix, "/") {
		prefix = "/" + prefix
	}
	router.prefix = prefix
}

func (router *Router) Get(path string, handler http.HttpHandler) {
	router.getHandlers[router.prefix+path] = handler
}

func (router *Router) Post(path string, handler http.HttpHandler) {
	router.postHandlers[router.prefix+path] = handler
}

func (router *Router) Route(context *http.HttpContext, next http.HttpHandler) {
	handle := router.getHandler(context)
	if handle != nil {
		handle(context)
		context.Response.Close()
	} else {
		next(context)
	}
}

func (router *Router) getHandler(context *http.HttpContext) http.HttpHandler {
	path := context.Request.GetRequestPath()
	method := context.Request.Method()
	var handlers map[string]http.HttpHandler
	if method == get {
		handlers = router.getHandlers
	} else if method == post {
		handlers = router.postHandlers
	} else {
		return nil
	}
	if handler, ok := handlers[path]; ok {
		return handler
	}
	return nil
}
