package tony

import (
	"strings"
	"time"
	"xiao/pkg/codec"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
	"xiao/pkg/serialize"
	"xiao/proto/proto_login"

	"github.com/gorilla/websocket"
)

var Module = func() module.Module {
	login := new(Tony)
	return login
}

type Tony struct {
	modules.BaseModule

	tick1 time.Duration
}

func (m *Tony) Version() string { return "1.0.0" }
func (m *Tony) GetType() string { return "Tony" }

func (m *Tony) OnMessage(msg interface{}) interface{} {
	return nil
}

func (a *Tony) OnTick(delta time.Duration) {
	a.tick1 += delta
	if a.tick1 > time.Second*5 {
		NewDialer("ws://127.0.0.1:8081")
		a.tick1 = 0

		res, err := a.Invoke("Login", "TestLogin", 99999, "hello invoke")
		log.Debug("* invoke login.testlogin result %v, err %v", res, err)
	}

	// log.Debug("* Tony tick delta: %v", delta)
}

func NewDialer(url string) {
	dialer := &websocket.Dialer{}
	conn, _, err := dialer.Dial(strings.Replace(url, "http", "ws", 1), nil)
	if err != nil {
		log.Error("connect gate error")
		return
	}
	go func() {
		timer1 := time.NewTimer(2 * time.Second)
		timer2 := time.NewTimer(3 * time.Second)
		defer timer1.Stop()
		defer timer2.Stop()
		for {
			select {
			case <-timer1.C:
				p := &proto_login.LoginRequest{}
				data, _ := serialize.Marshal(p)
				data, _ = codec.Encode(1, data)
				conn.WriteMessage(websocket.BinaryMessage, data)
			case <-timer2.C:
				conn.Close()
				return
			}
		}
	}()
}
