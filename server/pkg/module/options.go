package module

import (
	"time"
)

type Option func(*Options)

type Options struct {
	Version     string
	Debug       bool
	Env         []byte
	ConfPath    string
	WorkDir     string
	LogDir      string
	ID          int
	Name        string
	Host        string
	Port        int
	Main        Module
	Fps         int
	KillWaitTTL time.Duration
}

func WithVersion(v string) Option {
	return func(o *Options) {
		o.Version = v
	}
}

func WithDebug(v bool) Option {
	return func(o *Options) {
		o.Debug = v
	}
}

func WithWorkDir(v string) Option {
	return func(o *Options) {
		o.WorkDir = v
	}
}

func WithConfPath(path string) Option {
	return func(o *Options) {
		o.ConfPath = path
	}
}

func WithName(name string) Option {
	return func(o *Options) {
		o.Name = name
	}
}

func WithHost(v string) Option {
	return func(o *Options) {
		o.Host = v
	}
}

func WithPort(v int) Option {
	return func(o *Options) {
		o.Port = v
	}
}

func WithKillWaitTTL(v time.Duration) Option {
	return func(o *Options) {
		o.KillWaitTTL = v
	}
}

func WithMain(main Module) Option {
	return func(o *Options) {
		o.Main = main
	}
}

func WithFps(fps int) Option {
	return func(o *Options) {
		o.Fps = fps
	}
}

func WithEnv(env []byte) Option {
	return func(o *Options) {
		o.Env = env
	}
}
