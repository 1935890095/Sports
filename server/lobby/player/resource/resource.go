package resource

import (
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func Sync(ctx context.Player, pl *models.Player) {
	props := pl.GetChangedProperties()
	count := len(props)
	if count <= 0 {
		return
	}

	pl.ClearChangedProperties()
	msg := new(proto_player.S2CResourceChanged)
	for i := 0; i < count; i++ {
		msg.Types = append(msg.Types, int32(props[i]))
		msg.Values = append(msg.Values, pl.GetResource(props[i]))
	}
	ctx.Send(msg)
}

func Change(ctx context.Player, p *models.Player, t define.ResourceType, change int32, event define.ResourceEventType) bool {
	if change == 0 {
		return true
	}

	if change < 0 {
		if !p.IsResourceEnough(t, -change) {
			return false
		}
	}

	switch t {
	case define.ResourceType_Money:
		p.Base.Money += change
	case define.ResourceType_Coin:
		p.Base.Coin += change
	default:
		log.Info("can't change resource by type %v", t)
		return false
	}

	if p.Cache.ChangedProperties == nil {
		p.Cache.ChangedProperties = make(map[define.ResourceType]bool)
	}
	p.Cache.ChangedProperties[t] = true

	ctx.SavePlayer()
	return true
}
