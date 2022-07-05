package bag

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/proto/proto_player"
)

// AddItem 添加一个道具
func AddItem(ctx context.Player, pl *models.Player, id int32, count int32, eventType define.ItemEventType) bool {
	cfg := define.GetItemConfig(id)
	if cfg == nil {
		return false
	}

	if cfg.AutoUse {
		return checkUseItem(ctx, pl, cfg, count, eventType, false) // TODO: 使用效果
	} else {
		success, item := pl.Bag.AddItem(id, count, cfg.CountLimit)
		if success {
			syncItem(ctx, item) // TODO: 同步物品
		}
	}
	ctx.SavePlayer()
	return true
}

// UseItem 使用道具，处理使用消耗和效果
func UseItem(ctx context.Player, pl *models.Player, id int32, count int32, eventType define.ItemEventType) bool {
	cfg := define.GetItemConfig(int32(id))
	if cfg == nil {
		return false
	}

	if !pl.Bag.IsItemEnough(cfg.CountLimit, count) {
		return false
	}

	ok := checkUseItem(ctx, pl, cfg, count, eventType, true)
	if ok {
		if ok, item := pl.Bag.RemoveItem(cfg.Id, count); ok {
			syncItem(ctx, item)
			ctx.SavePlayer()
		}
	}
	return ok
}

// TODO: 检测资源，扣除资源
func checkUseItem(ctx context.Player, pl *models.Player, cfg *define.ItemConfig, count int32, eventType define.ItemEventType, checkConsume bool) bool {
	if checkConsume && cfg.CurrencyType > 0 {
		// t := define.ItemType(cfg.CurrencyType)
		// v := int64(cfg.Price) * count

		// if pl.IsResourceEnough(t, v) {
		// 	if !resource.Change(ctx, pl, t, -v, define.FromUseItem, eventType) {
		// 		return false
		// 	}
		// } else {
		// 	return falses
		// }
	}

	return onEffect(ctx, pl, cfg, count, eventType)
}

func syncItem(ctx context.Player, item *models.Item) {
	if item == nil {
		return
	}

	ctx.Send(&proto_player.S2CBag{
		Update: item.Marshal(),
	})
}
