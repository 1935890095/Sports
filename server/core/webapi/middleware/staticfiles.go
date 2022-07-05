package middleware

import (
	ghttp "net/http"
	"strings"

	"xiao/core/webapi/http"
	"xiao/pkg/log"
)

func StaticFiles(dir string) http.HttpMiddleware {
	return func(next http.HttpHandler) http.HttpHandler {
		return func(context *http.HttpContext) {
			path := context.Request.GetRequestPath()
			if strings.Contains(path, "static") {
				log.Info("process static file request: %s", path)
				handler := ghttp.FileServer(ghttp.Dir(dir))
				handler.ServeHTTP(context.Response.Raw(), context.Request.Raw())
			} else {
				next(context)
			}
		}
	}
}
