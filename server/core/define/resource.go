package define

type ResourceType int

const (
	ResourceType_None  ResourceType = iota
	ResourceType_Money              // 打牌用的货币
	ResourceType_Coin               // 系统产出货币
)

type ResourceEventType int

const (
	ResourceEventType_None   ResourceEventType = iota
	ResourceEventType_Item                     // 使用道具
	ResourceEventType_BuyIn                    // 牌局买入
	ResourceEventType_Return                   // 牌局返还
)
