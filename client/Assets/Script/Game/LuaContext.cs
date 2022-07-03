/********************************************************
    Id: LuaContext.cs
    Desc: lua层Context全局上下文在cs层的绑定
    Author: figo
    Date: 2020-03-21 12:35:34
*********************************************************/
using System;
using XLua;

namespace XFX.Game {
    [CSharpCallLua]
    public interface LuaContext {
        // 加载
        void load();

        // 卸载
        void unload();

        // 驱动，对应lua层Context.Loop
        void loop(float deltaTime = 0f, float unscaledDeltaTime = 0f, float realtimeSinceStartup = 0f);

        // lua层提供cs层调用的通用接口，对应lua层的Context.Invoke
        void invoke(string type, string id, string func, params object[] args);

        void print_func_ref_by_csharp();
    }
}

