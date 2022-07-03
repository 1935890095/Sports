// using UnityEngine;
// using XFX.Core.Render;

// namespace XFX.Asset
// {
//     public interface ITextMeshPro : IRenderObject
//     {
//         string text { get; set; }
//         float fontSize { get; set; }
//         Color color { get; set; }
//     }

//     public class TextMeshPro : Depends, ITextMeshPro
//     {
//         private TMPro.TextMeshPro component;

//         private string _text = null;
//         private float _fontSize = -1;
//         private Color? _color = null;

//         public string text {
//             get { 
//                 if (null != component)
//                     return component.text;
//                 return string.Empty;
//             }
//             set { 
//                 if (null != component)
//                     component.text = value;
//                 _text = value;
//             }
//         }

//         public float fontSize {
//             get { 
//                 if (null != component)
//                     return component.fontSize;
//                 return 0;
//             }
//             set { 
//                 if (null != component)
//                     component.fontSize = value;
//                 _fontSize = value;
//             }
//         }

//         public Color color {
//             get { 
//                 if (null != component)
//                     return component.color;
//                 return Color.white;
//             }
//             set { 
//                 if (null != component)
//                     component.color = value;
//                 _color = value;
//             }
//         }

//         protected override void OnCreate(IRenderResource resource)
//         {
//             gameObject = GameObject.Instantiate(resource.asset) as GameObject;
//             component = gameObject.GetComponent<TMPro.TextMeshPro>();

//             if (null != _text) component.text = _text;
//             if (_fontSize > 0) component.fontSize = _fontSize;
//             if (null != _color) component.color = _color.Value;
//         }
        
//         protected override void OnDestroy() {
//             if (this.gameObject != null) {
//                 RenderObject.DestroyObject(this.gameObject);
//                 this.gameObject = null;
//             }
//         }
//     }
// }