package mgate

import (
	"time"
	"xiao/lobby/messages"
	"xiao/pkg/agent"
	"xiao/pkg/gate"
	"xiao/pkg/gate/wsgate"
	"xiao/pkg/log"
	"xiao/pkg/module/modules"
	"xiao/proto/proto_login"
	"xiao/proto/proto_player"
)

// var A gate.Agent = &Agent{}

const PING_TIME = 10 * time.Second

type Agent struct {
	wsgate.Agent
	modules.BaseAgent
	gate *Gate

	playerId  int64
	playerPID agent.PID
	pingTime  time.Duration
}

func NewAgent(gate *Gate) *Agent {
	a := &Agent{gate: gate}
	return a
}

func (a *Agent) OnInit(gate gate.Gate, session gate.Session) {
	a.BaseAgent.OnInit(a.gate.App)
	a.Agent.OnInit(gate, session)
}

// Message from session
type sessionmsg struct{ msg interface{} }

// Called from session
func (a *Agent) Recv(msg interface{}) {
	log.Debug("* agent recv %v msg: %v", a.GetSession().GetID(), msg)
	// Disable wrap msg in agent
	a.Context.Cast(a.Context.Self(), &sessionmsg{msg: msg})
}

func (a *Agent) Close() {
	a.Context.Stop()
}

func (a *Agent) OnStart(ctx agent.Context) {
	a.BaseAgent.OnStart(ctx)
	log.Debug("* agent %s actor started", a.GetSession().GetID())

	now := time.Now()
	_, zone := now.Zone()

	login := &proto_login.LoginKey{
		SessionId:  a.GetSession().GetID(),
		ServerTime: now.Unix(),
		ServerZone: int32(zone),
	}
	a.Send(login)
	a.pingTime = PING_TIME
}

func (a *Agent) OnStop() {
	log.Debug("* agent %s actor stopped", a.GetSession().GetID())
	a.GetSession().Close()
	a.Invoke("Login", "disconnect", a.playerId)
}

func (a *Agent) OnTick(delta time.Duration) {
	a.pingTime -= delta
	if a.pingTime <= 0 {
	}
}

func (a *Agent) OnMessage(msg interface{}) interface{} {
	// Receive msg from I/O
	switch m := msg.(type) {
	case *sessionmsg:
		a.OnSessionMessage(m.msg)
	case *proto_player.S2CKick:
		a.Send(m)
		a.Close()
	default:
		// 发给客户端
		// log.Debug("gate agent sent to client: %v", m)
		a.Send(m)
	}
	return nil
}

// 处理网关协议
func (a *Agent) OnSessionMessage(msg interface{}) {
	switch m := msg.(type) {
	case *proto_login.LoginRequest:
		result, _ := a.Invoke("Login", "login", &messages.Login{
			Session: a.Context.Self(),
			Request: m,
		})
		loginResult, ok := result.(*messages.LoginResult)
		if ok {
			a.playerId = loginResult.PlayerId
			a.playerPID = loginResult.PlayerPID
			a.Send(loginResult.Response)
			log.Info("session login success, player id %d", a.playerId)
		} else {
			log.Error("session login error")
		}
	case *proto_login.Logout:
		a.Invoke("Login", "logout", a.playerId)
	case *proto_login.Ping:
		a.pingTime = PING_TIME
		a.Send(&proto_login.Pong{
			Time: time.Now().Unix(),
		})
	default:
		// 转给玩家进程
		if a.playerPID != nil {
			a.Context.Cast(a.playerPID, msg)
		} else {
			log.Error("session message error")
		}
	}
}
