
namespace XFX.Game {
    using UnityEngine;
    using XFX.Core.Render;
    using XFX.Asset;
    using XFX.Core.Util;
    using Component = UnityEngine.Component;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEngine.UI;

	public class LuaView : RenderComponent, IView {
        private string name;
        private LuaContext context;
        private Map<int, Component> children = new Map<int, Component>();
        private Empty noderoot;
        private Map<int, Empty> nodes;
        private Map<string, SpriteAtlas> atlas;
        private bool pendingshow = false;
        private bool pendinghide = false;
        private bool pendingdestory = false;


        // 创建一个View，返回用于标识该View的id
        public static LuaView Create(LuaContext context, IRoot root, string name) {
            View view = RenderInstance.Create<View>(string.Format("res/ui/{0}.ui", name.ToLower()), root);
            view.destroyChildrenOnDestroy = true;
            var c = view.AddComponent<LuaView>();
            c.context = context;
            c.name = name;
            return c;
        }

        public new bool enabled {
            get { return base.enabled; }
            set { }
        }

        public void Show() { 
            if (this.renderObject != null) this.renderObject.active = true; 
            else this.pendingshow = true;
            if (this.pendinghide) this.pendinghide = false;
        }
        public void Hide() {
            if (this.renderObject != null) this.renderObject.active = false; 
            else this.pendinghide = true;
            if (this.pendingshow) this.pendingshow = false;
        }
        public bool isShow { 
            get { 
                if (this.renderObject != null) return this.renderObject.active;
                if (this.pendingshow) return true;
                if (this.pendinghide) return false;
                return false;
            }
        }
        public void Destroy() { 
            if (this.renderObject != null) this.renderObject.Destroy(); 
            else {
                this.pendingdestory = true;
                this.pendingshow = false;
                this.pendinghide = false;
            }

            if (atlas != null) {
                foreach (var item in atlas) {
                    item.Value.Destroy();   
                }
            }
        }
        protected sealed override void OnCreate() { 
            this.context.invoke("view", name, "onCreate");  
            if (this.pendingdestory) {
                this.Destroy();
                this.pendingdestory = false;
            }
            if (this.pendingshow) {
                this.Show();
                this.pendingshow = false;
            }
            if (this.pendinghide) {
                this.Hide();
                this.pendinghide = false;
            }

            if (Application.isEditor && RenderFactory.Settings.ReplaceShaderInEditorMode) {
                var images = this.renderObject.gameObject.GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    var mat = img.GetModifiedMaterial(img.material);
                    if (mat.shader.name.Contains("ZFU")) {
                        var shader = Shader.Find(mat.shader.name);
                        if (shader) {
                            mat.shader = shader;
                        }
                    }
                }
            }
        }
        protected sealed override void OnPrepare() { this.context.invoke("view", name, "load"); }
        protected sealed override void OnStart() { this.context.invoke("view", name, "onStart"); }
        protected sealed override void OnDestroy() { this.context.invoke("view", name, "onDestroy"); }
        protected sealed override void OnUpdate() { this.context.invoke("view", name, "onUpdate"); }
        protected sealed override void OnEnabled() { this.context.invoke("view", name, "onEnabled"); }
        protected sealed override void OnDisable() { this.context.invoke("view", name, "onDisable"); }

        public int Lookup(int parentid, string path, string type) {
            int id = 0;
            GameObject parent = null;

            if (parentid != 0) parent = Get(parentid);
            else if (this.renderObject != null) parent = this.renderObject.gameObject;

            if (parent != null) {
                GameObject go = null;
                if  (string.IsNullOrEmpty(path)) {
                    go = parent;
                } else {
                    var trans = parent.transform.Find(path);
                    if (trans!= null) { go = trans.gameObject; }
                }
                if (go != null) {
                    id = go.GetInstanceID();
                    Component component = null;
                    if (type != "GameObject") component = go.GetComponent(type);
                    if (component == null) component = go.transform;
                    // children.Add(id, component);
                    children[id] = component;
                }
            }
            if (id == 0) {
                Log.Warn("View: {0} lookup path: {1} failed", this.name, path);
            }
            return id;
        }

        public int Find(string path) {
            GameObject parent = this.renderObject.gameObject;
            GameObject go = null;
            if(parent == null)
            {
                Log.Warn("View: Find gameobject failed");
            }
            if  (string.IsNullOrEmpty(path)) {
                go = parent;
            } else {
                var trans = parent.transform.Find(path);
                if (trans!= null) { go = trans.gameObject; }
            }
            if (go != null) {
                return go.GetInstanceID();
            }
            return 0;
        }

        public GameObject Get(int id) {
            Component component;
            children.TryGetValue(id, out component);
            if (component != null) return component.gameObject;
            else return null;
        }

