/********************************************************
    Id: LuaComponent.cs
    Desc: 面向lua的资源组件
    Author: figo
    Date: 2020-07-29 15:09:25
*********************************************************/
using System.Collections.Generic;
using ZF.Core.Render;
using ZF.Game;
using XLua;

namespace ZF.Game {

    public abstract class LuaComponent :  RenderComponent {
        public LuaContext context { get; set; }
        public LuaTable table { get; set; }

        protected sealed override void OnCreate() {
            table.Set("render", this.renderObject);
            table.Set("this", (IRenderComponent)this);
            context.invoke("component", "", "onCreate", table);
        }
        protected sealed override void OnDestroy() { 
            context.invoke("component", "", "onDestroy", table);
            table.Dispose();
        }
        protected sealed override void OnStart() { context.invoke("component", "", "onStart", table); }
        protected sealed override void OnUpdate() { context.invoke("component", "", "onUpdate", table); }
        protected sealed override void OnEnabled() { context.invoke("component", "", "onEnabled", table); }
        protected sealed override void OnDisable() { context.invoke("component", "", "onDisable", table); }
        protected sealed override void OnCommand(string cmd,  params object[] args) {
            switch (args.Length) {
                case 0: context.invoke("component", "", "onCommand", table, cmd); break;
                case 1: context.invoke("component", "", "onCommand", table, cmd, args[0]); break;
                case 2: context.invoke("component", "", "onCommand", table, cmd, args[0], args[1]); break;
                case 3: context.invoke("component", "", "onCommand", table, cmd, args[0], args[1], args[2]); break;
                case 4: context.invoke("component", "", "onCommand", table, cmd, args[0], args[1], args[2], args[3]); break;
                default: throw new System.Exception("LuaComponent.OnCommand too many arguments");
            }
        }

        #region [types]
        class T01 : LuaComponent { }
        class T02 : LuaComponent { }
        class T03 : LuaComponent { }
        class T04 : LuaComponent { }
        class T05 : LuaComponent { }
        class T06 : LuaComponent { }
        class T07 : LuaComponent { }
        class T08 : LuaComponent { }
        class T09 : LuaComponent { }
        class T10 : LuaComponent { }
        class T11 : LuaComponent { }
        class T12 : LuaComponent { }
        class T13 : LuaComponent { }
        class T14 : LuaComponent { }
        class T15 : LuaComponent { }
        class T16 : LuaComponent { }
        class T17 : LuaComponent { }
        class T18 : LuaComponent { }
        class T19 : LuaComponent { }
        class T20 : LuaComponent { }

        #endregion

        public class Pool : RenderComponent {
            Dictionary<string, System.Type> name2type = new Dictionary<string, System.Type>();
            int num = 0;
            System.Type[] types = new System.Type[] {
                typeof (T01),
                typeof (T02),
                typeof (T03),
                typeof (T04),
                typeof (T05),
                typeof (T06),
                typeof (T07),
                typeof (T08),
                typeof (T09),
                typeof (T10),
                typeof (T11),
                typeof (T12),
                typeof (T13),
                typeof (T14),
                typeof (T15),
                typeof (T16),
                typeof (T17),
                typeof (T18),
                typeof (T19),
                typeof (T20),
            };

            public System.Type Alloc(string name) {
                if (name2type.ContainsKey(name))
                    throw new System.Exception("alloc lua component duplicated: " + name);
                if (num >= types.Length)
                    throw new System.Exception("alloc lua component out of size: " + num);
                var type = types[num++];
                name2type.Add(name, type);
                return type;
            }

            public void Free(string name) {
                throw new System.NotImplementedException("free lua componnent");
            }

            public System.Type Get(string name) {
                System.Type type;
                if (name2type.TryGetValue(name, out type)) 
                    return type;
                return null;
            }
        }
    }
}

public static class IRenderObjectExtension {
    public static object AddComponent(this IRenderObject obj, LuaTable cls, params object[] args) {
        // Log.Debug("############## extension method: addcomponent");
        object result = null;
        LuaFunction fun = null;
        try {
            var pool = obj.GetComponent<LuaComponent.Pool>();
            if (pool == null)  {
                pool = obj.AddComponent<LuaComponent.Pool>();
                pool.enabled = false;
            }

            string name = cls.Get<string>("__name");
            var com = obj.AddComponent(pool.Alloc(name)) as LuaComponent;
            fun = cls.Get<LuaFunction>("new");
            result = fun.Call(args)[0];
            com.context = LuaApi.context;
            com.table = result as LuaTable;
            fun.Dispose();
        } catch(System.Exception e) {
            Log.Error(e.ToString());
        } finally {
            if (fun != null) fun.Dispose();
            if (cls != null) cls.Dispose();
        }
        return result;
    }

    public static object GetComponent(this IRenderObject obj, LuaTable cls) {
        var pool = obj.GetComponent<LuaComponent.Pool>();
        if (pool == null) return null;

        try {
            string name = cls.Get<string>("__name");
            var type = pool.Get(name);
            if (type == null) return null;
            var com = obj.GetComponent(type) as LuaComponent;
            if (com != null) return com.table;
        } catch (System.Exception e) {
            Log.Error(e.ToString());
        } finally {
            if (cls != null) cls.Dispose();
        }

        return null;
    }

    public static void RemoveComponent(this IRenderObject obj, LuaTable cls) {
        var pool = obj.GetComponent<LuaComponent.Pool>();
        if (pool == null) return;
        try {
            string name = cls.Get<string>("__name");
            var type = pool.Get(name);
            if (type == null) return;
            obj.RemoveComponent(type);
        } catch (System.Exception e) {
            Log.Error(e.ToString());
        } finally {
            if (cls != null) cls.Dispose();
        }
    }
}