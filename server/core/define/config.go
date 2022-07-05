package define

type OpenConfig struct {
	Id           int32   `json:"id"`            //功能开启ID
	Name         string  `json:"name"`          //功能名称
	TypeFunction int32   `json:"type_function"` //功能类型1系统功能2活动功能3特殊机制
	Type         []int32 `json:"type"`          //开启条件类型1对局数2联赛等级3特殊处理
	Operator     []int32 `json:"operator"`      //判断1>2<3=
	Value        []int32 `json:"value"`         //参数
	Icon         int32   `json:"icon"`          //默认显示图标1否2是
	Node         string  `json:"node"`          //功能资源名
	Language     string  `json:"language"`      //多元文字
}

type HeadConfig struct {
	Id    int32  `json:"id"`    //序号id
	Order int32  `json:"order"` //排序
	Head  string `json:"head"`  //头像
}

type ItemConfig struct {
	Id           int32  `json:"id"`            //序号id1:PGC2:MPKG
	Name         string `json:"name"`          //名字填写字符串常量，读取翻译表
	Type         int32  `json:"type"`          //道具类型
	OpenLevel    int32  `json:"open_level"`    //使用等级
	CurrencyType int32  `json:"currency_type"` //使用消耗类型1:PGC2:MPKG
	Price        int32  `json:"price"`         //使用消耗价格（房间道具价格在用，其他不需要）
	CountLimit   int32  `json:"count_limit"`   //数量上限
	AutoUse      bool   `json:"auto_use"`      //自动使用
	EffectType   int32  `json:"effect_type"`   //使用效果1:获得货币及房间道具
	EffectParam1 int32  `json:"effect_param1"` //效果参数1（对应的道具，收集物对应chat表）
	EffectParam2 int32  `json:"effect_param2"` //效果参数2（若是货币类，则是使用获得的数量）（倍数，最少为1）
	EffectParam3 int32  `json:"effect_param3"` //效果参数3
	Icon         string `json:"icon"`          //道具图标
}

type TaskConfig struct {
	Id         int32    `json:"id"`          //任务id
	TaskType   int32    `json:"task_type"`   //任务分类1：每日任务2：成就任务
	TaskSort   int32    `json:"task_sort"`   //任务类型1：登录次数2：参与游戏局数3：参与拍卖次数4：赢局数（实际盈利局数）5：累计下注数量
	Param1     int32    `json:"param1"`      //特殊参数（用于后期拓展使用）
	Param2     int32    `json:"param2"`      //任务相关参数
	AwardType1 []int32  `json:"award_type1"` //任务奖励类型2：MPKG
	AwardNum1  []int32  `json:"award_num1"`  //奖励数量
	Icon1      []string `json:"icon1"`       //任务奖励图标（填写图标资源名）
	Text       string   `json:"text"`        //任务文字
}

type RoomConfig struct {
	Id         int32   `json:"id"`          //房间id
	Nums       []int32 `json:"nums"`        //房间人数
	StakesMin  int32   `json:"stakes_min"`  //房间小盲注
	StakesMax  int32   `json:"stakes_max"`  //房间大盲注
	BuyMin     int32   `json:"buy_min"`     //最小买入量
	BuyMax     int32   `json:"buy_max"`     //最大买入量
	BuyDafault int32   `json:"buy_dafault"` //默认买入量
	Give       float32 `json:"give"`        //荷官打赏金额（以房间大盲注为基础）
	Take       float32 `json:"take"`        //系统抽水比例（按照房间大盲注抽水）
	AutoBuy    bool    `json:"auto_buy"`    //是否默认勾选自动续费true:默认勾选false：默认不勾选
	AutoBuymax bool    `json:"auto_buymax"` //是否默认勾选买入最大true:默认勾选false：默认不勾选
	RobotType  []int32 `json:"robot_type"`  //机器人匹配类型
	RobotBuy   []int32 `json:"robot_buy"`   //机器人默认买入量
	RobotOpen  bool    `json:"robot_open"`  //机器人入场开关true：开放false：不开放
	RobotEntry string  `json:"robot_entry"` //机器人入场配置
	Icon       string  `json:"icon"`        //背景图
}

type RobotAiConfig struct {
	Id          int32     `json:"id"`           //序号
	Mode        int32     `json:"mode"`         //模式
	Round       []int32   `json:"round"`        //圈
	CallType    int32     `json:"call_type"`    //CALL值类型
	CallValue   string    `json:"call_value"`   //CALL值参数
	Win         bool      `json:"win"`          //最终盈利情况
	RoundBets   string    `json:"round_bets"`   //本圈主动投注次数
	Hands1      []int32   `json:"hands1"`       //翻牌前最大牌型
	Hands2      []int32   `json:"hands2"`       //翻牌后最大牌型
	RemainBlind string    `json:"remain_blind"` //剩余筹码（大盲倍数）
	Action1     int32     `json:"action1"`      //行为1
	Waiting1    []int32   `json:"waiting1"`     //等待时间1
	Param1      []float32 `json:"param1"`       //参数1
	Pro1        int32     `json:"pro1"`         //几率1
	Action2     int32     `json:"action2"`      //行为2
	Waiting2    []int32   `json:"waiting2"`     //等待时间2
	Param2      []float32 `json:"param2"`       //参数2
	Pro2        int32     `json:"pro2"`         //几率2
	Action3     int32     `json:"action3"`      //行为3
	Waiting3    []int32   `json:"waiting3"`     //等待时间3
	Param3      []float32 `json:"param3"`       //参数3
	Pro3        int32     `json:"pro3"`         //几率3
	Action4     int32     `json:"action4"`      //行为4
	Waiting4    []int32   `json:"waiting4"`     //等待时间4
	Param4      []float32 `json:"param4"`       //参数4
	Pro4        int32     `json:"pro4"`         //几率4
}

type RobotConfig struct {
	Id        int32    `json:"id"`         //序号id
	PlayerId  int64    `json:"player_id"`  //账号ID这项数据不可随意修改
	Gender    int32    `json:"gender"`     //性别0:女1:男
	Names     []string `json:"names"`      //名字组
	Heads     []int32  `json:"heads"`      //头像组
	RobotType int32    `json:"robot_type"` //类型标识
	GameMax   int32    `json:"game_max"`   //参与牌局上限
	QuitPro   int32    `json:"quit_pro"`   //退房概率
	BuyMax    int32    `json:"buy_max"`    //买入次数上限
	WorkTime  string   `json:"work_time"`  //上班时间
	Cooldown  int32    `json:"cooldown"`   //冷却时间
}

type HandTypeConfig struct {
	Id    int32 `json:"id"`    //牌型序号ID
	Type  int32 `json:"type"`  //类型ID
	Hand1 int32 `json:"hand1"` //牌1点数
	Hand2 int32 `json:"hand2"` //牌2点数
	Suit  int32 `json:"suit"`  //花色
}
