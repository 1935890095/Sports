package define

const (
	StateNotOpen = iota
	StateOpen
	StateComplete
	StateClose
)

const (
	DailyTask   = 1 + iota // 日常任务
	AchieveTask            // 成就任务
)

const (
	TaskType_LoginTimes       = 1 + iota // 登录次数
	TaskType_JoinGameTimes               // 参与游戏局数
	TaskType_JoinAuctionTimes            // 参与拍卖次数
	TaskType_WinGameCount                // 实际盈利局数
	TaskType_TotalBetCount               // 累计下注数量

)
