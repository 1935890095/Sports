local class = class or require("core.Class").class
local Component = class()

function Component:ctor() 
    self.render = nil
end

function Component:onCreate() end
function Component:onStart() end
function Component:onDestroy() end
function Component:onEnabled() end
function Component:onDisable() end
function Component:onUpdate() end
function Component:onCommand(cmd, ...) end

return Component