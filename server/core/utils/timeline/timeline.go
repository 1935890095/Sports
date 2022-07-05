package timeline

import (
	"fmt"
	"strings"
	"time"
)

type TimeKind int

const (
	TimeKindUnknown TimeKind = iota
	TimeKindAllday           // 全天候
	TimeKindDaily            // 每天型
	TimeKindWeekly           // 每周型
	TimeKindHoliday          // 节日型
)

type Timeline interface {
	Kind() TimeKind                                                                               // 时间类型
	Over() bool                                                                                   // 是否结束了
	Add(begin int, offset time.Duration, proc func(ud interface{}) bool, ud interface{}) Timeline // 调度
	Shift(t time.Duration)                                                                        // 偏移
	Run()                                                                                         // 驱动
	BeginTime() time.Time                                                                         // 开始时间
	EndTime() time.Time                                                                           // 结束时间
}

type taskInfo struct {
	begin  int                       // offset from begin or end
	offset time.Duration             // offset time
	proc   func(ud interface{}) bool // work proc
	ud     interface{}               // userdata
	magic  int                       // magic digits
}

type timeline struct {
	kind       TimeKind
	beginDay   int
	beginClock string
	endDay     int
	endClock   string
	beginTime  time.Time
	endTime    time.Time
	version    int
	over       bool
	tasks      []*taskInfo
	magic      int           // magic digits, 0关1开2关3开...
	shift      time.Duration // shift time
}

func NewTimeline(beginDay int, beginClock string, endDay int, endClock string) Timeline {
	if len(strings.Split(beginClock, ":")) == 2 {
		beginClock = beginClock + ":00"
	}
	if len(strings.Split(endClock, ":")) == 2 {
		endClock = endClock + ":00"
	}

	tl := &timeline{
		beginDay:   beginDay,
		beginClock: beginClock,
		endDay:     endDay,
		endClock:   endClock,
		kind:       TimeKindUnknown,
		over:       false,
	}

	bKind := tl.getKind(beginDay, beginClock)
	eKind := tl.getKind(endDay, endClock)
	if bKind != TimeKindUnknown && bKind == eKind {
		tl.kind = bKind
		tl.refresh(time.Now())
	}
	// fmt.Printf("* time kind %v\n", tl.kind)

	return tl
}

func (l *timeline) Kind() TimeKind       { return l.kind }
func (l *timeline) Over() bool           { return l.over }
func (l *timeline) BeginTime() time.Time { return l.beginTime }
func (l *timeline) EndTime() time.Time   { return l.endTime }

func (l *timeline) Add(begin int, offset time.Duration, proc func(ud interface{}) bool, ud interface{}) Timeline {
	task := &taskInfo{
		begin:  begin,
		offset: offset,
		proc:   proc,
		ud:     ud,
		magic:  -1,
	}
	l.tasks = append(l.tasks, task)
	return l
}

func (l *timeline) Run() {
	now := time.Now()

	if l.over {
		return
	}

	switch l.kind {
	case TimeKindUnknown:
		return // invalid
	case TimeKindAllday:
		l.processTasks(func(task *taskInfo) bool {
			return task.begin != 0 && task.offset == 0
		})
	case TimeKindDaily, TimeKindWeekly, TimeKindHoliday:
		if !now.Before(l.beginTime) && !now.After(l.endTime) {
			if l.magic%2 == 0 {
				l.magic++
				// fmt.Printf("* magic inc %v\n", l.magic)
			}
		} else {
			if l.magic%2 == 1 {
				l.magic++
				// fmt.Printf("* magic inc %v\n", l.magic)
			}
		}
		l.processTasks(func(task *taskInfo) bool {
			bt := l.beginTime.Add(task.offset)
			et := l.endTime.Add(task.offset)
			if l.magic%2 == 0 {
				if task.begin != 0 && task.offset < 0 && !now.Before(bt) {
					return true
				}
				if l.magic > 0 { // Important!
					if task.begin == 0 && task.offset >= 0 && !now.Before(et) {
						return true
					}
				}
			} else {
				if task.begin != 0 && task.offset >= 0 && !now.Before(bt) {
					return true
				}
				if task.begin == 0 && task.offset < 0 && !now.Before(et) {
					return true
				}
			}
			return false
		})
	}

	l.refresh(now)
}

