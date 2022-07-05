package agent

import (
	"errors"
	"fmt"
	"strings"
	"time"

	"github.com/AsynkronIT/protoactor-go/actor"
)

var (
	ErrNilPID      = errors.New("agent: pid is nil")
	ErrEmptyName   = errors.New("agent: name is empty")
	ErrInvalidName = errors.New("agent: invalid name(maybe contains ':' or '/')")
)

type MessageFilter func(msg interface{}) (bool, interface{})

type Context interface {
	Self() PID
	Cast(pid PID, msg interface{})
	Call(pid PID, msg interface{}) (interface{}, error)
	CallNR(pid PID, msg interface{}) error
	Watch(pid PID)
	Unwatch(pid PID)
	Stop()
	Create(name string, agent Agent, opts ...Option) (PID, error)
	SetMessageFilter(MessageFilter)
	Message() interface{}
	Sender() PID
}

type agentContext struct {
	context actor.Context
	system  *System
	opts    *Options
	states  map[string]interface{}
	filter  MessageFilter
	message interface{}
	sender  PID
	senderA string
}

func (c *agentContext) Self() PID { return c.context.Self() }

func (c *agentContext) Cast(pid PID, msg interface{}) {
	if pid == nil {
		return
	}
	m, e := wrapMessage(c.Self(), pid, msg, false)
	if e != nil {
		// return nil, e
		return
	}
	c.context.Request(pid, m)
}

func (c *agentContext) Call(pid PID, msg interface{}) (interface{}, error) {
	if pid == nil {
		return nil, ErrNilPID
	}
	m, e := wrapMessage(c.Self(), pid, msg, true)
	if e != nil {
		return nil, e
	}
	f := c.context.RequestFuture(pid, m, c.opts.CallTTL)
	if result, err := f.Result(); err != nil {
		return nil, fmt.Errorf("agent context: call error(%v)", err)
	} else {
		return result, nil
	}
}

func (c *agentContext) CallNR(pid PID, msg interface{}) error {
	if pid == nil {
		return ErrNilPID
	}
	m, e := wrapMessage(c.Self(), pid, msg, true)
	if e != nil {
		return e
	}
	f := c.context.RequestFuture(pid, m, c.opts.CallTTL)
	if err := f.Wait(); err != nil {
		return fmt.Errorf("agent context: call error(%v)", err)
	} else {
		return nil
	}
}

func (c *agentContext) Watch(pid PID)   { c.context.Watch(pid) }
func (c *agentContext) Unwatch(pid PID) { c.context.Unwatch(pid) }
func (c *agentContext) Stop()           { c.context.Stop(c.Self()) }

func (c *agentContext) Set(key string, value interface{}) {}
func (c *agentContext) Get(key string) (interface{}, bool) {
	v, ok := c.states[key]
	return v, ok
}

func (c *agentContext) Create(name string, agent Agent, opts ...Option) (PID, error) {
	opt := Options{
		CallTTL: DefaultCallTTL,
		Name:    name,
		Agent:   agent,
	}
	for _, o := range opts {
		o(&opt)
	}

	// Name is expect for every agent
	if opt.Name == "" {
		return nil, ErrEmptyName
	}

	// Agent name can't contain ':' or '/' rune
	if strings.ContainsAny(opt.Name, ":/") {
		return nil, ErrInvalidName
	}

	if opt.Agent == nil {
		opt.Agent = todoAgent
	}

	props := actor.PropsFromProducer(func() actor.Actor {
		return &defaultActor{opts: opt, system: c.system}
	})
	if opt.Restart {
		decider := func(reason interface{}) actor.Directive {
			fmt.Printf("error: %v\n", reason)
			return actor.RestartDirective
		}
		supervisor := actor.NewOneForOneStrategy(5, time.Second, decider)
		props.WithSupervisor(supervisor)
	}

	var (
		pid PID
		err error
	)
	if name != "" {
		pid, err = c.context.SpawnNamed(props, opt.Name)
	} else {
		pid = c.context.Spawn(props)
	}
	return pid, err
}

func (c *agentContext) SetMessageFilter(filter MessageFilter) { c.filter = filter }
func (c *agentContext) Message() interface{}                  { return c.message }
func (c *agentContext) Sender() PID {
	if c.sender != nil {
		return c.sender
	}
	if c.senderA != "" {
		c.sender, _ = Parse(c.senderA)
		return c.sender
	}
	return nil
}
