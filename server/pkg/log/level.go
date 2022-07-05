package log

type Level int

const (
	LEVEL_DEBUG Level = iota
	LEVEL_INFO
	LEVEL_WARNING
	LEVEL_ERROR
	LEVEL_FATAL
	LEVEL_NONE
)

func (level Level) String() string {
	return [...]string{"DBUG", "INFO", "WARN", "ERRO", "FTAL", "NONE"}[level]
}
