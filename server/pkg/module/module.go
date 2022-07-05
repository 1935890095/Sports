package module

import (
	"time"
	"xiao/pkg/agent"
	"xiao/pkg/env"
)

type App interface {
	OnInit() error
	OnDestroy() error
	Run(mods ...Module) error

	GetEnv() *env.Env
	Configure(*env.Env) error
	OnConfigurationLoaded(func(app App)) error
	OnModuleInited(func(app App, module Module)) error
	OnStartup(func(app App)) error
	OnShutdown(func(app App)) error

	GetModule(mod string) Module
	// Cast(mod string, msg interface{})
	// Call(mod string, msg interface{}) (interface{}, error)
	// CallNR(mod string, msg interface{}) error
}

// Module 基本模块定义
type Module interface {
	Agent
	Version() string // 模块版本
	GetType() string // 模块类型
	GetApp() App
	OnAppConfigurationLoaded(app App)
	OnInit(app App)
	OnDestroy()
}

type (
	PID     = agent.PID
	Context = agent.Context
)

//
type Agent interface {
	GetApp() App

	// agent.Agent
	Self() PID
	OnStart(ctx Context)
	OnStop()
	OnTerminated(pid PID, reason int)
	OnMessage(msg interface{}) interface{}
	OnTick(delta time.Duration)
	Cast(mod string, msg interface{})
	Call(mod string, msg interface{}) (interface{}, error)
	CallNR(mod string, msg interface{}) error
	Invoke(mod, fn string, args ...interface{}) (interface{}, error)
	InvokeP(pid PID, fn string, args ...interface{}) (interface{}, error)
}
