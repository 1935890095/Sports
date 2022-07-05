// 背包道具相关

package models

import "xiao/proto/proto_player"

type PlayerBag struct {
	Items []*Item `json:"Items" bson:"Items"`
}

type Item struct {
	Id    int32 // 配置表id
	Count int32 // 数量
}

func (pl *Player) InitBag() {
	pl.Bag = &PlayerBag{
		Items: make([]*Item, 0),
	}
}

func (item *Item) Marshal() *proto_player.Item {
	return &proto_player.Item{
		Id:    int32(item.Id),
		Count: item.Count,
	}
}

func (bag *PlayerBag) Marshal() *proto_player.PlayerBag {
	p := &proto_player.PlayerBag{}
	p.Items = make([]*proto_player.Item, 0)
	for _, item := range bag.Items {
		p.Items = append(p.Items, item.Marshal())
	}
	return p
}

func (bag *PlayerBag) AddItem(id int32, count int32, max int32) (ok bool, item *Item) {
	find := false
	for _, v := range bag.Items {
		if v.Id == id {
			v.Count += count
			if max > 0 && v.Count > max {
				v.Count = max
			}
			find = true
			ok = true
			item = v
			break
		}
	}

	if !find {
		if max > 0 && count > max {
			count = max
		}
		ok = true
		item := &Item{
			Id:    id,
			Count: count,
		}
		bag.Items = append(bag.Items, item)
	}
	return
}

func (bag *PlayerBag) RemoveItem(id int32, count int32) (ok bool, item *Item) {
	for i, item := range bag.Items {
		if item.Id == id {
			if item.Count < count {
				return false, nil
			}
			item.Count -= count
			if item.Count <= 0 {
				bag.Items = append(bag.Items[:i], bag.Items[i+1:]...)
			}
			return true, item
		}
	}
	return false, nil
}

func (bag *PlayerBag) GetCount(id int32) int32 {
	for _, item := range bag.Items {
		if item.Id == id {
			return item.Count
		}
	}
	return 0
}

func (bag *PlayerBag) IsItemEnough(id int32, count int32) bool {
	return bag.GetCount(id) >= count
}

func (bag *PlayerBag) GetItem(id int32) *Item {
	for _, item := range bag.Items {
		if item.Id == id {
			return item
		}
	}
	return nil
}
