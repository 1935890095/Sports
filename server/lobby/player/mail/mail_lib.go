package mail

import (
	"xiao/core/models"
)

type mailSort []models.PrivateMail

func (m mailSort) Len() int           { return len(m) }
func (m mailSort) Swap(i, j int)      { m[i], m[j] = m[j], m[i] }
func (m mailSort) Less(i, j int) bool { return m[i].CreateTime < m[j].CreateTime }
