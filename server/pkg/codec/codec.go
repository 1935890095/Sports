package codec

import "bytes"

const (
	HeadLength    = 6
	MaxPacketSize = 64 * 1024
)

// A Decoder reads and decodes network data slice
type Decoder struct {
	buf  *bytes.Buffer
	fb   int // dog flag
	size int // last packet length
	typ  int // last packet type(id)
}

func NewDecoder() *Decoder {
	return &Decoder{
		buf:  bytes.NewBuffer(nil),
		size: -1,
	}
}

// Decode decode the network bytes slice to packet.Packet(s)
// TODO(Warning): shared slice
func (c *Decoder) Decode(data []byte) ([]*Packet, error) {
	c.buf.Write(data)

	var (
		packets []*Packet
		err     error
	)

	if c.buf.Len() < HeadLength {
		return nil, err
	}

	// first time
	if c.size < 0 {
		if err = c.forward(); err != nil {
			return nil, err
		}
	}

	for c.size <= c.buf.Len() {
		p := &Packet{Type: c.typ, Length: c.size, Data: c.buf.Next(c.size)}
		packets = append(packets, p)

		// more packet
		if c.buf.Len() < HeadLength {
			c.size = -1
			break
		}

		if err = c.forward(); err != nil {
			return nil, err
		}
	}
	return packets, err
}

func (c *Decoder) forward() error {
	header := c.buf.Next(HeadLength)

	c.size = bytesToInt(header[:2])
	c.size -= HeadLength

	c.fb = bytesToInt(header[2:4])
	if c.fb != 0xFB {
		return ErrWrongCheckFbFlag
	}

	c.typ = bytesToInt(header[4:])
	if c.typ <= 0 {
		return ErrPacketWrongType
	}

	// packet length limitation
	if c.size > MaxPacketSize {
		return ErrPacketSizeExceed
	}
	return nil
}

// Encode:
// Procotol: --<length>--|--<FB>--|--<type>--
func Encode(typ int, data []byte) ([]byte, error) {
	p := &Packet{Type: typ, Length: len(data)}
	buf := make([]byte, p.Length+HeadLength)

	copy(buf[:2], intToBytes(p.Length+HeadLength))
	copy(buf[2:4], intToBytes(0xFB))
	copy(buf[4:HeadLength], intToBytes(typ))
	copy(buf[HeadLength:], data)
	return buf, nil
}

// Decode packet data length byte to int(Big end)
func bytesToInt(b []byte) int {
	result := 0
	for _, v := range b {
		result = result<<8 + int(v)
	}
	return result
}

// Encode packet data length to bytes(Big end)
func intToBytes(n int) []byte {
	buf := make([]byte, 2)
	buf[0] = byte((n >> 8) & 0xFF)
	buf[1] = byte(n & 0xFF)
	return buf
}
