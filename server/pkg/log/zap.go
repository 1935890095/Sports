package log

import (
	"runtime"
	"strconv"
	"strings"
	"xiao/pkg/env"

	"github.com/natefinch/lumberjack"
	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
)

type zapLogger struct {
	logger *zap.SugaredLogger
	level  Level
}

func NewZapLogger(level Level, env *env.Env) BaseLogger {
	if !env.Log.WriteFile {
		return nil
	}

	writeSyncer := getLogWriter(env)
	encoder := getEncoder()
	core := zapcore.NewCore(encoder, writeSyncer, getlevel(level))
	logger := zap.New(core)

	return &zapLogger{
		logger: logger.Sugar(),
		level:  level,
	}
}

func (zlog *zapLogger) IsEnabled(level Level) bool {
	return level >= zlog.level
}

func (zlog *zapLogger) Log(level Level, v ...interface{}) {
	f := formatLine()
	switch level {
	case LEVEL_DEBUG:
		zlog.logger.Debug(f, v)
	case LEVEL_INFO:
		zlog.logger.Info(f, v)
	case LEVEL_WARNING:
		zlog.logger.Warn(f, v)
	case LEVEL_ERROR:
		zlog.logger.Error(f, v)
	case LEVEL_FATAL:
		zlog.logger.Fatal(f, v)
	default:
	}

}

func getEncoder() zapcore.Encoder {
	encoderConfig := zap.NewProductionEncoderConfig()
	encoderConfig.EncodeTime = zapcore.ISO8601TimeEncoder
	encoderConfig.EncodeLevel = zapcore.CapitalLevelEncoder
	// encoder := zapcore.NewJSONEncoder(zap.NewProductionEncoderConfig())
	return zapcore.NewConsoleEncoder(encoderConfig)
}

func getLogWriter(env *env.Env) zapcore.WriteSyncer {
	lumberJackLogger := &lumberjack.Logger{
		Filename:   env.Log.Path,
		MaxSize:    env.Log.MaxSize,
		MaxBackups: env.Log.MaxBackups,
		MaxAge:     env.Log.MaxAge,
		Compress:   env.Log.Compress,
	}
	return zapcore.AddSync(lumberJackLogger)
}

func getlevel(level Level) zapcore.LevelEnabler {
	switch level {
	case LEVEL_DEBUG:
		return zapcore.DebugLevel
	case LEVEL_INFO:
		return zapcore.InfoLevel
	case LEVEL_WARNING:
		return zapcore.WarnLevel
	case LEVEL_ERROR:
		return zapcore.ErrorLevel
	case LEVEL_FATAL:
		return zapcore.FatalLevel
	default:
		return zapcore.DebugLevel
	}
}

func formatLine() string {
	var file string
	var line int
	var ok bool
	pc := make([]uintptr, 10)
	n := runtime.Callers(2, pc)
	for i := 0; i < n; i++ {
		f := runtime.FuncForPC(pc[i])
		file, line = f.FileLine(pc[i])
		if !strings.Contains(file, "pkg/log") {
			ok = true
			break
		}
	}

	if !ok {
		file = "???"
		line = 0
	}
	short := file
	for i := len(file) - 1; i > 0; i-- {
		if file[i] == '/' {
			short = file[i+1:]
			break
		}
	}
	file = short
	lineString := strconv.Itoa(line)
	return file + ":" + lineString + " "
}
