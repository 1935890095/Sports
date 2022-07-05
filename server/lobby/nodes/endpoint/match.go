package endpoint

import (
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
)

type Match struct {
	modules.BaseAgent
	Key       string
	Type      string
	Name      string
	Host      string
	remotePid module.PID
	Active    bool
}

func NewMatch(ctx module.Context, name string, host string) *Match {
	ep := &Match{
		Host: host,
		Name: name,
	}
	ctx.Create("match", ep)
	return ep
}

func (ep *Match) OnStart(ctx module.Context) {
	ep.BaseAgent.OnStart(ctx)
	// ep.remotePid = module.NewPID(ep.Host, ep.Name)
}
