package agent

import (
	"fmt"
	"time"

	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/AsynkronIT/protoactor-go/remote"
)

type System struct {
	name    string
	system  *actor.ActorSystem
	context *actor.RootContext
	remote  *remote.Remote
	root    PID
	opts    Options
}

func (s *System) Root() PID { return s.root }

func NewSystem(opts ...Option) *System {
	opt := Options{
		CallTTL: DefaultCallTTL,
		Agent:   todoAgent,
	}
	for _, o := range opts {
		o(&opt)
	}

	system := &System{
		opts: opt,
		name: opt.Name,
	}

	system.system = actor.NewActorSystem()
	system.context = system.system.Root

	if opt.Host != "" {
		configure := remote.Configure(opt.Host, opt.Port) // TODO: 研究Configure功能
		system.remote = remote.NewRemote(system.system, configure)
	}
	return system
}

func (s *System) Start() {
	if s.remote != nil {
		s.remote.Start()
	}

	decider := func(reason interface{}) actor.Directive {
		if s.opts.Restart {
			return actor.RestartDirective
		} else {
			return actor.StopDirective
		}
	}
	supervisor := actor.NewOneForOneStrategy(5, time.Second, decider)
	props := actor.PropsFromProducer(
		func() actor.Actor {
			return &defaultActor{opts: s.opts, system: s}
		},
	).WithSupervisor(supervisor)

	if s.name != "" {
		root, err := s.system.Root.SpawnNamed(props, s.name)
		if err != nil {
			panic(fmt.Sprintf("agent: start system error(%v)", err))
		}
		s.root = root
	} else {
		s.root = s.system.Root.Spawn(props)
	}
}

func (s *System) Stop() {
	future := s.context.PoisonFuture(s.root)
	if err := future.Wait(); err != nil {
		panic(fmt.Sprintf("agent: stop system(%s) error(%v)", s.name, err))
	}
	s.remote.Shutdown(true)
}

func (s *System) Create(name string, agent Agent, opts ...Option) (PID, error) {
	future := s.system.Root.RequestFuture(s.root, &createMessage{name: name, agent: agent, opts: opts}, time.Second)
	if result, err := future.Result(); err != nil {
		return nil, fmt.Errorf("agent: system(%s) create %s error(%v)", s.name, name, err)
	} else {
		if pid, ok := result.(PID); ok {
			return pid, nil
		} else {
			if err, ok := result.(error); ok {
				return nil, err
			} else {
				return nil, fmt.Errorf("agent: system(%s) create error(%v)", s.name, err)
			}
		}
	}
}

func (s *System) Destroy(pid PID) {
	future := s.system.Root.PoisonFuture(pid)
	future.Wait()
}

/*
func (s *System) Cast(pid PID, msg interface{}) {
	s.context.Request(pid, WrapMessage(msg, false))
}

func (s *System) Call(pid PID, msg interface{}) (interface{}, error) {
	future := s.context.RequestFuture(pid, WrapMessage(msg, true), s.opts.CallTTL)
	if result, err := future.Result(); err != nil {
		return nil, fmt.Errorf("agent.System Call %v error: %v", pid, err)
	} else {
		return result, err
	}
}

func (s *System) CallNR(pid PID, msg interface{}) error {
	future := s.context.RequestFuture(pid, WrapMessage(msg, true), s.opts.CallTTL)
	if err := future.Wait(); err != nil {
		return fmt.Errorf("agent.System Call error: %v", err)
	} else {
		return nil
	}
}
*/
