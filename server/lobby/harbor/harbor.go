package harbor

import (
	"time"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
)

// Harbor模块

var Module = func() module.Module {
	harbor := new(Harbor)
	return harbor
}

type Harbor struct {
	modules.BaseModule
}

func (m *Harbor) Version() string            { return "1.0.0" }
func (m *Harbor) GetType() string            { return "Harbor" } //模块类型
func (m *Harbor) OnTick(delta time.Duration) {}

func (m *Harbor) OnMessage(msg interface{}) interface{} {
	log.Debug("###### harbor receive msg: %v", msg)
	return nil
}
