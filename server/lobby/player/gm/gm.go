package gm

import (
	"regexp"
	"strconv"
	"strings"
	"time"
	"xiao/core/define"
	"xiao/core/models"
	"xiao/lobby/context"
	"xiao/lobby/invoke"
	"xiao/lobby/messages"
	"xiao/lobby/player/bag"
	"xiao/lobby/player/mail"
	"xiao/lobby/player/resource"
	"xiao/lobby/player/task"
	"xiao/pkg/log"
	"xiao/proto/proto_player"
)

func Process(ctx context.Player, pl *models.Player, command string) {
	defer func() {
		if err := recover(); err != nil {
			log.Error("gm.Process error: %v", err)
		}
	}()
	if command == "" {
		return
	}
	datas := strings.Split(deleteExtraSpace(command), " ")
	if len(datas) <= 0 {
		return
	}

	cmd := strings.ToLower(datas[0])
	args := datas[1:]

	log.Debug("Process Gm Command: %v", cmd)
	switch cmd {
	case "task":
		completeTasks(ctx, pl, args)
	case "mail":
		if len(args) >= 2 {
			expiration, _ := strconv.Atoi(args[2])
			var accessory []models.AccessoryInfo
			if len(args) == 4 {
				accessory = newAccessory(args[3])
			}
			err := mail.SendGlobalMail(args[0], args[1], "GM", int64(expiration), nil, nil, accessory)
			if err == nil {
				invoke.Boardcast(ctx, &proto_player.S2CNewMail{})
			}
		}
	case "private_mail":
		// 邮件标题/邮件内容/附件(1,100#2,200)
		if len(args) >= 1 {
			var accessory []models.AccessoryInfo
			if len(args) == 3 {
				accessory = newAccessory(args[2])
			}
			err := mail.SendPrivateMail(args[0], args[1], nil, nil, accessory, pl.ID, "GM")
			if err == nil {
				invoke.Dispatch(ctx, pl.ID, &proto_player.S2CNewMail{})
			}
		}
	case "notice":
		if len(args) >= 6 {
			t, _ := strconv.Atoi(args[0])
			priority, _ := strconv.Atoi(args[1])
			times, _ := strconv.Atoi(args[2])
			timespan, _ := strconv.Atoi(args[3])
			opentime, _ := strconv.Atoi(args[4])
			var otime int64
			if opentime == 0 {
				otime = 0
			} else {
				otime = int64(opentime) + time.Now().Unix()
			}
			ctx.Invoke("Announce", "newNotice", &messages.NewNotice{
				Type:     int32(t),
				Priority: int32(priority),
				Opentime: otime,
				Times:    int32(times),
				Timespan: int32(timespan),
				Content:  args[5],
				Params:   nil,
			})
		}
	case "additem":
		addItem(ctx, pl, args)
	case "announce":
		if len(args) >= 7 {
			t, _ := strconv.Atoi(args[0])
			priority, _ := strconv.Atoi(args[1])
			count, _ := strconv.Atoi(args[2])
			opentime, _ := strconv.Atoi(args[3])
			closetime, _ := strconv.Atoi(args[4])
			ctx.Invoke("Announce", "newAnnouncement", &messages.NewAnnouncement{
				Type:      int32(t),
				Priority:  int32(priority),
				Count:     int32(count),
				Title:     args[5],
				Content:   args[6],
				OpenTime:  time.Now().Unix() + int64(opentime),
				CloseTime: time.Now().Unix() + int64(closetime),
			})
		}
	default:
		log.Info("no command processer")
	}
}

func newAccessory(args string) []models.AccessoryInfo {
	if args == "" {
		return nil
	} else {
		ass := make([]models.AccessoryInfo, 0)
		for _, s := range strings.Split(args, "#") {
			award := strings.Split(s, ",")
			if len(award) != 2 {
				return nil
			}
			id, _ := strconv.Atoi(award[0])
			num, _ := strconv.Atoi(award[1])
			ass = append(ass, models.AccessoryInfo{
				ItemId:     int32(id),
				ItemNumber: int32(num),
			})
		}
		return ass
	}
}

func addItem(ctx context.Player, pl *models.Player, args []string) {
	if len(args) != 2 {
		return
	}

	id, err := strconv.Atoi(args[0])
	if err != nil {
		return
	}

	count, err := strconv.ParseInt(args[1], 10, 64)
	if err != nil {
		return
	}

	bag.AddItem(ctx, pl, int32(id), int32(count), define.ItemEventType_GM)
	resource.Sync(ctx, pl)
}

func completeTasks(ctx context.Player, pl *models.Player, args []string) {
	m := make(map[int32]int)
	var completeAll bool
	if args[0] == "all" {
		completeAll = true
	} else {
		ss := strings.Split(args[0], ",")
		for _, s := range ss {
			i, _ := strconv.Atoi(s)
			m[int32(i)] = 1
		}
	}

	changeTask := make([]*models.Task, 0)
	for _, task := range pl.Task.Tasks {
		_, ok := m[task.Id]
		if task.State < define.StateComplete && (completeAll || ok) {
			task.Count = task.Target
			task.State = define.StateComplete
			changeTask = append(changeTask, task)
		}
	}
	task.DispatchTask(ctx, changeTask)
}

func deleteExtraSpace(s string) string {
	//删除字符串中的多余空格，有多个空格时，仅保留一个空格
	s1 := strings.Replace(s, "  ", " ", -1)      //替换tab为空格
	regstr := "\\s{2,}"                          //两个及两个以上空格的正则表达式
	reg, _ := regexp.Compile(regstr)             //编译正则表达式
	s2 := make([]byte, len(s1))                  //定义字符数组切片
	copy(s2, s1)                                 //将字符串复制到切片
	spc_index := reg.FindStringIndex(string(s2)) //在字符串中搜索
	for len(spc_index) > 0 {                     //找到适配项
		s2 = append(s2[:spc_index[0]+1], s2[spc_index[1]:]...) //删除多余空格
		spc_index = reg.FindStringIndex(string(s2))            //继续在字符串中搜索
	}
	return string(s2)
}
