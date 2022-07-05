package modules

import (
	"xiao/pkg/module"
)

// var M module.Module = &BaseModule{}

// 基础模块：争对棋块的基本实现
type BaseModule struct {
	// App module.App
	BaseAgent
}

// func (m *BaseModule) Version() string                         { return "0.0.0" }
// func (m *BaseModule) GetType() string                         { return "unknown" }
// func (m *BaseModule) OnInit(app module.App)                   { m.App = app }
// func (m *BaseModule) GetApp() module.App { return m.App }

func (m *BaseModule) OnAppConfigurationLoaded(app module.App) {}
func (m *BaseModule) OnDestroy()                              {}
