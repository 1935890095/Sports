package invoke

import (
	"errors"
	"fmt"
	"reflect"
)

type Invoker struct {
	functions map[string]*FunctionInfo
}

func NewInvoker() *Invoker {
	return &Invoker{
		functions: make(map[string]*FunctionInfo),
	}
}

func (iv *Invoker) Register(fn string, f interface{}) {
	fi := new(FunctionInfo)
	fi.Function = reflect.ValueOf(f)
	fi.FuncType = fi.Function.Type()
	fi.InType = []reflect.Type{}
	for i := 0; i < fi.FuncType.NumIn(); i++ {
		fi.InType = append(fi.InType, fi.FuncType.In(i))
	}
	iv.functions[fn] = fi
}
func (iv *Invoker) Invoke(fn string, args ...interface{}) (interface{}, error) {

	fi, ok := iv.functions[fn]
	if !ok {
		return nil, fmt.Errorf("not found function %s", fn)
	}

	f := fi.Function
	// fType := fi.FuncType
	fInType := fi.InType

	if len(args) != len(fInType) {
		return nil, fmt.Errorf("params1 not match %s", fn)
	}

	var in []reflect.Value
	if len(args) > 0 {
		in = make([]reflect.Value, len(args))
		for i := 0; i < len(args); i++ {
			aType := reflect.ValueOf(args[i]).Type()
			if !aType.AssignableTo(fInType[i]) {
				return nil, fmt.Errorf("params2 not match %s", fn)
			}
			in[i] = reflect.ValueOf(args[i])
		}
	}

	// out := f.Call(in)
	// if len(out) != 2 {
	// 	return nil, fmt.Errorf("return value not match %s", fn)
	// }
	// rs := make([]interface{}, len(out))
	// for i, v := range out {
	// 	rs[i] = v.Interface()
	// }
	// var rerr error
	// switch e := rs[1].(type) {
	// case error:
	// 	rerr = e
	// case nil:
	// 	rerr = nil
	// default:
	// 	rerr = fmt.Errorf("invoke function the second result type must be 'error'")
	// }
	// return rs[0], rerr

	out := f.Call(in)
	if len(out) > 2 {
		return nil, errors.New("function return values error")
	}

	rs := make([]interface{}, len(out))
	for i, v := range out {
		rs[i] = v.Interface()
	}

	if len(out) == 0 {
		return nil, nil
	} else if len(out) == 1 {
		switch e := rs[0].(type) {
		case error:
			return nil, e
		case nil:
			return nil, nil
		default:
			return out[0].Interface(), nil
		}
	} else {
		var rerr error
		switch e := rs[1].(type) {
		case error:
			rerr = e
		case nil:
			rerr = nil
		default:
			rerr = fmt.Errorf("invoke function the second result type must be 'error'")
		}
		return rs[0], rerr
	}
}

type FunctionInfo struct {
	Function reflect.Value
	FuncType reflect.Type
	InType   []reflect.Type
}
