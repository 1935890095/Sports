package define

type ConditionType int32

const (
	GameCountCondition  ConditionType = iota + 1 // 对局数
	MatchLevelCondition                          // 联赛等级
)

const (
	FUNCTION_CLOSE = iota
	FUNCTION_OPEN
)
