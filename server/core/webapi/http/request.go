package http

import (
	"encoding/json"
	"errors"
	"io/ioutil"
	ghttp "net/http"
	"reflect"
	"strconv"
	"strings"
)

type HttpRequest struct {
	r *ghttp.Request
}

func (this *HttpRequest) Raw() *ghttp.Request {
	return this.r
}

func (this *HttpRequest) GetRequestPath() string {
	return this.r.URL.Path
}

func (this *HttpRequest) Method() string {
	return this.r.Method
}

func (this *HttpRequest) JsonBody(data interface{}) (err error) {
	var body []byte
	if body, err = ioutil.ReadAll(this.r.Body); err != nil {
		return
	}
	if err = json.Unmarshal(body, data); err != nil {
		return
	}
	return
}

func (this *HttpRequest) GetFormType() (formType string) {
	formType = "multipart/form-data"
	if findType, ok := this.r.Header["Content-Type"]; ok {
		findTypeReal := ""
		if len(findType) > 0 {
			findTypeReal = findType[0]
		}
		if inPos := strings.Index(findTypeReal, "multipart"); inPos == -1 {
			formType = "form-urlencoded"
		}
	}
	return
}

func (this *HttpRequest) Form(data interface{}) (err error) {
	switch formType := this.GetFormType(); formType {
	case "multipart/form-data":
		if err = this.r.ParseMultipartForm(1048576); err != nil {
			return
		}
	case "form-urlencoded":
		if err = this.r.ParseForm(); err != nil {
			return
		}
	default:
		err = errors.New("error form type")
		return
	}
	formParams := this.r.Form
	var m = make(map[string]string, len(formParams))
	for curKey, curRow := range formParams {
		switch curRowLen := len(curRow); curRowLen {
		case 0:
			m[curKey] = ""
		default:
			m[curKey] = curRow[0]
		}
	}
	switch pType := reflect.TypeOf(data).Kind(); pType {
	case reflect.Ptr:
		dataHandler := reflect.ValueOf(data).Elem()
		typeHandler := reflect.TypeOf(data).Elem()
		dataNumField := dataHandler.NumField()
		for i := 0; i < dataNumField; i++ {
			tag, ok := typeHandler.Field(i).Tag.Lookup("json")
			if !ok {
				tag = dataHandler.Type().Field(i).Name
			}
			obj := dataHandler.Field(i)
			this.setFormValue(&obj, m[tag])
		}
	}
	return
}

func (this *HttpRequest) setFormValue(obj *reflect.Value, newV string) {
	oldValue := obj.Interface()
	switch pType := reflect.TypeOf(oldValue).Kind(); pType {
	case reflect.Uint8:
	case reflect.Int8:
	case reflect.Uint:
	case reflect.Int64:
	case reflect.Uint64:
	case reflect.Int32:
	case reflect.Uint32:
	case reflect.Int:
		tmp, _ := strconv.ParseInt(newV, 10, 64)
		obj.SetInt(tmp)
	case reflect.String:
		obj.SetString(newV)
	case reflect.Float32:
	case reflect.Float64:
		tmp, _ := strconv.ParseFloat(newV, 64)
		obj.SetFloat(tmp)
	case reflect.Bool:
		if newV == "true" {
			obj.SetBool(true)
		} else {
			obj.SetBool(false)
		}
	default:
		return
	}
}

func NewHttpRequest(r *ghttp.Request) HttpRequest {
	return HttpRequest{
		r: r,
	}
}
