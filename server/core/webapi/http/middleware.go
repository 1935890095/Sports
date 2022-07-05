package http

type HttpMiddleware func(next HttpHandler) HttpHandler
