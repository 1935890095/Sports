package main

import (
	_ "embed"
	"time"
	"xiao/lobby/announce"
	mgate "xiao/lobby/gate"
	"xiao/lobby/harbor"
	"xiao/lobby/http"
	"xiao/lobby/login"
	"xiao/pkg/app"
	"xiao/pkg/config"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/utils/id"
)

//go:embed env.toml
var env []byte

func main() {
	app := app.NewApp(
		module.WithVersion("1.0.0"),
		module.WithEnv(env),
		module.WithFps(10),
		module.WithKillWaitTTL(3*time.Second),
		module.WithMain(harbor.Module()),
	)
	app.OnConfigurationLoaded(configuration)
	app.OnStartup(startup)
	app.OnShutdown(shutdown)
	app.Run(
		mgate.Module(),
		login.Module(),
		announce.Module(),
		http.Module(),
	)
}

func configuration(app module.App) {
	consul := app.GetEnv().Consul
	config.AddConsulJsonWithFolder(consul.Host, consul.Path, consul.ReloadOnChange)
}

func startup(app module.App) {
	id.Init(uint32(app.GetEnv().ID))

	conf := &mongo.Config{
		Url:             app.GetEnv().Mongo.Url,
		MaxPoolSize:     uint64(app.GetEnv().Mongo.MaxPoolSize),
		MinPoolSize:     uint64(app.GetEnv().Mongo.MinPoolSize),
		MaxConnIdleTime: app.GetEnv().Mongo.MaxConnIdleTime,
	}
	mongo.NewMongo(conf)

	log.Info("* startup")
}

func shutdown(app module.App) {
	mongo.Close()
}