        public T Get<T>(int id) where T : Component {
            Component component;
            children.TryGetValue(id, out component);
            if (component != null) {
                if (component is T ) {
                    return component as T;
                } else {
                    var go = component.gameObject;
                    return go.GetComponent<T>();
                }
            }
            else return null;
        }

        public int Clone(int id) {
            int clonedid = 0;
            var go =  Get(id);
            if (go != null) {
                var cloned = UnityEngine.GameObject.Instantiate(go) as GameObject;
                if (cloned) {
                    cloned.transform.SetParent(go.transform.parent);
                    cloned.transform.localPosition = Vector3.zero;
                    cloned.transform.localRotation = Quaternion.identity;
                    cloned.transform.localScale = Vector3.one;
                    cloned.layer = go.transform.parent.gameObject.layer;
                    clonedid = cloned.GetInstanceID();
                    children.Add(clonedid, cloned.transform);
                }
            }
            return clonedid;
        }

        public void Delete(int id) {
            var go = Get(id);
            if (go != null) {
                children.Remove(id);
                if (null != nodes && nodes.ContainsKey(id)) { 
                    var node = nodes[id];
                    node.Destroy();
                    nodes.Remove(id);
                }
                UnityEngine.Object.Destroy(go);
            }
        }

        public SpriteAtlas CreateAtlas(string name) { 
            if (null == atlas)
                atlas = new Map<string, SpriteAtlas>();

            SpriteAtlas result = null;
            if (atlas.TryGetValue(name, out result)) {
                return result;
            }

            result = RenderInstance.Create<SpriteAtlas>(string.Format("res/ui/atlas/{0}.atlas", name), noderoot);
            atlas.Add(name, result);
            return result;
        }

        public T CreateAsset<T>(int id, string fileName) where T : IRenderObject {
            if (string.IsNullOrEmpty(fileName)) return default(T);

            var node = GetNode(id);
            if (null == node) return default(T);

            var obj = RenderInstance.Create<T>(fileName, node);
            obj.layer = XFX.Misc.Defines.Layer.UI;
            return obj;
        }

        public ICanvas CreateCanvas(int id)
        {
            var node = GetNode(id);
            if (null == node) return null;
            var canvas = FaceCanvas.Create(node.gameObject.name, node);
            return canvas;
        }

        public IVideoPlayer CreateVideoPlayer(int id) {
            var node = GetNode(id);
            if (null == node) return null;
            var player = VideoPlayer.Create(node.gameObject.name, node);
            return player;
        }

        private Empty GetNode(int id) { 
            if (noderoot == null) {
                noderoot = RenderInstance.Create<Empty>("empty", this.renderObject);
                noderoot.destroyChildrenOnDestroy = true;
            }
            if (null == nodes) nodes = new Map<int, Empty>();

            Empty node = null;
            if (nodes.TryGetValue(id, out node)) {
                return node;
            }

            var go = Get(id);
            if (null == go) return null;

            node = RenderInstance.Create<Empty>("empty", noderoot);
            node.destroyChildrenOnDestroy = true;
            node.Set(go);
            nodes.Add(id, node);
            return node;
        }
    }

    #if false
    class EditorView : View {
        protected override IEnumerator OnCreateAs(IRenderResource _) { return null; }
        protected override void OnCreate(IRenderResource resource) {
            string name = System.IO.Path.GetFileNameWithoutExtension(resource.name);
            this.gameObject = GameObject.Instantiate(Resources.Load(string.Format("ui/{0}", name))) as GameObject;
        }

        protected override void OnDestroy() {
            if (this.gameObject != null) {
                UnityEngine.Object.DestroyImmediate(this.gameObject);
                this.gameObject = null;
            }
        }
    }

    class GameView : View {
        protected override IEnumerator OnCreateAs(IRenderResource resource) {
            var itr  = base.OnCreateAs(resource);
            if (itr != null) {
                while (itr.MoveNext()) yield return null;
                itr = null;
            }

            var prop = (resource.asset as GameObject).GetComponent<XFX.Asset.Properties.DependsProperty>();
            if (prop != null) {
                var dependencies = prop.dependencies;
                int n = 0, num = dependencies.Length;
                // List<ShaderDep> list = new List<ShaderDep();
                for (int i = 0; i < num; ++i) {
                    var path = dependencies[i].path;
                    if (path.EndsWith(".sd")) {
                        var dep = RenderInstance.Create<ShaderDep>(path, this);
                        dep.Attach(dependencies[i].assets);
                        // list.Add(dep);
                    }
                }
            }
        }

        class ShaderDep : RenderObject {
            Shader shader;
            object[] assets;
            protected override void OnCreate(IRenderResource resource) {
                shader = resource.asset as Shader;
                if (this.assets != null) {
                    this.Attach(this.assets);
                }
            }

            public void Attach(object[] assets) {
                this.assets = assets;
                if (!this.complete) return;
                //
            }
        }
    }

    #endif
}
