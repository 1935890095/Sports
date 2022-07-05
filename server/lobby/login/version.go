package login

const (
	CheckVersionResultOk  = 0
	CheckVersionResultLow = 1
)

// 版本检查
func CheckVersion(appVersion, resVersion string) int {
	return CheckVersionResultOk
}
