package define

type ItemEventType int

const (
	ItemEventTypeNone       ItemEventType = iota
	ItemEventType_Recharge                // 充值
	ItemEventType_RoomAward               // 房间奖励
	ItemEventType_ShopBuy                 // 商城购买
	ItemEventType_GM                      // gm命令获取
	ItemEventType_Mail                    // 邮件附件获取
)

type ItemEffectType int

// 道具使用效果类型
const (
	ItemEffectType_None        ItemEffectType = iota
	ItemEffectType_AddResource                // 添加货币
)
