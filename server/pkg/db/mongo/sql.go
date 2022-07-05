package mongo

import (
	"context"
	"fmt"
	"reflect"
	"time"
	"xiao/pkg/log"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
)

type (
	FindOptions  = options.FindOptions
	CountOptions = options.CountOptions

	// SumType 用以解析sum后的数据
	SumType struct {
		Sum int64 `json:"sum" bson:"sum"`
	}
)

type DB struct {
	cli    *mongo.Client
	ctx    context.Context
	cancel context.CancelFunc
}

func (db *DB) IQ(database, collection string) *IQ {
	return newIQ(db, database, collection)
}

func Open(conf *Config) (*DB, error) {
	db := new(DB)

	var clientOptions *options.ClientOptions
	if conf.Url != "" {
		clientOptions = options.Client().ApplyURI(conf.Url)
	} else {
		uri := fmt.Sprintf("mongodb://%s:%s@%s:%v", conf.User, conf.Password, conf.Host, conf.Port)
		clientOptions = options.Client().ApplyURI(uri)
	}
	clientOptions.SetMaxConnIdleTime(conf.MaxConnIdleTime)
	clientOptions.SetMaxPoolSize(conf.MaxPoolSize)
	clientOptions.SetMinPoolSize(conf.MinPoolSize)

	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	client, err := mongo.Connect(ctx, clientOptions)
	if err != nil {
		defer func() {
			if cancel != nil {
				cancel()
			}
		}()
		return nil, err
	}

	// Ping the primary
	if err := client.Ping(ctx, readpref.Primary()); err != nil {
		defer func() {
			if cancel != nil {
				cancel()
			}
		}()
		return nil, err
	}

	db.cli = client
	db.ctx = ctx
	db.cancel = cancel
	return db, nil
}

func (db *DB) Close() error {
	var err error
	if db.cli != nil {
		db.cancel()
		err = db.cli.Disconnect(context.Background())
	}
	log.Debug("* mongo closed")
	return err
}

func (db *DB) Insert(database, collection string, data interface{}) error {
	defer func() {
		if err := recover(); err != nil {
			log.Error("mongo insert collection error(%v)", err)
		}
	}()

	var (
		isArrayFlag bool
		err         error
		newData     []interface{}
		// insertIDS   []string
	)

	// 这里进行类型判断，只有结构体和结构体数组才可以进行
	switch reflect.TypeOf(data).Kind() {
	case reflect.Ptr:
		dataHandler := reflect.ValueOf(data).Elem()
		switch dataHandler.Kind() {
		case reflect.Slice:
			for i := 0; i < dataHandler.Len(); i++ {
				newData = append(newData, dataHandler.Index(i).Interface())
			}
			isArrayFlag = true
		case reflect.Struct:
			isArrayFlag = false
		default:
			err = fmt.Errorf("数据结构类型错误")
		}
	case reflect.Slice:
		dataHandler := reflect.ValueOf(data)
		for i := 0; i < dataHandler.Len(); i++ {
			newData = append(newData, dataHandler.Index(i).Interface())
		}
		isArrayFlag = true
	case reflect.Struct:
		isArrayFlag = false
	default:
		err = fmt.Errorf("数据结构类型错误")
	}

	if err != nil {
		// 发生错误
		return err
	}

	// 数组和单条记录的创建方式不一样
	if isArrayFlag {
		collection := db.cli.Database(database).Collection(collection)
		// var res *mongo.InsertManyResult
		_, err = collection.InsertMany(context.TODO(), newData)
		if err != nil {
			err = fmt.Errorf("添加数据集失败，详细：%v", err)
			return err
		}
		// if res != nil {
		// 	for _, r := range res.InsertedIDs {
		// 		if newID, ok := r.(primitive.ObjectID); ok {
		// 			insertIDS = append(insertIDS, newID.Hex())
		// 		} else {
		// 			insertIDS = append(insertIDS, fmt.Sprintf("%s", r))
		// 		}
		// 	}
		// }
	} else {
		collection := db.cli.Database(database).Collection(collection)
		// var res *mongo.InsertOneResult
		_, err = collection.InsertOne(context.TODO(), data)
		if err != nil {
			err = fmt.Errorf("添加文档失败，详细：%v", err.Error())
			return err
		}
		// if newID, ok := res.InsertedID.(primitive.ObjectID); ok {
		// 	insertIDS = append(insertIDS, newID.Hex())
		// } else {
		// 	insertIDS = append(insertIDS, fmt.Sprintf("%s", res.InsertedID))
		// }
	}

	// d.err = nil
	// insertIDS 插入数据后的ID，目前没发现有什么用处。所以不处理
	// return d
	return nil
}

