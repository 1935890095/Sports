package env

import (
	"bytes"
	"time"

	"github.com/spf13/viper"
)

var (
	v *viper.Viper
)

func LoadEnv(env []byte) (*Env, error) {
	v = viper.New()

	v.SetConfigType("toml")
	v.AutomaticEnv()
	buff := bytes.NewReader(env)

	var err error
	err = v.ReadConfig(buff)
	if err != nil {
		return nil, err
	}

	e := new(Env)
	err = v.Unmarshal(&e)
	if err != nil {
		return nil, err
	}
	e.v = v
	return e, nil
}

type Env struct {
	v        *viper.Viper
	ID       int
	Name     string
	Host     string
	Port     int
	Debug    bool
	ConfPath string
	HttpUrl  string
	Fps      int
	Gate     *Gate
	Mongo    *Mongo
	Redis    *Redis
	Consul   *Consul
	Node     *Nodes
	Log      *Log
}

type Gate struct {
	Host              string
	WriteWait         time.Duration
	PongWait          time.Duration
	PingPeriod        time.Duration
	MaxMessageSize    int
	MessageBufferSize int
}

type Mongo struct {
	Url             string
	MinPoolSize     int
	MaxPoolSize     int
	MaxConnIdleTime time.Duration
}

type Redis struct {
	Hosts    []string
	Type     string
	Password string
	PoolSize int
}

type Consul struct {
	Host           string
	Path           string
	ReloadOnChange bool
}

type Node struct {
	Host string
	Port int
	Id   int
	Name string
}

type Nodes struct {
	Lobby []*Node
	Game  []*Node
	Match *Node
}

type Log struct {
	WriteFile  bool
	Path       string
	MaxSize    int
	MaxBackups int
	MaxAge     int
	Compress   bool
}

func (e *Env) Get(key string) interface{}            { return e.v.Get(key) }
func (e *Env) GetBool(key string) bool               { return e.v.GetBool(key) }
func (e *Env) GetDuration(key string) time.Duration  { return e.v.GetDuration(key) }
func (e *Env) GetInt(key string) interface{}         { return e.v.GetInt(key) }
func (e *Env) GetInt32(key string) interface{}       { return e.v.GetInt32(key) }
func (e *Env) GetInt64(key string) interface{}       { return e.v.GetInt64(key) }
func (e *Env) GetIntSlice(key string) interface{}    { return e.v.GetIntSlice(key) }
func (e *Env) GetString(key string) interface{}      { return e.v.GetString(key) }
func (e *Env) GetStringMap(key string) interface{}   { return e.v.GetStringMap(key) }
func (e *Env) GetStringSlice(key string) interface{} { return e.v.GetStringSlice(key) }
func (e *Env) GetTime(key string) interface{}        { return e.v.GetTime(key) }
