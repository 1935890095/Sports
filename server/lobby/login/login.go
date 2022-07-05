package login

import (
	"fmt"
	"time"
	"xiao/core/models"
	"xiao/lobby/messages"
	"xiao/lobby/player"
	"xiao/pkg/agent"
	"xiao/pkg/db/mongo"
	"xiao/pkg/log"
	"xiao/pkg/module"
	"xiao/pkg/module/modules"
	"xiao/pkg/utils/id"
	"xiao/proto/proto_login"
)

var Module = func() module.Module {
	login := new(Login)
	return login
}

type Login struct {
	modules.BaseModule
	players map[int64]agent.PID
}

func (l *Login) OnInit(app module.App) {
	l.BaseModule.OnInit(app)
	l.Register("login", l.login)
	l.Register("logout", l.logout)
	l.Register("disconnect", l.disconnect)
	l.Register("castAgent", l.castAgent)
	l.players = make(map[int64]agent.PID)
}

func (l *Login) Version() string { return "1.0.0" }
func (l *Login) GetType() string { return "Login" }

func (m *Login) OnTick(delta time.Duration) {}

func (l *Login) OnMessage(msg interface{}) interface{} {
	log.Debug("* login message %v", msg)
	return nil
}

func (l *Login) login(msg *messages.Login) (result *messages.LoginResult, err error) {
	result = &messages.LoginResult{
		Response: &proto_login.LoginResponse{
			Result: proto_login.LoginFail,
		},
	}
	loginData, loginRes := checkLoginData(msg.Request)
	if loginData == nil {
		result.Response.Result = loginRes
		return
	}

	var pid module.PID
	pid, ok := l.players[loginData.UserId]
	if ok {
		_, err = l.Context.Call(pid, &messages.LoginReplace{
			Session: msg.Session,
		})
		if err != nil {
			result.Response.Result = proto_login.LoginReplaceError
			return
		}
	} else {
		model, loginRes := loadPlayerData(loginData.UserId)
		if model == nil {
			result.Response.Result = loginRes
			return
		}

		pl := player.New(model, msg.Session)
		pid, err = l.Context.Create(
			fmt.Sprintf("player#%d", model.ID),
			pl,
		)
		if err != nil {
			result.Response.Result = proto_login.LoginCreateError
			return
		}
		pl.OnInit(l.App)
		l.players[model.ID] = pid
		l.Context.Cast(pid, &messages.LoginSuccess{})
	}

	result.PlayerId = loginData.UserId
	result.PlayerPID = pid
	result.Response.Result = proto_login.LoginSuccess
	return
}

// 加载登录信息
func checkLoginData(req *proto_login.LoginRequest) (*models.Login, proto_login.LoginResult) {
	account := req.Account
	password := req.Password
	if account == "" || password == "" {
		return nil, proto_login.LoginAccountOrPasswordError
	}

	m := &models.Login{}
	q := mongo.NewIQ("game", "login").Where("_id", account).Limit(1).Find(m)
	if q.Error() != nil || m.Account == "" {
		uid, _ := id.General()
		m = &models.Login{
			Account:  account,
			Password: password,
			Type:     int32(req.LoginType),
			Channel:  req.Channel,
			UserId:   uid,
		}

		if err := mongo.NewIQ("game", "login").Insert(m).Error(); err != nil {
			return nil, proto_login.LoginDBError
		}

		return m, proto_login.LoginSuccess
	}

	if m.Password != password {
		return nil, proto_login.LoginAccountOrPasswordError
	}

	return m, proto_login.LoginSuccess
}

// 加载玩家基础信息
func loadPlayerData(playerId int64) (*models.Player, proto_login.LoginResult) {
	model := &models.Player{}
	query := mongo.NewIQ("game", "player")
	if err := query.Where("_id", playerId).Find(model).Error(); err != nil {
		log.Error("login.login: find %v", err)
		return nil, proto_login.LoginDBError
	}

	if query.Result() == 0 {
		model.Init(playerId)
		query.Clear()
		if err := query.Insert(model).Error(); err != nil {
			log.Error("login.login insert: %v", err)
			return nil, proto_login.LoginDBError
		}
	}

	return model, proto_login.LoginSuccess
}

func (l *Login) logout(playerId int64) error {
	pid, ok := l.players[playerId]
	if ok {
		l.Context.Call(pid, &messages.Logout{})
		delete(l.players, playerId)
	}
	return nil
}

func (l *Login) disconnect(playerId int64) error {
	pid, ok := l.players[playerId]
	if ok {
		l.Context.Call(pid, &messages.Disconnect{})
	}
	return nil
}

func (l *Login) castAgent(playerId int64, msg interface{}) error {
	if playerId == 0 {
		for _, pid := range l.players {
			l.Context.Cast(pid, msg)
		}
	} else {
		pid, ok := l.players[playerId]
		if ok {
			l.Context.Cast(pid, msg)
		}
	}

	return nil
}
