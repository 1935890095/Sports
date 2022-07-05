package config

type Configuration interface {
	Bind(key string, value interface{}) error
	Get(key string) interface{}
}
