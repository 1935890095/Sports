package config

import (
	"encoding/json"
	"errors"
	"fmt"
)

type ConfigurationProvider interface {
	Get(key string) interface{}
	Keys() []string
	Load() error
	GetReload() chan interface{}
}

type provider struct {
	datas map[string]interface{}
}

func (p *provider) Get(key string) interface{} {
	if p.datas == nil {
		return nil
	}
	return p.datas[key]
}

func (p *provider) Keys() []string {
	keys := make([]string, 0)
	if p.datas == nil {
		return keys
	}
	for k := range p.datas {
		keys = append(keys, k)
	}
	return keys
}

func (p *provider) Load() error {
	return errors.New("not implemented")
}

type memoryProvider struct {
	provider
	source memorySource
}

func (p *memoryProvider) Load() error {
	if p.source == nil {
		return errors.New("the source is nil")
	}
	kvs := p.source.LoadDatas()
	if kvs == nil {
		return errors.New("the memory source datas is nil")
	}

	p.datas = make(map[string]interface{})
	for k, v := range kvs {
		p.datas[k] = v
	}
	return nil
}

func (p *memoryProvider) GetReload() chan interface{} {
	return nil
}

func newMemoryProvider(source memorySource) ConfigurationProvider {
	return &memoryProvider{
		source: source,
	}
}

type jsonProvider struct {
	provider
	source byteSource
}

func (p *jsonProvider) Load() error {
	if p.source == nil {
		return errors.New("the source is nil")
	}
	bytes := p.source.LoadDatas()
	if bytes == nil {
		return errors.New("the source datas is nil")
	}

	values := make(map[string]interface{})
	if bufByte := bytes; len(bufByte) > 0 {
		//91=列表  123=对象
		switch bufByte[0] {
		case 91:
			var newList []interface{}
			if err := json.Unmarshal(bufByte, &newList); err != nil {
				//尝试解析数组
				return err
			}
			values = make(map[string]interface{}, len(newList))
			for _, row := range newList {
				if rowM, ok := row.(map[string]interface{}); ok {
					if findID, findOk := rowM["id"]; findOk {
						values[fmt.Sprintf("%v", findID)] = row
					}
				}
			}
		case 123:
			if err := json.Unmarshal(bytes, &values); err != nil {
				return err
			}
		}
	}

	p.datas = make(map[string]interface{})
	for k, v := range values {
		p.datas[k] = v
	}

	return nil
}

func (p *jsonProvider) GetReload() chan interface{} {
	return p.source.GetReload()
}

func newJsonProvider(source byteSource) ConfigurationProvider {
	return &jsonProvider{
		source: source,
	}
}
