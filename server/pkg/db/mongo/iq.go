package mongo

import (
	"reflect"
	"xiao/pkg/log"

	"go.mongodb.org/mongo-driver/bson"
)

type sortOptions = bson.E

// Integrated query
type IQ struct {
	db         *DB
	database   string
	collection string
	sort       sortOptions
	condition  map[string]interface{}
	limit      int64
	skip       int64
	result     int64
	err        error
}

func newIQ(db *DB, database, collection string) *IQ {
	iq := &IQ{
		db:         db,
		database:   database,
		collection: collection,
		condition:  make(map[string]interface{}),
	}
	return iq
}

func (d *IQ) Result() int64 { return d.result }
func (d *IQ) Error() error  { return d.err }

func (d *IQ) Clear() {
	d.sort = sortOptions{}
	d.condition = make(map[string]interface{})
	d.limit = 0
	d.skip = 0
	d.result = 0
	d.err = nil
}

// Where 条件
func (d *IQ) Where(key string, value interface{}) *IQ {
	d.condition[key] = value
	return d
}

// WhereIn
func (d *IQ) WhereIn(key string, value interface{}) *IQ {
	var vList bson.A
	switch reflect.TypeOf(value).Kind() {
	case reflect.Slice, reflect.Array:
		newV := reflect.ValueOf(value)
		for i := 0; i < newV.Len(); i++ {
			vList = append(vList, newV.Index(i).Interface())
		}
	default:
		log.Error("WhereIn参数不正确，value不为数组或切片")
		return d
	}

	inValue := bson.M{
		"$in": vList,
	}
	d.condition[key] = inValue

	return d
}

//WhereLike WhereLike
func (d *IQ) WhereLike(key []string, value string) *IQ {
	var regexList bson.A
	for _, filedName := range key {
		bm := bson.M{filedName: bson.M{"$regex": value, "$options": "im"}}
		regexList = append(regexList, bm)
	}

	d.condition["$or"] = regexList
	return d
}

// Sort 排序
func (d *IQ) Sort(filedName, sortType string) *IQ {
	var value = 1
	if sortType == "desc" || sortType == "DESC" {
		value = -1
	}

	d.sort = sortOptions{
		Key:   filedName,
		Value: value,
	}
	return d
}

// Limit 限制记录数
func (d *IQ) Limit(number int64) *IQ {
	d.limit = number
	return d
}

// Skip 跳过的记录数
func (d *IQ) Skip(number int64) *IQ {
	d.skip = number
	return d
}

func (d *IQ) Insert(data interface{}) *IQ {
	d.err = d.db.Insert(d.database, d.collection, data)
	return d
}

func (d *IQ) Delete() *IQ {
	d.result, d.err = d.db.Delete(d.database, d.collection, d.condition)
	return d
}

func (d *IQ) Update(data interface{}) *IQ {
	d.result, d.err = d.db.Update(d.database, d.collection, d.condition, data)
	return d
}

func (d *IQ) Add(fieldName string, fieldValue interface{}) *IQ {
	d.result, d.err = d.db.Add(d.database, d.collection, d.condition, fieldName, fieldValue)
	return d
}

func (d *IQ) Find(data interface{}) *IQ {
	opt := &FindOptions{}
	if d.sort.Key != "" {
		opt.SetSort(bson.D{bson.E{Key: d.sort.Key, Value: d.sort.Value}})
	}
	if d.limit != 0 {
		opt.SetLimit(d.limit)
	}
	if d.skip != 0 {
		opt.SetSkip(d.skip)
	}

	d.result, d.err = d.db.Find(d.database, d.collection, d.condition, data, opt)
	return d
}

func (d *IQ) Count(count *int64) *IQ {
	opt := &CountOptions{}
	if d.limit != 0 {
		opt.SetLimit(d.limit)
	}
	if d.skip != 0 {
		opt.SetSkip(d.skip)
	}
	d.result, d.err = d.db.Count(d.database, d.collection, d.condition, opt)
	*count = d.result
	return d
}

func (d *IQ) FindRange(data interface{}) *IQ {
	d.err = d.db.FindRange(d.database, d.collection, data)
	return d
}

func (d *IQ) Sum(sumFieldName string, sumNum *int64) *IQ {
	d.result, d.err = d.db.Sum(d.database, d.collection, d.condition, sumFieldName)
	*sumNum = d.result
	return d
}

func (d *IQ) IsExist(collection string) bool {
	return d.db.IsExist(d.database, collection)
}

// func (d *IQ) CreateTable(tableName string, tableData interface{}) *IQ {
// 	return d.db.CreateTable(d, tableName, tableData)
// }
