/********************************************************
    id: Resource.cs
    desc: 资源管理器
    author: figo
    date: 2018/09/10 10:32:15

    Copyright (C) 2018 zwwx Ltd. All rights reserved.
*********************************************************/
using System;
using ZF.Core.Render;

namespace ZF.Game {
    class Resource : Instance, IRenderFactory {
        private IRenderFactory factory = null;

        protected override void OnInit() {
            RenderFactory.Settings.ReplaceShaderInEditorMode = true;
            this.factory = new RenderFactory();
            this.factory.lingerTime = 30;
            // 启用RenderInstance资源创建全局接口
            RenderInstance.Setup();
        }

        protected override void OnDestroy() { }

        protected override void OnUpdate() {
            factory.Loop();
        }

        //--------- IRenderFactory Beg ---------
        public int concurrency {
            get { return factory.concurrency; }
            set { factory.concurrency = value; }
        }

        public int lingerTime {
            get { return factory.lingerTime; }
            set { factory.lingerTime = value; }
        }

        public T CreateInstance<T>(string filename, IRenderObject parent,
            int priority, string tag) where T : IRenderObject {
            return factory.CreateInstance<T>(filename, parent, priority, tag);
        }

        public IRenderObject CreateInstance(Type type, string filename, IRenderObject parent,
            int priority, string tag) {
            return factory.CreateInstance(type, filename, parent, priority, tag);
        }

        void IRenderFactory.RemoveResource(IRenderResource resource) {
            factory.RemoveResource(resource);
        }

        void IRenderFactory.Loop() {
            //factory.Loop();
        }
        bool IRenderFactory.isIdle {
            get { return this.factory.isIdle; }
        }
        //--------- IRenderFactory End ---------
    }
}