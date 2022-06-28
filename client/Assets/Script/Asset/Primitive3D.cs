namespace ZF.Asset {
    using ZF.Core.Render;
    using UnityEngine;
    using System.Collections.Generic;

    public interface IPrimitive3D : IRenderObject
    {
        void SetShaderField(string field, string type, object val);
    }

    public class Primitive3D : Depends, IPrimitive3D
    {
        private Material material;
        private Vector3 _position;
        private Dictionary<string, KeyValuePair<string, object>> fields;

        public Vector3 worldPosition {
            set {
                _position = value;
                if (null != this.gameObject)
                    this.gameObject.transform.position = value;
            }
            get { 
                if (null != this.gameObject)
                    return this.gameObject.transform.position;
                return Vector3.zero;
            }
        }

        public void SetShaderField(string field, string type, object val) {
            if (null != material) {
                SetField(field, type, val);
            } else {
                if (null == fields)
                    fields = new Dictionary<string, KeyValuePair<string, object>>();
                if (!fields.ContainsKey(field))
                    fields.Add(field, new KeyValuePair<string, object>(type, val));
                else
                    fields[field] = new KeyValuePair<string, object>(type, val);
            }
        }

        private void SetField(string field, string type, object val) {
            if (null == material) return;
            if (null == field || null == val) return;

            switch (type) { 
                case "color":
                    material.SetColor(field, (Color)val);
                    break;
                case "float":
                    material.SetFloat(field, System.Convert.ToSingle(val));
                    break;
            }
        }

        private void SetFields() {
            if (null != fields) {
                var enumrator = fields.GetEnumerator();
                while (enumrator.MoveNext()) {
                    SetField(enumrator.Current.Key, enumrator.Current.Value.Key, enumrator.Current.Value.Value);
                }
                enumrator.Dispose();
            }
            fields = null;
        }

        protected override void OnCreate(IRenderResource resource)
        {
            gameObject = GameObject.Instantiate(resource.asset) as GameObject;
            var render = gameObject.GetComponentInChildren<MeshRenderer>(true);
            if (null != render) material = render.material;

            SetFields();
        }

        protected override void OnDestroy() {
            if (this.gameObject != null) {
                RenderObject.DestroyObject(this.gameObject);
                this.gameObject = null;
            }
        }
    }
}