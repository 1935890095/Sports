package context

import (
	"xiao/pkg/agent"
)

type Player interface {
	Self() agent.PID
	Cast(pid agent.PID, msg interface{})
	Call(pid agent.PID, msg interface{}) (interface{}, error)
	Invoke(mod, fn string, args ...interface{}) (interface{}, error)
	InvokeP(pid agent.PID, fn string, args ...interface{}) (interface{}, error)
	Send(msg interface{})
	Watch(pid agent.PID)
	Unwatch(pid agent.PID)

	SavePlayer()
	SaveNow()
	Stop()
}
