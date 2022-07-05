package match

import (
	"errors"
	"fmt"
	"time"
	"xiao/pkg/agent"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
	"xiao/proto/proto_node"
)

const PING_TIME = time.Second * 1
const GET_COUNT_INTERVAL = time.Second * 10

var notfound = errors.New("not found")

var Module = func() module.Module {
	return new(Match)
}

type Match struct {
	modules.BaseModule

	active    bool
	pingTime  time.Duration
	remotePID agent.PID

	counts     map[int32]int32
	lastUpdate int64
}

func (m *Match) Version() string { return "1.0.0" }
func (m *Match) GetType() string { return "Match" }

func (m *Match) OnInit(app module.App) {
	m.BaseAgent.OnInit(app)
	m.Register("proxy", m.proxy)
	m.Register("count", m.onGetCount)
}

func (m *Match) OnStart(ctx module.Context) {
	m.BaseAgent.OnStart(ctx)

	env := m.GetApp().GetEnv()
	node := env.Node.Match
	if node == nil {
		log.Error("Match node is nil")
		return
	}

	m.remotePID = agent.NewPID(fmt.Sprintf("%s:%d", node.Host, node.Port), node.Name)
}

func (m *Match) OnStop() {
	m.active = false
}

func (m *Match) OnTerminated(pid agent.PID, reason int) {
	m.active = false
}

func (m *Match) OnTick(delta time.Duration) {
	m.pingTime -= delta
	if m.pingTime <= 0 {
		m.ping()
	}
}

func (m *Match) OnMessage(msg interface{}) interface{} {
	switch msg.(type) {
	case *proto_node.Pong:
		m.active = true
	}

	return nil
}

func (m *Match) ping() {
	m.pingTime = PING_TIME
	if m.remotePID != nil {
		m.Context.Cast(m.remotePID, &proto_node.Ping{
			Address: m.Self().Address,
			Id:      m.Self().Id,
		})
	}
}

func (m *Match) proxy() (agent.PID, error) {
	if m.active {
		return m.remotePID, nil
	} else {
		return nil, notfound
	}
}

func (m *Match) onGetCount(roomId int32) int32 {
	if time.Now().Unix()-m.lastUpdate <= int64(GET_COUNT_INTERVAL.Seconds()) {
		count, ok := m.counts[roomId]
		if ok {
			return count
		} else {
			return 0
		}
	}

	result, err := m.Context.Call(m.remotePID, &proto_node.L2MCountsRequest{})
	if err != nil {
		return 0
	}

	resp, ok := result.(*proto_node.L2MCountsResponse)
	if !ok {
		return 0
	}

	m.counts = make(map[int32]int32)
	for i := 0; i < len(resp.RoomIds); i++ {
		m.counts[resp.RoomIds[i]] = resp.Counts[i]
	}
	m.lastUpdate = time.Now().Unix()
	count, ok := m.counts[roomId]
	if !ok {
		return 0
	}
	return count
}
