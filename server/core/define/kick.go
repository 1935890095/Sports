package define

type KickReason int32

const (
	KickReasonNone  KickReason = iota
	KickReasonClose            // 房间解散
)
