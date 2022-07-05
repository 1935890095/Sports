package bag

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/player/resource"
)

// 道具的使用效果
func onEffect(ctx context.Player, plObj *models.Player, cfg *define.ItemConfig, count int32, eventType define.ItemEventType) bool {
	success := false
	effectType := cfg.EffectType
	switch effectType {
	case int32(define.ItemEffectType_AddResource):
		onAddResource(ctx, plObj, define.ResourceType(cfg.Type), cfg.EffectParam2*count, eventType)
	default:
	}
	return success
}

// 添加货币
func onAddResource(ctx context.Player, pl *models.Player, t define.ResourceType, value int32, eventType define.ItemEventType) bool {
	resource.Change(ctx, pl, t, value, define.ResourceEventType_Item)
	return true
}
