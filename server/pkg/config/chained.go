package config

import (
	"errors"
	"fmt"
	"reflect"
	"strings"
	"xiao/pkg/log"

	"github.com/mitchellh/mapstructure"
)

type ChainedConfiguration interface {
	Configuration
	AddProvider(ConfigurationProvider) error
}

type chainedConfiguration struct {
	providers []ConfigurationProvider
}

func (c *chainedConfiguration) Bind(key string, value interface{}) (err error) {
	keys := strings.Split(key, ".")
	var decodeData interface{}
	datas := c.getDatas(keys)

	switch reflect.TypeOf(value).Kind() {
	case reflect.Ptr:
		dataHandler := reflect.ValueOf(value).Elem()
		switch dataHandler.Kind() {
		case reflect.Slice:
			das := make([]map[string]interface{}, 0)
			for _, v := range datas {
				v1, ok := v.(map[string]interface{})
				if !ok {
					return fmt.Errorf("Bind error,no correct data")
				}
				das = append(das, v1)
			}
			decodeData = das
		default:
			decodeData = datas
		}
	default:
		return fmt.Errorf("Bind error,must be ptr")
	}

	if datas != nil {
		decoder, err := mapstructure.NewDecoder(&mapstructure.DecoderConfig{
			ZeroFields:       true,
			Squash:           true,
			WeaklyTypedInput: true,
			Result:           value,
			TagName:          "json",
		})
		if err != nil {
			return err
		}
		return decoder.Decode(decodeData)
	}

	return fmt.Errorf("Bind error, can not found key '%s'", key)
}

func (c *chainedConfiguration) Get(key string) interface{} {
	keys := strings.Split(key, ".")
	if key == "" {
		return c.getDatas(keys)
	}
	keyCount := len(keys)
	if keyCount == 1 {
		for i := len(c.providers) - 1; i >= 0; i-- {
			if v := c.providers[i].Get(key); v != nil {
				return v
			}
		}
		return nil
	}

	findKeys := keys[:len(keys)-1]
	datas := c.getDatas(findKeys)
	if datas != nil {
		if v, ok := datas[keys[keyCount-1]]; ok {
			return v
		}
	}
	return nil
}

func (c *chainedConfiguration) AddProvider(provider ConfigurationProvider) error {
	if provider != nil {
		err := provider.Load()
		if err != nil {
			return err
		}

		c.providers = append(c.providers, provider)
		c := provider.GetReload()
		if c != nil {
			go func() {
				defer func() {
					close(c)
				}()

				for {
					n := <-c
					provider.Load()
					log.Info("datas %v reloaded", n)
				}
			}()
		}

		return nil
	}

	return errors.New("provider is nil")
}

func (c *chainedConfiguration) getDatas(keys []string) map[string]interface{} {
	var datas map[string]interface{}
	m, n := len(keys), len(c.providers)

	if m == 1 && keys[0] == "" {
		datas = make(map[string]interface{})
		for i := 0; i < n; i++ {
			keys := c.providers[i].Keys()
			for _, v := range keys {
				datas[v] = c.providers[i].Get(v)
			}
		}

		return datas
	}

	for j := n - 1; j >= 0; j-- {
		v := c.providers[j].Get(keys[0])
		maps, ok := v.(map[string]interface{})
		if ok {
			datas = maps
			break
		}
	}

	if datas != nil {
		for i := 1; i < m; i++ {
			v := datas[keys[i]]
			maps, ok := v.(map[string]interface{})
			if ok {
				datas = maps
			} else {
				return nil
			}
		}
	}

	return datas
}

func NewChainedConfiguration(providers ...ConfigurationProvider) ChainedConfiguration {
	chained := &chainedConfiguration{
		providers: make([]ConfigurationProvider, 0),
	}
	for _, v := range providers {
		chained.AddProvider(v)
	}
	return chained
}
