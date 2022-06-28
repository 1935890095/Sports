/********************************************************
    Id: Camera.cs
    Desc: 3D射像机
    Author: figo
    Date: 2020-07-29 10:33:23
*********************************************************/

namespace ZF.Asset {
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;
    using ZF.Core.Render;

    public interface ICamera : IRenderObject {
        bool enabled { get; set; }
        Color backgroundColor { get; set; }
        int clearFlags { get; set; }
        int cullingMask { get; set; }
        bool orthographic { get; set; }
        float orthographicSize { get; set; }
        float nearClipPlane { get; set; }
        float farClipPlane { get; set; }
        float fieldOfView { get; set; }
        float depth { get; set; }
        Rect rect { get; set; }

        Vector3 WorldToViewportPoint(Vector3 pos);
        Vector3 WorldToScreenPoint(Vector3 pos);
        Vector3 ViewportToWorldPoint(Vector3 pos);
        Vector3 ScreenToWorldPoint(Vector3 pos);
        Vector3 ViewportToScreenPoint(Vector3 pos);
        Vector3 ScreenToViewportPoint(Vector3 pos);

        // 发起射线检测，返回命中的对角名称
        // 有的资源对象的collider组件是挂在它的子节点上的，为了能命中正确的资源，
        // 提供@up参数用于指定向上追朔父节点。
        object Raycast(Vector3 pos, string tag, int up, string resultType, int layerMask);

        void FadeOut(System.Action onComplete);
        void FadeIn(System.Action onComplete);
        void Shake(float amount, float duration);
    }

    public class Camera : RenderObject, ICamera {
        private UnityEngine.Camera camera;

        public static ICamera Create(string name, IRenderObject parent) {
            ICamera camera = RenderInstance.Create<Camera>("empty", parent);
            camera.name = name;
            return camera;
        }

        protected override void OnCreate(IRenderResource resource) {
            this.gameObject = UnityEngine.GameObject.FindWithTag(ZF.Misc.Defines.Tag.SceneCamera);
            if (null == this.gameObject) this.gameObject = new GameObject("camera");
            this.camera = this.gameObject.GetComponent<UnityEngine.Camera>();
            if (null == this.camera) this.camera = this.gameObject.AddComponent<UnityEngine.Camera>();
        }

        protected override void OnDestroy() {
            this.gameObject = null;
            this.camera = null;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (shaking)
                Shaking();
        }

        public bool enabled {
            get { return this.camera.enabled; }
            set { this.camera.enabled = value; }
        }

        public Color backgroundColor  {
            get { return this.camera.backgroundColor; }
            set { this.camera.backgroundColor = value; }
        }
        public int clearFlags { 
            get { return (int)this.camera.clearFlags; }
            set { this.camera.clearFlags = (CameraClearFlags)value; }
        }

        public int cullingMask {
            get { return this.camera.cullingMask; }
            set { this.camera.cullingMask = value; }
        }

        public bool orthographic { 
            get { return this.camera.orthographic; }
            set { this.camera.orthographic = value; }
        }

        public float orthographicSize {
            get { return this.camera.orthographicSize; }
            set { this.camera.orthographicSize = value; }
        }

        public float  nearClipPlane { 
            get { return this.camera.nearClipPlane; }
            set { this.camera.nearClipPlane = value; }
        }

        public float farClipPlane {
            get { return this.camera.farClipPlane; }
            set { this.camera.farClipPlane = value; }
        }

        public float fieldOfView { 
            get { return this.camera.fieldOfView; }
            set { this.camera.fieldOfView = value; }
        }

        public float depth {
            get { return this.camera.depth; }
            set { this.camera.depth = value; }
        }

        public Rect rect {
            get { return this.camera.rect; }
            set { this.camera.rect = value; }
        }

        public Vector3 WorldToViewportPoint(Vector3 pos) { return this.camera.WorldToViewportPoint(pos); }
        public Vector3 WorldToScreenPoint(Vector3 pos) { return this.camera.WorldToScreenPoint(pos); }
        public Vector3 ViewportToWorldPoint(Vector3 pos) { return this.camera.ViewportToWorldPoint(pos); }
        public Vector3 ScreenToWorldPoint(Vector3 pos) { return this.camera.ScreenToWorldPoint(pos); }
        public Vector3 ViewportToScreenPoint(Vector3 pos) { return this.camera.ViewportToScreenPoint(pos); }
        public Vector3 ScreenToViewportPoint(Vector3 pos) { return this.camera.ScreenToViewportPoint(pos); }

        public object Raycast(Vector3 pos, string tag, int up, string resultType, int layerMask) {
            object result = null;

            var ray = this.camera.ScreenPointToRay(pos);
            RaycastHit hit;
            bool success = false;
            if (layerMask > 0) 
                success = Physics.Raycast(ray, out hit, 1000f, layerMask);
            else 
                success = Physics.Raycast(ray, out hit, 1000f);

            if (success) {
                var temp = hit.collider.gameObject;
                GameObject go = null;
                if (!string.IsNullOrEmpty(tag) && !temp.CompareTag(tag)) {
                    while (up > 0) {
                        temp = temp.transform.parent.gameObject;
                        if (temp == null)
                            break;
                        if (temp.CompareTag(tag)) {
                            go = temp;
                            break;
                        }
                        --up;
                    }
                } else {
                    go = temp;
                }

                if (go) {
                    switch (resultType) { 
                        case "name":
                            result = go.name;
                            break;
                        case "point":
                            result = hit.point;
                            break;
                    }
                }
            }
            return result;
        }

        // public string[] RaycastAll(Vector3 pos, string tag, int layerMask) {
        //     var ray = this.camera.ScreenPointToRay(pos);
        //     RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, layerMask);
        //     var hitslist = new List<RaycastHit>(hits);
        //     hitslist.Sort(delegate(RaycastHit hit1, RaycastHit hit2) { return hit1.distance.CompareTo(hit2.distance); });
        //     List<string> result =new List<string>();
        //     for (int i =  0; i < hitslist.Count; ++i) {
        //        if (!string.IsNullOrEmpty(tag) && hitslist[i].collider.CompareTag(tag)) {
        //            result.Add(hitslist[i].collider.gameObject.name);
        //        }else {
        //            result.Add(hitslist[i].collider.gameObject.name);
        //        }
        //     }
        //     return result.ToArray();
        // }

        public void FadeOut(System.Action onComplete) {}
        public void FadeIn(System.Action onComplete) {}

        #region [shake]
        bool shaking = false;
        ShakeData shakeData;

        public void Shake(float amount, float duration) {
            if (shaking) return;
            if (duration <= 0) return;

            shaking = true;
            shakeData = new ShakeData();
            shakeData.originPos = camera.transform.localPosition;
            shakeData.amount = amount;
            shakeData.duration = duration;
        }

        private void Shaking() {
            camera.transform.localPosition = shakeData.originPos + UnityEngine.Random.insideUnitSphere * shakeData.amount;

            shakeData.duration -= Time.deltaTime;
            if (shakeData.duration <= 0) {
                camera.transform.localPosition = shakeData.originPos;
                shakeData = null;
                shaking = false;
            }
        }

        class ShakeData {
            public Vector3 originPos;
            public float amount;
            public float duration;
        }
        #endregion
    }
}