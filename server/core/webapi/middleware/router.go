package middleware

import (
	"xiao/core/webapi/http"
	"xiao/core/webapi/router"
)

func Router(r *router.Router) http.HttpMiddleware {
	return func(next http.HttpHandler) http.HttpHandler {
		return func(context *http.HttpContext) {
			r.Route(context, next)
		}
	}
}

func RouterWithPrefix(r *router.Router, prefix string) http.HttpMiddleware {
	r.Prefix(prefix)
	return func(next http.HttpHandler) http.HttpHandler {
		return func(context *http.HttpContext) {
			r.Route(context, next)
		}
	}
}
