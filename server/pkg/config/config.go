package config

import (
	"fmt"
)

var chaind ChainedConfiguration = NewChainedConfiguration()

func AddLocalJson(fileName string, reloadOnChange bool) error {
	source := newFileSource(fileName, reloadOnChange)
	provider := newJsonProvider(source)
	return chaind.AddProvider(provider)
}

func AddConsulJson(address string, key string, reloadOnChange bool) error {
	source := newConsulSource(address, key, reloadOnChange)
	provider := newJsonProvider(source)
	return chaind.AddProvider(provider)
}

func AddConsulJsonWithFolder(address string, path string, reloadOnChange bool) error {
	sources := buildConsulSource(address, path, reloadOnChange)
	var err error
	for _, source := range sources {
		provider := newJsonProvider(source)
		err = chaind.AddProvider(provider)
		if err != nil {
			return err
		}
	}

	return nil
}

func AddSource(source ConfigurationSource) error {
	if source == nil {
		fmt.Println("The source can't be nil")
	}
	provider := source.Build()
	if provider == nil {
		fmt.Println("The build provider can't be nil")
	}
	return chaind.AddProvider(provider)
}

func Get(key string) interface{} {
	return chaind.Get(key)
}

func Bind(key string, value interface{}) error {
	return chaind.Bind(key, value)
}
