package log

import (
	"fmt"
	"os"
	"runtime"
	"strings"
	"sync"
	"time"
)

const (
	colorBlack = 30 + iota
	colorRed
	colorGreen
	colorYellow
	colorBlue
	colorMagenta
	colorCyan
	colorWhite
	colorGray = colorWhite

	colorReset = 0
)

const (
	toBase8         = "\x1b[%dm%s\x1b[0m"
	toBase16Bright  = "\x1b[%d;1m%s\x1b[0m"
	toBase256       = "\x1b[38;5;%dm%s\x1b[0m"
	toBase256Bright = "\x1b[38;5;%d;1m%s\x1b[0m"
)

type consoleLogger struct {
	mu    sync.Mutex
	buf   []byte
	level Level
}

func (console *consoleLogger) IsEnabled(level Level) bool {
	return level >= console.level
}

func (console *consoleLogger) Log(level Level, v ...interface{}) {
	console.log(level, fmt.Sprint(v...))
}

func (console *consoleLogger) log(level Level, s string) error {
	now := time.Now()
	var file string
	var line int
	var ok bool
	console.mu.Lock()
	defer console.mu.Unlock()

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

	console.buf = console.buf[:0]

	colorCode := console.getColorCode(level)
	console.buf = append(console.buf, console.formatColor(fmt.Sprintf("[%s]", level), colorCode)...)
	console.buf = append(console.buf, " "...)
	console.formatHeader(&console.buf, now, file, line)
	console.buf = append(console.buf, s...)
	if len(s) == 0 || s[len(s)-1] != '\n' {
		console.buf = append(console.buf, '\n')
	}
	_, err := os.Stdout.Write(console.buf)
	return err
}

func (console *consoleLogger) getColorCode(level Level) int {
	switch level {
	case LEVEL_DEBUG:
		return colorWhite
	case LEVEL_INFO:
		return colorCyan
	case LEVEL_WARNING:
		return colorYellow
	case LEVEL_ERROR:
		return colorRed
	case LEVEL_FATAL:
		return colorRed + 10
	case LEVEL_NONE:
		return colorGray
	default:
		return colorWhite
	}
}

func (console *consoleLogger) formatColor(s string, colorCode int) string {
	format := toBase8

	if colorCode < colorBlack || colorCode > 10+colorWhite {
		format = toBase256
	}
	return fmt.Sprintf(format, colorCode, s)
}

func (console *consoleLogger) formatHeader(buf *[]byte, t time.Time, file string, line int) {
	year, month, day := t.Date()
	itoa(buf, year, 4)
	*buf = append(*buf, '/')
	itoa(buf, int(month), 2)
	*buf = append(*buf, '/')
	itoa(buf, day, 2)
	*buf = append(*buf, ' ')

	hour, min, sec := t.Clock()
	itoa(buf, hour, 2)
	*buf = append(*buf, ':')
	itoa(buf, min, 2)
	*buf = append(*buf, ':')
	itoa(buf, sec, 2)
	*buf = append(*buf, '.')
	itoa(buf, t.Nanosecond()/1e3, 6)
	*buf = append(*buf, ' ')

	short := file
	for i := len(file) - 1; i > 0; i-- {
		if file[i] == '/' {
			short = file[i+1:]
			break
		}
	}
	file = short

	*buf = append(*buf, file...)
	*buf = append(*buf, ':')
	itoa(buf, line, -1)
	*buf = append(*buf, ": "...)
}

func itoa(buf *[]byte, i int, wid int) {
	// Assemble decimal in reverse order.
	var b [20]byte
	bp := len(b) - 1
	for i >= 10 || wid > 1 {
		wid--
		q := i / 10
		b[bp] = byte('0' + i - q*10)
		bp--
		i = q
	}
	// i < 10
	b[bp] = byte('0' + i)
	*buf = append(*buf, b[bp:]...)
}

func NewConsoleLogger(level Level) BaseLogger {
	return &consoleLogger{
		level: level,
	}
}
