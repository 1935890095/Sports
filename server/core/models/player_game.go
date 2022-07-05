package models

import (
	"xiao/core/define"
	"xiao/proto/proto_game"
	"xiao/proto/proto_player"
)

type PlayerGame struct {
	GameInfoList []*GameInfo `bson:"GameInfoList" json:"GameInfoList"`
}

type GameInfo struct {
	GameType    define.GameType `bson:"GameType" json:"GameType"`       // 游戏类型
	TotalCount  int32           `bson:"GameCount" json:"GameCount"`     // 游戏对局数
	WinCount    int32           `bson:"WinRate" json:"WinRate"`         // 游戏胜率
	MaxWin      int32           `bson:"MaxWin" json:"MaxWin"`           // 单局最大赢取
	TotalWin    int32           `bson:"TotalWin" json:"TotalWin"`       // 总赢取
	CardValue   int32           `bson:"CardValue" json:"CardValue"`     // 卡牌值
	Cards       []int32         `bson:"Cards" json:"Cards"`             // 赢取最大牌型
	CardType    int32           `bson:"CardType" json:"CardType"`       // 卡牌类型
	RaiseCount  int32           `bson:"RaiseCount" json:"RaiseCount"`   // 加注频率
	RaiseCounts []int32         `bson:"RaiseCounts" json:"RaiseCounts"` // 加注圈
	FoldCounts  []int32         `bson:"FoldCounts" json:"FoldCounts"`   // 弃牌圈
	IntroCount  int32           `bson:"IntroCount" json:"IntroCount"`   // 入池频率
}

func (info *GameInfo) Marshal() *proto_player.GameInfo {
	msg := &proto_player.GameInfo{
		GameType:    int32(info.GameType),
		TotalCount:  info.TotalCount,
		WinCount:    info.WinCount,
		MaxWin:      info.MaxWin,
		TotalWin:    info.TotalWin,
		Cards:       info.Cards,
		CardType:    info.CardType,
		RaiseCount:  info.RaiseCount,
		RaiseCounts: info.RaiseCounts,
		FoldCounts:  info.FoldCounts,
		IntroCount:  info.IntroCount,
	}
	return msg
}

func (pl *Player) InitGame() {
	pl.GamePlay = &PlayerGame{
		GameInfoList: make([]*GameInfo, 0),
	}
}

func (game *PlayerGame) Marshal() *proto_player.PlayerGame {
	proto := &proto_player.PlayerGame{}
	gameInfoList := make([]*proto_player.GameInfo, 0)
	for _, gameInfo := range game.GameInfoList {
		gameInfoList = append(gameInfoList, gameInfo.Marshal())
	}

	proto.GameInfos = gameInfoList
	return proto
}

func (game *PlayerGame) GetInfo(t define.GameType) *GameInfo {
	for _, info := range game.GameInfoList {
		if info.GameType == t {
			return info
		}
	}

	info := new(GameInfo)
	info.GameType = t
	info.RaiseCounts = make([]int32, 4)
	info.FoldCounts = make([]int32, 4)
	game.GameInfoList = append(game.GameInfoList, info)
	return info
}

func (game *PlayerGame) UpdateInfo(msg *proto_game.GameInfo) *GameInfo {
	info := game.GetInfo(define.GameTypeNormal)
	info.TotalCount++
	if msg.Win {
		info.WinCount++
		if msg.Stakes > 0 {
			if msg.Stakes > info.MaxWin {
				info.MaxWin = msg.Stakes
			}
			info.TotalWin += msg.Stakes
		}

		if len(msg.Cards) == 5 {
			if len(info.Cards) <= 0 || msg.CardValue <= info.CardValue {
				info.Cards = msg.Cards
				info.CardValue = msg.CardValue
				info.CardType = int32(msg.CardType)

			}
		}
	}

	if msg.RaiseFlag > 0 {
		info.RaiseCount++
		for i := 0; i < 4; i++ {
			if msg.RaiseFlag&(1<<i) != 0 {
				info.RaiseCounts[i]++
			}
		}
	}

	if msg.FoldFlag > 0 {
		for i := 0; i < 4; i++ {
			if msg.FoldFlag&(1<<i) != 0 {
				info.FoldCounts[i]++
			}
		}
	}

	// 入池率
	if msg.CallOrRaise {
		info.IntroCount++
	}

	return info
}
