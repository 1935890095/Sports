package base

import (
	"regexp"
	"strings"
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

// 修改玩家头像
func ChangeHead(ctx context.Player, pl *models.Player, newHead string) {
	ret := &proto_player.S2CChangeHead{}
	if pl.Base.Head != newHead {
		cfg := define.GetHeadConfig(newHead)
		if cfg != nil {
			pl.Base.Head = newHead
			ctx.SavePlayer()
			ret.NewHead = newHead
			ret.Success = true
		}
	}

	ctx.Send(ret)
}

// 修改玩家名字
func ChangeName(ctx context.Player, pl *models.Player, newName string) {
	ret := &proto_player.S2CPlayerRename{}
	if pl.Base.Name != newName {
		newName = delete_extra_space(strings.TrimSpace(newName))
		length := len([]rune(newName))
		if length >= 1 && length <= 12 {
			pl.Base.Name = newName
			ctx.SavePlayer()
			ret.Success = true
			ret.NewName = newName
		}
	}

	ctx.Send(ret)
}

func delete_extra_space(s string) string {
	s1 := strings.Replace(s, "	", " ", -1)
	regstr := "\\s{2,}"
	reg, _ := regexp.Compile(regstr)
	s2 := make([]byte, len(s1))
	copy(s2, s1)
	spc_index := reg.FindStringIndex(string(s2))
	for len(spc_index) > 0 {
		s2 = append(s2[:spc_index[0]+1], s2[spc_index[1]:]...)
		spc_index = reg.FindStringIndex(string(s2))
	}
	return string(s2)
}

func GetRoomPlayerInfo(ctx context.Player, pl *models.Player, msg *proto_player.C2SGetRoomPlayerInfo) {
	player := &models.Player{}
	if err := mongo.NewIQ("game", "player").Where("_id", msg.Id).Find(player).Error(); err != nil {
		log.Info("get room player info err:", err)
		return
	}

	if player.ID != 0 {
		gameInfo := player.GamePlay.GetInfo(define.GameType(msg.GameType))
		ret := &proto_player.S2CGetRoomPlayerInfo{
			Id:         msg.Id,
			Head:       player.Base.Head,
			Name:       player.Base.Name,
			TotalCount: gameInfo.TotalCount,
			WinCount:   gameInfo.WinCount,
		}

		ctx.Send(ret)
	} else {
		robot := &models.Robot{}
		if err := mongo.NewIQ("game", "robot").Where("_id", msg.Id).Find(robot).Error(); err != nil {
			log.Info("get room robot info err:", err)
			return
		}
		if robot.ID != 0 {
			gameInfo := robot.GamePlay.GetInfo(define.GameType(msg.GameType))
			ret := &proto_player.S2CGetRoomPlayerInfo{
				Id:         msg.Id,
				Head:       robot.Base.Head,
				Name:       robot.Base.Name,
				TotalCount: gameInfo.TotalCount,
				WinCount:   gameInfo.WinCount,
			}

			ctx.Send(ret)
		}

	}

}
