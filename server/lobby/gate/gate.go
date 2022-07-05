package mgate

import (
	"xiao/pkg/gate"
	"xiao/pkg/gate/wsgate"
	"xiao/pkg/module"
)

var Module = func() module.Module {
	gate := new(Gate)
	gate.SetCreateAgent(gate.CreateAgent)
	return gate
}

type Gate struct {
	wsgate.Gate
}

func (gt *Gate) GetType() string { return "Gate" }
func (gt *Gate) Version() string { return "1.0.0" }

func (gt *Gate) CreateAgent(gate gate.Gate, session gate.Session) (gate.Agent, error) {
	agent := NewAgent(gt)

	agent.OnInit(gate, session)
	_, err := gt.Context.Create("session#"+session.GetID(), agent)
	if err != nil {
		return nil, err
	}

	return agent, nil
}
