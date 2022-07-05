package config

import (
	"fmt"
	"io/ioutil"
	"strings"

	"github.com/fsnotify/fsnotify"
	"github.com/hashicorp/consul/api"
	"github.com/hashicorp/consul/api/watch"
)

type ConfigurationSource interface {
	Build() ConfigurationProvider
}

type source interface {
	GetReload() chan interface{}
}

type memorySource interface {
	source
	LoadDatas() map[string]interface{}
}

type internalMemorySource struct {
	datas map[string]interface{}
}

func (s *internalMemorySource) GetReload() chan interface{} {
	// not support reload
	return nil
}

func (s *internalMemorySource) LoadDatas() map[string]interface{} {
	return s.datas
}

func newMemorySource(datas map[string]interface{}) memorySource {
	return &internalMemorySource{
		datas: datas,
	}
}

type byteSource interface {
	source
	LoadDatas() []byte
}

type fileSource interface {
	byteSource
}

type localFileSource struct {
	fileName       string
	reloadOnChange bool
	datas          []byte
	c              chan interface{}
}

func (s *localFileSource) LoadDatas() []byte {
	bytes, err := ioutil.ReadFile(s.fileName)
	if err != nil {
		// keep loaded datas
		fmt.Printf("load file '%s' error %v", s.fileName, err)
	} else {
		s.datas = bytes
	}

	return s.datas
}

func (s *localFileSource) GetReload() chan interface{} {
	return s.c
}

func (s *localFileSource) watchReload() {
	if !s.reloadOnChange {
		return
	}
	watcher, err := fsnotify.NewWatcher()
	if err != nil {
		fmt.Println(err)
	}
	go func() {
		for {
			select {
			case event, ok := <-watcher.Events:
				if !ok {
					return
				}
				if event.Op&fsnotify.Write == fsnotify.Write {
					fmt.Printf("modified file: %s", s.fileName)
					s.c <- true
				}
			case _, ok := <-watcher.Errors:
				if !ok {
					return
				}
				fmt.Println(err)
			}
		}
	}()

	err = watcher.Add(s.fileName)
	if err != nil {
		fmt.Println(err)
	}
}

func newFileSource(fileName string, reloadOnChange bool) fileSource {
	s := &localFileSource{
		fileName:       fileName,
		reloadOnChange: reloadOnChange,
		c:              make(chan interface{}),
	}
	s.watchReload()
	return s
}

type consulSource struct {
	address        string
	key            string
	reloadOnChange bool
	c              chan interface{}
	datas          []byte
	reloadDatas    []byte
}

func (s *consulSource) LoadDatas() []byte {
	if s.reloadDatas != nil {
		s.datas = s.reloadDatas
		s.reloadDatas = nil
		return s.datas
	}

	client, err := api.NewClient(&api.Config{Address: s.address})
	if err != nil {
		fmt.Println(err)
		return s.datas
	}

	pair, _, err := client.KV().Get(s.key, &api.QueryOptions{})
	if err != nil {
		fmt.Println(err)
	} else {
		s.datas = pair.Value
	}

	return s.datas
}

func (s *consulSource) GetReload() chan interface{} {
	return s.c
}

func (s *consulSource) watchReload() {
	if !s.reloadOnChange {
		return
	}

	plan, err := watch.Parse(map[string]interface{}{
		"type": "key",
		"key":  s.key,
	})
	if err != nil {
		fmt.Printf("watch consul address %s error: %v", s.address, err)
		return
	}

	plan.Handler = func(idx uint64, data interface{}) {
		switch d := data.(type) {
		case *api.KVPair:
			if d.Key == s.key && d.Value != nil {
				s.reloadDatas = d.Value
				s.c <- d.Key
			}
		}
	}

	go plan.Run(s.address)
}

func newConsulSource(address string, key string, reloadOnChange bool) fileSource {
	s := &consulSource{
		address:        address,
		key:            key,
		reloadOnChange: reloadOnChange,
		c:              make(chan interface{}),
	}
	s.watchReload()
	return s
}

func buildConsulSource(address string, path string, reloadOnChange bool) []fileSource {
	cfg := api.DefaultConfig()
	cfg.Address = address
	client, err := api.NewClient(cfg)
	if err != nil {
		panic(err)
	}
	kv := client.KV()
	pairs, _, err := kv.List(path, &api.QueryOptions{})
	if err != nil {
		panic(err)
	}

	fileSources := make([]fileSource, 0)
	for _, pair := range pairs {
		if !strings.HasSuffix(pair.Key, "/") {
			cs := &consulSource{
				address:        address,
				key:            pair.Key,
				reloadOnChange: reloadOnChange,
				c:              make(chan interface{}),
			}
			cs.watchReload()
			fileSources = append(fileSources, cs)
		}
	}
	return fileSources
}
