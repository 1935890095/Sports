package log

import (
	"fmt"
)

type Logger interface {
	Debug(format string, v ...interface{})
	Info(format string, v ...interface{})
	Warning(format string, v ...interface{})
	Error(format string, v ...interface{})
	Fatal(format string, v ...interface{})
}

type BaseLogger interface {
	IsEnabled(level Level) bool
	Log(level Level, v ...interface{})
}

type chainedLogger struct {
	loggers     []BaseLogger
	loggerCount int
}

func newChainedLogger(loggers ...BaseLogger) *chainedLogger {
	return &chainedLogger{
		loggers:     loggers,
		loggerCount: len(loggers),
	}
}

func (chained *chainedLogger) AddLogger(logger BaseLogger) {
	chained.loggers = append(chained.loggers, logger)
	chained.loggerCount = len(chained.loggers)
}

func (chained *chainedLogger) IsEnabled(level Level) bool { return true }

func (chained *chainedLogger) Debug(format string, v ...interface{}) {
	chained.log(LEVEL_DEBUG, fmt.Sprintf(format, v...))
}

func (chained *chainedLogger) Info(format string, v ...interface{}) {
	chained.log(LEVEL_INFO, fmt.Sprintf(format, v...))
}

func (chained *chainedLogger) Warning(format string, v ...interface{}) {
	chained.log(LEVEL_WARNING, fmt.Sprintf(format, v...))
}

func (chained *chainedLogger) Error(format string, v ...interface{}) {
	chained.log(LEVEL_ERROR, fmt.Sprintf(format, v...))
}

func (chained *chainedLogger) Fatal(format string, v ...interface{}) {
	chained.log(LEVEL_FATAL, fmt.Sprintf(format, v...))
}

func (chained *chainedLogger) log(level Level, v ...interface{}) {
	for i := 0; i < chained.loggerCount; i++ {
		if chained.loggers[i].IsEnabled(level) {
			chained.loggers[i].Log(level, v...)
		}
	}
}
