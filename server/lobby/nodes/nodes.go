package nodes

import (
	"fmt"
	"xiao/lobby/nodes/endpoint"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
)

var Module = func() module.Module {
	nodes := new(Nodes)
	return nodes
}

type Nodes struct {
	modules.BaseModule
	matcher *endpoint.Match
}

func (m *Nodes) Version() string { return "1.0.0" }
func (m *Nodes) GetType() string { return "Endpoint" }

func (m *Nodes) OnInit(app module.App) {
	m.BaseModule.OnInit(app)
}

type HelloWorld struct {
	Say string
}

func (m *Nodes) OnStart(ctx module.Context) {
	m.BaseModule.OnStart(ctx)
	// m.Context.Call(m.matchPid, &HelloWorld{Say: "hello world"})

	v := m.App.GetEnv().Get("Node.Match")
	if vv, ok := v.([]interface{}); ok {
		for _, vvv := range vv {
			if info, ok := vvv.(map[string]interface{}); ok {
				name := info["Name"].(string)
				host := info["Host"].(string)
				port := info["Port"].(int64)
				log.Debug("* ID  =  %v\n", info["ID"])
				log.Debug("* Name =  %v\n", info["Name"])
				log.Debug("* Host =  %v\n", info["Host"])
				log.Debug("* Port =  %v\n", info["Port"])
				m.matcher = endpoint.NewMatch(m.Context, name, fmt.Sprintf("%s:%d", host, port))
				m.matcher.OnInit(m.App)
			}
		}
	}
}
