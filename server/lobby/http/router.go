package http

func (http *HttpModule) register() {
	http.router.Get("/announce", http.getAnnounce)
	http.router.Get("/newAnnounce", http.newAnnounce)
	http.router.Get("/newNotice", http.newNotice)
	http.router.Get("/functionOpen", http.functionOpen)
}
