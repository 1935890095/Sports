package define

// 牌型
type CardTypes int

const (
	CardTypesNone          CardTypes = iota
	CardTypesHighCard                // 高牌
	CardTypesOnePair                 // 一对
	CardTypesTwoPair                 // 两对
	CardTypesThreeOfAKind            // 三条
	CardTypesStraight                // 顺子
	CardTypesFlush                   // 同花
	CardTypesFullHouse               // 葫芦
	CardTypesFourOfAKind             // 四条
	CardTypesStraightFlush           // 同花顺
	CardTypesRoyalFlush              // 皇家同花顺
)

// 两张牌牌型
type TwoCardTypes int

const (
	TwoCardTypesNone      TwoCardTypes = iota
	TwoCardTypesBad                    // 差牌
	TwoCardTypesMix                    // 混合牌
	TwoCardTypesSpeculate              // 投机牌
	TwoCardTypesMiddle                 // 中等牌
	TwoCardTypesBetter                 // 强牌
	TwoCardTypesSuper                  // 超强牌
)
