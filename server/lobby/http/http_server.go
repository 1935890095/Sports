package http

import (
	"context"
	"xiao/core/webapi"
	"xiao/core/webapi/middleware"
	"xiao/core/webapi/router"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
)

var Module = func() module.Module {
	httpModule := new(HttpModule)
	return httpModule
}

type HttpModule struct {
	modules.BaseModule
	router *router.Router
	server webapi.HttpServer
}

func (http *HttpModule) Version() string { return "1.0.0" }

func (http *HttpModule) GetType() string { return "HttpModule" }

func (http *HttpModule) OnInit(app module.App) {
	http.BaseModule.OnInit(app)
	http.router = router.NewRouter()
	http.router.Prefix("api")
	http.startHttpServer(app)
}

func (http *HttpModule) OnStop() {
	http.server.Stop(context.Background())
}

func (http *HttpModule) startHttpServer(app module.App) {
	http.register()

	http.server = webapi.NewHttpServer(webapi.NewHttpOption(app.GetEnv().HttpUrl, 0, 0))
	http.server.Use(middleware.Router(http.router), middleware.Recover())

	http.server.Start(context.Background())
}
