package invoke

import (
	"xiao/lobby/messages"
)

type Invoker interface {
	Invoke(mod, fn string, args ...interface{}) (interface{}, error)
}

func CastAgent(m Invoker, playerId int64, msg interface{}) {
	m.Invoke("Login", "castAgent", playerId, &messages.SysMessage{
		Content: msg,
	})
}

func CastAgents(m Invoker, msg interface{}) {
	m.Invoke("Login", "castAgent", int64(0), &messages.SysMessage{
		Content: msg,
	})
}

func Dispatch(m Invoker, playerId int64, msg interface{}) {
	m.Invoke("Login", "castAgent", playerId, &messages.DispatchMessage{
		Content: msg,
	})
}

func Boardcast(m Invoker, msg interface{}) {
	m.Invoke("Login", "castAgent", int64(0), &messages.DispatchMessage{
		Content: msg,
	})
}
