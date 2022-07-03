
local json = require("dkjson")
local Http = class()

local encode
local decode
local onResponse

function Http:ctor(router)
    self.router = router
end

function Http:get(url, response, error)
    G.api.Http.Get(url, onResponse(self, url, response), error)
end

function Http:post(url, body, response, error)
    G.api.Http.Post(url, encode(body), onResponse(self, url, response), error)
end

onResponse = function(self, url, response)
    return function(body)
        response(decode(body))
    end
end

encode = function(data)
    return json.encode(data)
end

decode = function(data)
    return json.decode(data)
end

return Http