// shift number of days
func (l *timeline) Shift(shift time.Duration) { l.shift = shift }

func (l *timeline) processTasks(check func(task *taskInfo) bool) {
	for i := 0; i < len(l.tasks); i++ {
		task := l.tasks[i]
		if task.magic >= l.magic {
			continue
		}
		if check(task) {
			task.magic = l.magic
			if !task.proc(task.ud) {
				l.tasks = append(l.tasks[:i], l.tasks[i+1:]...)
				// fmt.Printf("* remove task, count %d\n", len(l.tasks))
				i--
			}
		}
	}
}

// refresh is flush the time of timeline, at least one minute
func (l *timeline) refresh(now time.Time) {
	year, month, day := now.Date()
	version := year*10000 + int(month)*100 + day

	if l.version != version {
		switch l.kind {
		case TimeKindAllday:
			break
		case TimeKindDaily, TimeKindHoliday:
			l.beginTime, _ = l.getDayTime(l.beginDay, l.beginClock, now)
			l.endTime, _ = l.getDayTime(l.endDay, l.endClock, now)
		case TimeKindWeekly:
			l.beginTime, _ = l.getWeekTime(l.beginDay, l.beginClock, now)
			l.endTime, _ = l.getWeekTime(l.endDay, l.endClock, now)
		}
		l.version = version
		// fmt.Printf("refrest begin time %v, end time %v\n", l.beginTime, l.endTime)
	}

	if l.kind == TimeKindHoliday && now.After(l.endTime) {
		l.over = true
	}
}

func (l *timeline) getWeekTime(weekday int, clock string, referTime time.Time) (time.Time, error) {
	y, m, d := referTime.Date()
	value := fmt.Sprintf("%d-%02d-%02d %s", y, m, d, clock)
	t, e := time.ParseInLocation("2006-01-02 15:04:05", value, time.Local)
	if e != nil {
		return t, e
	}
	wd := t.Weekday()
	if wd == time.Sunday {
		wd = 7
	}
	offset := weekday - int(wd)
	return t.AddDate(0, 0, offset).Add(l.shift), nil
}

func (l *timeline) getDayTime(day int, clock string, referTime time.Time) (time.Time, error) {
	y, m, d := referTime.Date()
	if day != 0 {
		y, day = day/10000, day%10000
		m, d = time.Month(day/100), day%100
	}
	value := fmt.Sprintf("%d-%02d-%02d %s", y, m, d, clock)
	t, e := time.ParseInLocation("2006-01-02 15:04:05", value, time.Local)
	if e != nil {
		return t, e
	}
	return t.Add(l.shift), nil
}

func (l *timeline) getKind(day int, clock string) TimeKind {
	kind := TimeKindUnknown
	y, m, d := time.Now().Date()
	if day == 0 && clock == "" {
		kind = TimeKindAllday
	}
	if day == 0 && clock != "" {
		value := fmt.Sprintf("%d-%02d-%02d %s", y, m, d, clock)
		_, err := time.ParseInLocation("2006-01-02 15:04:05", value, time.Local)
		if err == nil {
			kind = TimeKindDaily
		}
	}
	if day > 0 && day <= 7 {
		value := fmt.Sprintf("%d-%02d-%02d %s", y, m, d, clock)
		_, err := time.ParseInLocation("2006-01-02 15:04:05", value, time.Local)
		if err == nil {
			kind = TimeKindWeekly
		}
	}
	if day >= 19700101 {
		y, day = day/10000, day%10000
		m, d = time.Month(day/100), day%100
		value := fmt.Sprintf("%d-%02d-%02d %s", y, m, d, clock)
		_, err := time.ParseInLocation("2006-01-02 15:04:05", value, time.Local)
		if err == nil {
			kind = TimeKindHoliday
		}
	}
	return kind
}
