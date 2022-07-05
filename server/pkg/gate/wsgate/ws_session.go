package wsgate

import (
	"fmt"
	"strconv"
	"strings"
	"xiao/pkg/codec"
	"xiao/pkg/gate"
	"xiao/pkg/log"
	"xiao/pkg/net/ws"
	"xiao/pkg/serialize"
	"xiao/pkg/serialize/pb"
	"xiao/proto/proto_id"

	"github.com/gogo/protobuf/proto"
)

type Session struct {
	gate.Session
	s          *ws.Session
	ip         string
	id         string
	serializer serialize.Serializer
	decoder    *codec.Decoder
	agent      gate.Agent
}

func NewSession(s *ws.Session) *Session {
	ip := ""
	parts := strings.Split(s.Request().RemoteAddr, ":")
	if len(parts) > 0 {
		ip = parts[0]
	}
	session := &Session{
		s:          s,
		id:         strconv.FormatUint(s.ID(), 10),
		ip:         ip,
		serializer: pb.NewSerializer(),
		decoder:    codec.NewDecoder(),
	}
	return session
}

func (s *Session) GetIP() string  { return s.ip }
func (s *Session) GetID() string  { return s.id }
func (s *Session) IsClosed() bool { return s.s.IsClosed() }

func (s *Session) Send(msg interface{}) error {
	data, err := s.serializer.Marshal(msg)
	if err != nil {
		return fmt.Errorf("Session: send msg error: %v", err)
	}

	pb, ok := msg.(proto.Message)
	if !ok {
		return fmt.Errorf("Session: must be proto message")
	}

	// TODO:
	id := proto_id.MessageID(pb)
	if id <= 0 {
		return fmt.Errorf("Session: message id error %v", msg)
	}
	data, err = codec.Encode(id, data)
	if err != nil {
		return err
	}
	s.s.WriteBinary(data)
	return nil
}

func (s *Session) doRecv(data []byte) {
	packets, err := s.decoder.Decode(data)
	if err != nil {
		// error
		log.Error("Session: recv %v", err)
		return
	}

	if s.agent == nil {
		value, ok := s.Get("#agent")
		if !ok {
			log.Error("Session: no agent")
			return
		}
		s.agent, _ = value.(gate.Agent)
	}

	for _, p := range packets {
		log.Debug("* Session: recv packet: %v", p.String())
		pb := proto_id.NewMessage(p.Type)

		err := s.serializer.Unmarshal(p.Data, pb)
		if err != nil {
			log.Error("Session: unmarshal message error %v,message type =%v \n", err, p.Type)
			continue
		}
		s.agent.Recv(pb)
	}
}

func (s *Session) Close() error {
	return s.s.Close()
}

func (s *Session) Set(key string, value interface{}) {
	s.s.Set(key, value)
}
func (s *Session) Get(key string) (value interface{}, ok bool) {
	return s.s.Get(key)
}