func (db *DB) Delete(database, collection string, conditions map[string]interface{}) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	if len(conditions) == 0 {
		return 0, fmt.Errorf("删除文档失败,没有指明条件")
	}
	res, err := db.cli.Database(database).Collection(collection).DeleteMany(context.TODO(), conditions)
	if err != nil {
		return 0, fmt.Errorf("删除文档失败，详细：%v", err)
	}
	return res.DeletedCount, nil
}

//Update 修改记录，需配合Where使用
func (db *DB) Update(database, tlbname string, conditions map[string]interface{}, data interface{}) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	if len(conditions) == 0 {
		return 0, fmt.Errorf("修改文档失败,没有指明条件")
	}
	res, err := db.cli.Database(database).Collection(tlbname).UpdateMany(context.TODO(), conditions, bson.M{"$set": data})
	if err != nil {
		return 0, fmt.Errorf("修改文档失败，详细：%v", err)
	}
	return res.MatchedCount, nil
}

// 字段累加
func (db *DB) Add(database, collection string, conditions map[string]interface{}, fieldName string, value interface{}) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()
	if len(conditions) == 0 {
		return 0, fmt.Errorf("修改文档失败,没有指明条件")
	}
	res, err := db.cli.Database(database).Collection(collection).UpdateMany(context.TODO(), conditions, bson.M{"$inc": bson.M{fieldName: value}})
	if err != nil {
		return 0, fmt.Errorf("修改文档失败，详细：%v", err)
	}
	return res.MatchedCount, nil
}

// Find 查询记录，可配合Where使用，也可不带任何条件
func (db *DB) Find(database, collection string, conditions map[string]interface{}, data interface{}, opts ...*FindOptions) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	var (
		err         error
		isArrayFlag = false
	)

	if idvalue, ok := conditions["_id"]; ok {
		objectID, err := primitive.ObjectIDFromHex(fmt.Sprintf("%s", idvalue))
		if err == nil {
			conditions["_id"] = objectID
		}
	}

	//这里进行类型判断，只有结构体和结构体数组才可以进行
	switch reflect.TypeOf(data).Kind() {
	case reflect.Ptr:
		dataHandler := reflect.ValueOf(data).Elem()
		switch dataHandler.Kind() {
		case reflect.Slice:
			isArrayFlag = true
		case reflect.Struct:
			isArrayFlag = false
		case reflect.Map:
			isArrayFlag = false
		default:
			err = fmt.Errorf("数据结构类型错误")
		}
	case reflect.Slice:
		isArrayFlag = true
	case reflect.Struct:
		isArrayFlag = false
	case reflect.Map:
		isArrayFlag = false
	default:
		err = fmt.Errorf("数据结构类型错误")
	}
	if err != nil {
		return 0, err
	}

	//查询的时候获取到的是游标
	cursor, err := db.cli.Database(database).Collection(collection).Find(context.TODO(), conditions, opts...)
	if err != nil {
		return 0, fmt.Errorf("查询文档失败，详细：%v", err)
	}

	// 如果是切片，反解所有数据到结构体，如果是结构体，只从游标里取出一条
	if isArrayFlag {
		if err := cursor.All(context.Background(), data); err != nil {
			return 0, fmt.Errorf("查询文档失败，详细：%v", err)
		}
		// TODO: 需要返回条数
		return 1, nil
	} else {
		if ok := cursor.Next(context.Background()); ok {
			if err := cursor.Decode(data); err != nil {
				return 0, fmt.Errorf("查询文档失败，详细：%v", err)
			} else {
				return 1, nil
			}
		}
		return 0, nil
	}
}

//Count 统计，可配合Where使用，也可不带任何条件
func (db *DB) Count(database, collection string, conditions map[string]interface{}, opts ...*CountOptions) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	if idvalue, ok := conditions["_id"]; ok {
		objectID, err := primitive.ObjectIDFromHex(fmt.Sprintf("%s", idvalue))
		if err == nil {
			conditions["_id"] = objectID
		}
	}

	count, err := db.cli.Database(database).Collection(collection).CountDocuments(context.TODO(), conditions, opts...)
	if err != nil {
		return 0, fmt.Errorf("查询文档失败，详细：%v", err)
	}
	return count, nil
}

