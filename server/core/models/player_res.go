package models

import (
	"xiao/core/define"
	"xiao/pkg/log"
)

func (p *Player) GetResource(t define.ResourceType) int32 {
	have := int32(0)
	switch t {
	case define.ResourceType_Money:
		have = p.Base.Money
	case define.ResourceType_Coin:
		have = p.Base.Coin
	default:
		log.Info("can't get resource by type %v", t)
	}

	return have
}

func (p *Player) IsResourceEnough(t define.ResourceType, count int32) bool {
	return p.GetResource(t) >= count
}

func (p *Player) GetChangedProperties() []define.ResourceType {
	props := make([]define.ResourceType, 0)
	if p.ChangedProperties != nil {
		for k, _ := range p.ChangedProperties {
			props = append(props, k)
		}
	}
	return props
}

func (p *Player) ClearChangedProperties() {
	p.ChangedProperties = nil
}
