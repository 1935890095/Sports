--[[ This file is generated by tools, do not modify it manully! ]]

local LoginView = { 
	self,           --[[GameObject]]
	pivot,          --[[GameObject]]
	account,        --[[Image]]
	password,       --[[Image]]
	enter,          --[[Image]]
}
LoginView.__name = "LoginView"

function LoginView.load(view, parent, path)
	parent, path = parent or 0, path or ""
	local self = clone(LoginView)
	self.self = G.api.View.Lookup(view, parent, path, "Canvas")
	self.pivot = G.api.View.Lookup(view, self.self, "pivot", "GameObject")
	self.account = G.api.View.Lookup(view, self.self, "pivot/down/account", "Image")
	self.password = G.api.View.Lookup(view, self.self, "pivot/down/password", "Image")
	self.enter = G.api.View.Lookup(view, self.self, "pivot/down/enter", "Image")
	return self
end

return LoginView
