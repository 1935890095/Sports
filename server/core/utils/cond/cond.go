package cond

import (
	"math"
	"strconv"
	"strings"
	"xiao/pkg/log"
)

type Op int

const (
	NONE Op = iota
	EQ
	LT
	LE
	GT
	GE
)

type Cond struct {
	Op    Op
	Value float64
}

const LTSTR = ")"
const LESTR = "]"
const GTSTR = "("
const GESTR = "["

func parse(symbol string, value string) *Cond {
	op := NONE
	switch symbol {
	case LTSTR:
		op = LT
	case LESTR:
		op = LE
	case GTSTR:
		op = GT
	case GESTR:
		op = GE
	default:
		return nil
	}

	var v float64
	var err error
	if value == "*" {
		v = math.MaxFloat64
	} else {
		v, err = strconv.ParseFloat(value, 64)
		if err != nil {
			log.Error("parse float error %s", value)
			return nil
		}
	}

	c := &Cond{
		Op:    op,
		Value: v,
	}
	return c
}

func (c *Cond) true(value float64) bool {
	switch c.Op {
	case EQ:
		return value == c.Value
	case LT:
		return value < c.Value
	case LE:
		return value <= c.Value
	case GT:
		return value > c.Value
	case GE:
		return value >= c.Value
	}
	return false
}

// cond (x,y) [x,y] (x,y] [x,y)
func Pass(cond string, value float64) bool {
	if cond == "" {
		log.Error("cond is empty")
		return false
	}

	conds := strings.Split(cond, ",")
	if len(conds) != 2 {
		log.Error("conds len is not 2")
		return false
	}

	left := parse(conds[0][:1], conds[0][1:])
	if left == nil {
		log.Error("cond parse left error %s", conds[0])
		return false
	}
	l := len(conds[1])
	right := parse(conds[1][l-1:], conds[1][:l-1])
	if right == nil {
		log.Error("cond parse right error %s", conds[1])
		return false
	}
	return left.true(value) && right.true(value)
}

type Conds struct {
	conds []*Cond
}

func (c *Conds) Pass(value float64) bool {
	for _, cond := range c.conds {
		if !cond.true(value) {
			return false
		}
	}
	return true
}

func ParseConds(cond string) *Conds {
	if cond == "" {
		log.Error("cond is empty")
		return nil
	}

	conds := strings.Split(cond, ",")
	if len(conds) != 2 {
		log.Error("conds len is not 2")
		return nil
	}

	left := parse(conds[0][:1], conds[0][1:])
	if left == nil {
		log.Error("cond parse left error %s", conds[0])
		return nil
	}
	l := len(conds[1])
	right := parse(conds[1][l-1:], conds[1][:l-1])
	if right == nil {
		log.Error("cond parse right error %s", conds[1])
		return nil
	}

	c := &Conds{
		conds: []*Cond{left, right},
	}

	return c
}
