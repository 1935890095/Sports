package mongo

import (
	"time"
	"xiao/pkg/log"
)

type Config struct {
	Url             string
	User            string
	Password        string
	Host            string
	Port            int
	MaxPoolSize     uint64
	MinPoolSize     uint64
	MaxConnIdleTime time.Duration
}

var (
	db *DB
)

func NewMongo(c *Config) *DB {
	var err error
	db, err = Open(c)
	if err != nil {
		log.Error("open mongo error(%v)", err)
		panic(err)
	}
	return db
}

func Close() {
	if db != nil {
		db.Close()
		db = nil
	}
}

func NewIQ(database, collection string) *IQ {
	if db == nil {
		log.Error("mongo isn't init")
		return nil
	}
	return db.IQ(database, collection)
}
