package middleware

import (
	"xiao/core/webapi/http"
	"xiao/pkg/log"
)

func Recover() http.HttpMiddleware {
	return func(next http.HttpHandler) http.HttpHandler {
		return func(context *http.HttpContext) {
			defer func() {
				if err := recover(); err != nil {
					context.Response.Close()
					log.Error("%s request error %v", context.Request.GetRequestPath(), err)
				}
			}()

			next(context)
		}
	}
}
