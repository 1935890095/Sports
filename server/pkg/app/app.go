package app

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"xiao/pkg/agent"
	"xiao/pkg/env"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
)

func NewApp(opts ...module.Option) module.App {
	opt := module.Options{}
	for _, o := range opts {
		o(&opt)
	}
	app := new(DefaultApp)
	app.opts = opt
	return app
}

type DefaultApp struct {
	//module.App
	opts                module.Options
	env                 *env.Env
	startup             func(app module.App)
	shutdown            func(app module.App)
	configurationLoaded func(app module.App)
	system              *agent.System
	manager             *modules.ModuleManager
}

func (app *DefaultApp) OnInit() error { return nil }

func (app *DefaultApp) OnDestroy() error { return nil }

func (app *DefaultApp) Run(mods ...module.Module) error {
	e, err := env.LoadEnv(app.opts.Env)
	if err != nil {
		panic(fmt.Sprintf("load env error %v", err))
	}
	app.Configure(e)
	log.Init(app.env)

	if app.configurationLoaded != nil {
		app.configurationLoaded(app)
	}

	log.Info("app %v starting up", app.opts.Version)

	var tick int64
	if app.opts.Fps > 0 {
		tick = int64(time.Second) / int64(app.opts.Fps)
	}

	app.system = agent.NewSystem(
		agent.WithName(app.env.Name),
		agent.WithHost(app.env.Host),
		agent.WithPort(app.env.Port),
		agent.WithAgent(app.opts.Main),
		agent.WithTick(time.Duration(tick)),
	)
	app.system.Start()

	// modules
	manager := modules.NewModuleManager()

	if app.opts.Main != nil {
		// 将主模块加进来
		app.opts.Main.OnAppConfigurationLoaded(app)
		manager.Register(app.opts.Main)
	}

	for i := 0; i < len(mods); i++ {
		mods[i].OnAppConfigurationLoaded(app)
		manager.Register(mods[i])
	}

	app.OnInit()
	if app.startup != nil {
		app.startup(app)
	}
	manager.Init(app, app.system, app.opts.Main)
	app.manager = manager

	// close
	c := make(chan os.Signal, 1)
	signal.Notify(c, os.Interrupt, syscall.SIGINT, syscall.SIGTERM)
	sig := <-c

	// 一定时间内关不了则强制关闭
	timeout := time.NewTimer(app.opts.KillWaitTTL)
	wait := make(chan struct{})
	go func() {
		if app.shutdown != nil {
			app.shutdown(app)
		}
		manager.Destroy(app.system)
		app.system.Stop()
		app.OnDestroy()
		wait <- struct{}{}
	}()
	select {
	case <-timeout.C:
		panic(fmt.Sprintf("app close timeout (signal: %v)", sig))
	case <-wait:
		fmt.Printf("app closing down (signal: %v)\n", sig)
	}
	return nil
}

func (app *DefaultApp) Configure(env *env.Env) error {
	app.env = env
	if env.ConfPath != "" {
		app.opts.ConfPath = env.ConfPath
	}
	if env.Name != "" {
		app.opts.Name = env.Name
	}
	if env.Host != "" {
		app.opts.Host = env.Host
		app.opts.Port = env.Port
	}
	if env.ID != 0 {
		app.opts.ID = env.ID
	}
	if env.Fps != 0 {
		app.opts.Fps = env.Fps
	}
	app.opts.Debug = env.Debug
	return nil
}

// func (app *DefaultApp) Cast(mod string, msg interface{}) {
// 	if pid := app.manager.Get(mod).Self(); pid != nil {
// 		app.system.Cast(pid, msg)
// 	}
// }

// func (app *DefaultApp) Call(mod string, msg interface{}) (interface{}, error) {
// 	if pid := app.manager.Get(mod).Self(); pid != nil {
// 		return app.system.Call(pid, msg)
// 	} else {
// 		return nil, nil
// 	}
// }

// func (app *DefaultApp) CallNR(mod string, msg interface{}) error {
// 	if pid := app.manager.Get(mod).Self(); pid != nil {
// 		return app.system.CallNR(pid, msg)
// 	} else {
// 		return nil
// 	}
// }

func (app *DefaultApp) GetModule(mod string) module.Module {
	return app.manager.Get(mod)
}

func (app *DefaultApp) GetEnv() *env.Env {
	return app.env
}

func (app *DefaultApp) OnConfigurationLoaded(_func func(app module.App)) error {
	app.configurationLoaded = _func
	return nil
}

func (app *DefaultApp) OnModuleInited(func(app module.App, module module.Module)) error {
	// TODO
	return nil
}

func (app *DefaultApp) OnStartup(_func func(app module.App)) error {
	app.startup = _func
	return nil
}

func (app *DefaultApp) OnShutdown(_func func(app module.App)) error {
	app.shutdown = _func
	return nil
}
