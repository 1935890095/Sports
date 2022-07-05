package log

import (
	"xiao/pkg/env"
)

var logger = newChainedLogger()

func Init(env *env.Env) {
	AddLogger(NewConsoleLogger(LEVEL_DEBUG))
	AddLogger(NewZapLogger(LEVEL_DEBUG, env))
}

func AddLogger(l BaseLogger) {
	if l == nil {
		return
	}
	logger.AddLogger(l)
}

func Debug(format string, v ...interface{}) {
	logger.Debug(format, v...)
}

func Info(format string, v ...interface{}) {
	logger.Info(format, v...)
}

func Warning(format string, v ...interface{}) {
	logger.Warning(format, v...)
}

func Error(format string, v ...interface{}) {
	logger.Error(format, v...)
}

func Fatal(format string, v ...interface{}) {
	logger.Fatal(format, v...)
}
