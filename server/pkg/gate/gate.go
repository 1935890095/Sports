package gate

type Gate interface {
	NewSession(conn interface{}) (Session, error)
	NewAgent(session Session) (Agent, error)
	Close(sessionID string) error
	// Route(s Session, mod string, msg interface{})
	GetSession(sessionID string) Session
}

type Session interface {
	GetIP() string
	GetID() string
	Send(msg interface{}) error
	Close() error
	IsClosed() bool
	Set(key string, value interface{})
	Get(key string) (value interface{}, ok bool)
}

type Agent interface {
	GetGate() Gate
	OnInit(gate Gate, session Session)
	GetSession() Session
	Send(msg interface{})
	Recv(msg interface{})
	Close()
}

// type Router interface {
// 	OnRoute(session Session, msg interface{})
// }