//FindRange 随机查到一条记录，可配合Where使用，也可不带任何条件 todo 实现的不好。以后重构
func (db *DB) FindRange(database, collection string, data interface{}) error {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	var (
		err         error
		isArrayFlag bool
	)
	//这里进行类型判断，只有结构体和结构体数组才可以进行
	switch reflect.TypeOf(data).Kind() {
	case reflect.Ptr:
		dataHandler := reflect.ValueOf(data).Elem()
		switch dataHandler.Kind() {
		case reflect.Slice:
			isArrayFlag = true
		case reflect.Struct:
			isArrayFlag = false
		default:
			err = fmt.Errorf("数据结构类型错误")
		}
	case reflect.Slice:
		isArrayFlag = true
	case reflect.Struct:
		isArrayFlag = false
	default:
		err = fmt.Errorf("数据结构类型错误")
	}
	if err != nil {
		return err
	}

	//bson.D{{"name", name}, {"pass", pass}}
	var condBson = bson.D{bson.E{Key: "$sample", Value: bson.M{"size": 1}}}
	// 查询的时候获取到的是游标
	cursor, err := db.cli.Database(database).Collection(collection).Aggregate(context.TODO(), condBson)
	if err != nil {
		return fmt.Errorf("查询文档失败，详细：%v", err)
	}

	//如果是切片，反解所有数据到结构体，如果是结构体，只从游标里取出一条
	if isArrayFlag {
		if err := cursor.All(context.Background(), data); err != nil {
			return fmt.Errorf("查询文档失败，详细：%v", err)
		}
	} else {
		if ok := cursor.Next(context.Background()); ok {
			if err := cursor.Decode(data); err != nil {
				return fmt.Errorf("查询文档失败，详细：%v", err)
			}
		}
	}
	return nil
}

// Sum 汇总数据，可以带where
func (db *DB) Sum(database, collection string, conditions map[string]interface{}, sumFieldName string) (int64, error) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()

	var matchMore = bson.D{}
	for key, value := range conditions {
		if key == "_id" {
			objectID, err := primitive.ObjectIDFromHex(fmt.Sprintf("%s", value))
			if err != nil {
				return 0, fmt.Errorf("_id参数不正确")
			}
			matchMore = append(matchMore, bson.E{Key: key, Value: objectID})
		} else {
			matchMore = append(matchMore, bson.E{Key: key, Value: value})
		}
	}

	var waitSum SumType
	pipeline := mongo.Pipeline{
		bson.D{
			bson.E{Key: "$match", Value: matchMore},
		},
		bson.D{
			bson.E{
				Key: "$group", Value: bson.M{"_id": primitive.Null{}, "sum": bson.M{"$sum": "$" + sumFieldName}},
			},
		},
	}
	//查询的时候获取到的是游标
	cursor, err := db.cli.Database(database).Collection(collection).Aggregate(context.TODO(), pipeline)
	if err != nil {
		return 0, fmt.Errorf("查询文档失败，详细：%v", err)
	}

	if ok := cursor.Next(context.Background()); ok {
		if err := cursor.Decode(&waitSum); err != nil {
			return 0, fmt.Errorf("查询文档失败，详细：%v", err)
		}
	}
	return waitSum.Sum, nil
}

// IsExist 判断集合是否存在
func (db *DB) IsExist(database, collection string) bool {
	defer func() {
		if err := recover(); err != nil {
			log.Error("%v", err)
		}
	}()
	//查询的时候获取到的是游标
	if cursor, err := db.cli.Database(database).ListCollectionNames(context.Background(), bson.M{"name": collection}); err != nil || len(cursor) == 0 {
		return false
	}
	return true
}

// // Create 创建集合
// func (db *DB) Create(database, collection string, tableData interface{}) error {
// 	defer func() {
// 		if err := recover(); err != nil {
// 			log.Error("%v", err)
// 		}
// 	}()
// 	// 查询的时候获取到的是游标
// 	if err := db.cli.Database(database).CreateCollection(context.Background(), collection); err != nil {
// 		return fmt.Errorf("创建集合失败，详细：%v", err)
// 	}
// 	// 初始化一条零值记录
// 	return nil
// }
