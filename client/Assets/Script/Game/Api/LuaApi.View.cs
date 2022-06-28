/********************************************************
    id: LuaApi.View.cs
    desc: 定义给lua层使用的API——View
    author: figo
    date: 2020-03-12 14:19:48

    Copyright (C) 2018 zwwx Ltd. All rights reserved.
*********************************************************/

namespace ZF.Game
{
    using DG.Tweening;
    // using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.IO;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine;
    using XLua;
    using ZF.Asset;
    using ZF.Core.Util;
    using Component = UnityEngine.Component;

    public partial class LuaApi
    {
        // View Api
        [LuaCallCSharp]
        public static class View
        {
            [BlackList]
            static UnityEngine.Camera viewCamera;

            static Map<string, LuaView> views = new Map<string, LuaView> ();
            private static LuaView GetView (string name)
            {
                LuaView view;
                views.TryGetValue (name, out view);
                return view;
            }
            private static GameObject GetView (string name, int id)
            {
                var view = GetView (name);
                if (view != null) return view.Get (id);
                return null;
            }
            private static T GetView<T> (string name, int id) where T : Component
            {
                var view = GetView (name);
                if (view != null) return view.Get<T> (id);
                return default (T);
            }

            // 打开
            public static void Open (string name)
            {
                LuaView view = GetView (name);
                if (view == null)
                {
                    view = LuaView.Create (context, game.viewroot, name);
                    views.Add (name, view);
                    // Log.Debug(">>>> cs open view {0}", name);
                }
                else
                {
                    view.Show ();
                }
            }

            public static bool IsOpen (string name)
            {
                LuaView view = GetView (name);
                if (view != null) return view.isShow;
                else return false;
            }

            // 关闭
            public static void Close (string name, bool destroy)
            {
                LuaView view = GetView (name);
                if (view != null)
                {
                    if (destroy)
                    {
                        view.Destroy ();
                        views.Remove (name);
                    }
                    else
                    {
                        view.Hide ();
                    }
                }
            }

            public static string Dump ()
            {
                // TODO: dump出所有view信息，以json的形式
                return "";
            }

            // 查找View的子节点并返回子节点的id, 否则返回0
            public static int Lookup (string name, int parentid, string path, string type)
            {
                // Log.Debug("lookup view {0}, path: {1}", name, path);
                int id = 0;
                var view = GetView (name);
                if (view != null) id = view.Lookup (parentid, path, type);
                return id;
            }

            // 根据路径，获取view下路径为path的字节点id，找不到返回0
            public static int Find(string name, string path)
            {
                int id = 0;
                var view = GetView (name);
                if (view != null) id = view.Find(path);
                return id;
            }

            #region [节点相关接口]
            // 设置节点的有效性
            public static object GameObject (string name, int id, string func, object value)
            {
                object result = null;
                switch (func)
                {
                    case "active":
                        var go = GetView (name, id);
                        if (go != null)
                        {
                            result = go.activeInHierarchy;
                            if (value != null) go.SetActive ((bool) value);
                        }
                        break;
                    case "activeSelf":
                        go = GetView (name, id);
                        if (go != null)
                        {
                            result = go.activeSelf;
                        }
                        break;
                    case "tag":
                        go = GetView (name, id);
                        if (go != null)
                        {
                            result = go.tag;
                            if (value != null) go.tag = (string) value;
                        }
                        break;
                    case "clone": // special func: clone a node
                        var view = GetView (name);
                        if (view != null)
                        {
                            result = view.Clone (id);
                            if (value != null)
                            {
                                // set name
                                go = GetView (name, (int) result);
                                if (go != null) go.name = (string) value;
                            }
                        }
                        break;
                    case "delete": // special func: delete a node witch is created by clone
                        view = GetView (name);
                        if (view != null) view.Delete (id);
                        break;
                    case "name":
                        go = GetView (name, id);
                        if (go != null)
                        {
                            result = go.name;
                            if (value != null) go.name = (string) value;
                        }
                        break;
                    default:
                        Log.Warn ("GameObject.{0} is unsupported", func);
                        break;
                }
                return result;
            }

            public static object Transform (string name, int id, string func, object value)
            {
                object result = null;
                var go = GetView (name, id);
                if (go != null)
                {
                    var ts = go.transform;
                    switch (func)
                    {
                        case "SetSiblingIndex":
                            ts.SetSiblingIndex (System.Convert.ToInt32 (value));
                            break;
                        case "localPosition":
                            result = ts.localPosition;
                            if (value != null) ts.localPosition = (Vector3) value;
                            break;
                        case "localEulerAngles":
                            result = ts.localEulerAngles;
                            if (value != null) ts.localEulerAngles = (Vector3) value;
                            break;
                        case "localScale":
                            result = ts.localScale;
                            if (value != null) ts.localScale = (Vector3) value;
                            break;
                        case "position":
                            result = ts.position;
                            if (value != null) ts.position = (Vector3) value;
                            break;
                        case "parent":
                            var parent = GetView(name, System.Convert.ToInt32(value));
                            if (null != parent) ts.SetParent(parent.transform);
                            break;
                        case "screenPosition":
                            if (null == viewCamera) viewCamera = game.viewroot.gameObject.GetComponentInChildren<UnityEngine.Camera>(true);
                            result = viewCamera.WorldToScreenPoint(ts.position);
                            break;
                        case "objfindanddelete":
                            result = ts.transform;
                            for(int i = 0; i < ts.childCount; i++)
                            {
                              UnityEngine.GameObject.Destroy(ts.GetChild(i).gameObject);
                            }
                            break;
                        default:
                            Log.Warn ("Transform.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }

            #endregion

            #region [控件:事件]
            public static void SetEvent (string name, int id, int type, System.Action callback)
            {
                var view = GetView (name);
                if (view != null)
                {
                    var go = view.Get (id);
                    if (go != null)
                    {
                        var trigger = go.GetComponent<EventTrigger> ();
                        if (trigger == null) { trigger = go.AddComponent<EventTrigger> (); }
                        var entry = new EventTrigger.Entry ();
                        entry.eventID = (EventTriggerType) type;
                        entry.callback.AddListener ((e) => { callback (); });
                        trigger.triggers.Add (entry);
                    }
                }
            }

            public static void ClearEvents(string name, int id)
            {
                var view = GetView (name);
                if (view != null)
                {
                    var go = view.Get (id);
                    if (go != null)
                    {
                        var trigger = go.GetComponent<EventTrigger> ();
                        UnityEngine.GameObject.Destroy(trigger);
                    }
                }
            }
            #endregion

            #region [控件:RectTransform]
            public static object RectTransform (string name, int id, string func, params object[] args)
            {
                object value = args != null && args.Length > 0 ? args[0] : null;
                object result = null;
                var c = GetView<RectTransform> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "offsetMax":
                            result = c.offsetMax;
                            if (value != null) c.offsetMax = (Vector2) value;
                            break;
                        case "offsetMin":
                            result = c.offsetMin;
                            if (value != null) c.offsetMin = (Vector2) value;
                            break;
                        case "rectSize":
                            result = c.rect.size;
                            break;
                        case "sizeDelta":
                            result = c.sizeDelta;
                            if (value != null) c.sizeDelta = (Vector2) value;
                            break;
                        case "localPosition":
                            result = c.localPosition;
                            if (value != null) c.localPosition = (Vector2) value;
                            break;
                        case "anchoredPosition":
                            result = c.anchoredPosition;
                            if (value != null) c.anchoredPosition = (Vector2) value;
                            break;
                        case "anchoredPosition3D":
                            result = c.anchoredPosition3D;
                            if (value != null) c.anchoredPosition3D = (Vector3) value;
                            break;
                        case "pivot":
                            result = c.pivot;
                            if (null != value) c.pivot = (Vector2)value;
                            break;
                        case "anchorMax":
                            result = c.anchorMax;
                            if (value != null) c.anchorMax = (Vector2) value;
                            break;
                        case "anchorMin":
                            result = c.anchorMin;
                            if (value != null) c.anchorMin = (Vector2) value;
                            break;
                        case "SetInsetAndSizeFromParentEdge":
                            // edge参考RectTransform.Edge
                            var edge = (RectTransform.Edge) args[0];
                            float inset = System.Convert.ToSingle (args[1]);
                            float size = System.Convert.ToSingle (args[2]);
                            c.SetInsetAndSizeFromParentEdge (edge, inset, size);
                            break;
                        case "SetSizeWithCurrentAnchors":
                            // axis参考RectTransform.Axis
                            var axis = System.Convert.ToInt32 (args[0]);
                            size = System.Convert.ToSingle (args[1]);
                            c.SetSizeWithCurrentAnchors ((RectTransform.Axis) axis, size);
                            break;
                        case "RectangleContainsScreenPoint":
                            if (null != value) {
                                result = RectTransformUtility.RectangleContainsScreenPoint(c, (Vector2)value, GetCamera(c));
                            }
                            break;
                        case "mousePosition":
                            Vector2 pos;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle (c, Input.mousePosition, GetCamera(c), out pos);
                            result = pos;
                            break;
                        case "transformPosition":
                            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint (GetCamera(c), (Vector3) value);
                            Vector2 pos2;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle (c, screenPos, GetCamera(c), out pos2);
                            result = pos2;
                            break;
                        case "shaking":
                            Shaking.RectTransform (c, 10, 0.5f);
                            break;
                        case "localEulerAngles":
                            result = c.localEulerAngles;
                            if (value != null) c.localEulerAngles = (Vector3) value;
                            break;
                        case "localScale":
                            result = c.localScale;
                            if (value != null) c.localScale = (Vector3) value;
                            break;
                        case "transformLocalPosFromOther":
                            int otherId = System.Convert.ToInt32(value);
                            RectTransform other = GetView<RectTransform>(name, otherId);
                            var worldPos = other.parent.TransformPoint(other.localPosition);
                            var localPos = c.InverseTransformPoint(worldPos);
                            result = new Vector2(localPos.x, localPos.y);
                            break;
                        case "isOverlaps":
                            RectTransform r1 = c;
                            RectTransform r2 = GetView<RectTransform>(name, System.Convert.ToInt32(value));
                            Vector3[] corners1 = new Vector3[4];
                            r1.GetWorldCorners(corners1);
                            Vector3[] corners2 = new Vector3[4];
                            r2.GetWorldCorners(corners2);
                            var camera1 = GetCamera(r1);
                            var camera2 = GetCamera(r2);
                            var va0 = RectTransformUtility.WorldToScreenPoint(camera1, corners1[0]);
                            var va1 = RectTransformUtility.WorldToScreenPoint(camera1, corners1[2]);
                            var rect1 = new Rect(va0, va1 - va0);
                            var vb0 = RectTransformUtility.WorldToScreenPoint(camera2, corners2[0]);
                            var vb1 = RectTransformUtility.WorldToScreenPoint(camera2, corners2[2]);
                            var rect2 = new Rect(vb0, vb1 - vb0);
                            result = rect1.Overlaps(rect2);
                            break;
                        case "forceRebuildLayoutForRect":
                            LayoutRebuilder.ForceRebuildLayoutImmediate(c);
                            break;
                        default:
                            Log.Warn ("RectTransform.{0} is not supported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Vertical Layout Group]
            public static object VerticalLayoutGroup(string name, int id, string func, object value)
            {
                object result = null;

                var c = GetView<VerticalLayoutGroup>(name, id);
                if (c != null)
                {
                    switch(func)
                    {
                        case "spacing":
                            result = c.spacing;
                            if (value != null) c.spacing = System.Convert.ToSingle(value);
                            break;
                        default:
                            Log.Warn ("Vertical Layout Group.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Button]
            public static object Button (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<Button> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "onClick":
                            var click = (LuaFunction) value;
                            c.onClick.RemoveAllListeners ();
                            c.onClick.AddListener (() => { click.Call (); });
                            break;
                        case "interactable":
                            result = c.interactable;
                            if (value != null) c.interactable = (bool) value;
                            break;
                        default:
                            Log.Warn ("Button.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:DropDown]
            public static object DropDown (string name, int id, string func, params object[] args)
            {
                object result = null;
                var c = GetView<Dropdown> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "clearOptions":
                            c.ClearOptions ();
                            break;
                        case "addOptions":
                            List<string> options = new List<string> ();
                            if (args != null && args.Length > 0)
                            {
                                for (int i = 0; i < args.Length; i++)
                                    options.Add (System.Convert.ToString (args[i]));
                            }
                            c.AddOptions (options);
                            break;
                        case "onValueChanged":
                            if (args != null && args.Length > 0)
                            {
                                var lfn = (LuaFunction) args[0];
                                var callback = lfn.Cast<System.Action<int, int>> ();
                                lfn.Dispose ();
                                c.onValueChanged.RemoveAllListeners ();
                                c.onValueChanged.AddListener ((index) => { callback (id, index); });
                            }
                            break;
                        case "value":
                            if (args != null && args.Length > 0)
                            {
                                c.value = System.Convert.ToInt32 (args[0]);
                            }
                            break;
                        default:
                            Log.Warn ("DropDown.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Toggle]
            public static object Toggle (string name, int id, string func, object value)
            {
                object result = null;
                var view = GetView (name);
                var c = GetView<Toggle> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "onValueChanged":
                            var lfn = (LuaFunction) value;
                            var callback = lfn.Cast<System.Action<int, bool>> ();
                            lfn.Dispose ();
                            c.onValueChanged.RemoveAllListeners ();
                            c.onValueChanged.AddListener ((on) => { callback (id, on); });
                            break;
                        case "interactable":
                            result = c.interactable;
                            if (value != null) c.interactable = (bool) value;
                            break;
                        case "on":
                            result = c.isOn;
                            if (value != null) c.isOn = (bool) value;
                            break;
                        case "group":
                            int groupid = (int) value;
                            var group = view.Get<ToggleGroup> (groupid);
                            if (group != null) c.group = group;
                            break;
                        default:
                            Log.Warn ("Toggle.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }

            public static object ToggleGroup (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<ToggleGroup> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "allowSwitchOff":
                            result = c.allowSwitchOff;
                            if (value != null) c.allowSwitchOff = (bool) value;
                            break;
                        default:
                            Log.Warn ("ToggleGroup.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }

            #endregion

            #region [控件:Graphic] 
            public static object Graphic (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<Graphic> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "SetNativeSize":
                            c.SetNativeSize ();
                            break;
                        case "color":
                            result = c.color;
                            if (value != null) c.color = (Color) value;
                            break;
                        case "raycastTarget":
                            result = c.raycastTarget;
                            if (value != null) c.raycastTarget = (bool) value;
                            break;
                        case "gray":
                            if (c.material.name != "ZFU/UI/ImageGray")
                                c.material = new Material (Shader.Find ("ZFU/UI/ImageGray"));
                            if (value != null) c.color = (bool) value ? Color.black : Color.white;
                            break;
                        default:
                            Log.Warn ("Graphic.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Text]
            public static object Text (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<UnityEngine.UI.Text> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "text":
                            result = c.text;
                            if (value != null) c.text = (string) value;
                            break;
                        case "fontSize":
                            result = c.font;
                            if (value != null) c.fontSize = System.Convert.ToInt32(value);
                            break;
                        case "color":
                            result = c.color;
                            c.color = (Color)value;
                            break;
                        case "resize":
                            var filter = c.GetComponent<ContentSizeFitter> ();
                            if (filter)
                            {
                                filter.SetLayoutHorizontal ();
                                filter.SetLayoutVertical ();
                            }
                            break;
                        case "alignment":
                            result = c.alignment;
                            if(value != null) c.alignment = (TextAnchor)System.Enum.ToObject(typeof(TextAnchor), System.Convert.ToInt32(value));
                            break;
                        default:
                            Log.Warn ("Text.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            public static object Outline(string name, int id, string func, object value){
                object result = null;
                var c = GetView<Outline>(name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "effectColor":
                            result = c.effectColor;
                            if (value != null) c.effectColor = (Color)value;
                            break;
                        case "effectDistance":
                            result = c.effectDistance;
                            if (value != null) c.effectDistance = (Vector2)value;
                            break;
                        default:
                            break;
                    }
                }

                return result;
            }
            #region [控件：Shadow]
            public static object Shadow(string name, int id, string func, object value){
                object result = null;
                var c = GetView<Shadow>(name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "effectColor":
                            result = c.effectColor;
                            if (value != null) c.effectColor = (Color)value;
                            break;
                        case "effectDistance":
                            result = c.effectDistance;
                            if (value != null) c.effectDistance = (Vector2)value;
                            break;
                        default:
                            break;
                    }
                }

                return result;
            }
            #endregion
            #region [控件:InputField]
            public static object InputField (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<InputField> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "text":
                            result = c.text;
                            if (value != null) c.text = (string) value;
                            break;
                        case "activateInputField":
                            c.ActivateInputField ();
                            break;
                        case "onValueChanged":
                            var luaFunc = (LuaFunction) value;
                            var action = luaFunc.Cast<System.Action<string>> ();
                            luaFunc.Dispose ();
                            c.onValueChanged.RemoveAllListeners ();
                            c.onValueChanged.AddListener (content => action (content));
                            break;
                        case "onEndEdit":
                            var lfn = (LuaFunction) value;
                            var callback = lfn.Cast<System.Action<string>> ();
                            lfn.Dispose ();
                            c.onEndEdit.RemoveAllListeners ();
                            c.onEndEdit.AddListener (content => callback (content));
                            break;
                        default:
                            Log.Warn ("InputField.{0} is unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Slider]
            public static object Slider (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<Slider> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "value":
                            result = c.value;
                            if (value != null) c.value = System.Convert.ToSingle (value);
                            break;
                        default:
                            Log.Warn ("Slider.{0} in unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:ScrollRect]
            public static object ScrollRect (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<ScrollRect> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "onValueChanged":
                            var lfn = (LuaFunction) value;
                            var callback = lfn.Cast<System.Action<float, float>> ();
                            lfn.Dispose ();
                            c.onValueChanged.RemoveAllListeners ();
                            c.onValueChanged.AddListener (pos => { callback (pos.x, pos.y); });
                            break;
                        case "horizontalNormalizedPosition":
                            result = c.horizontalNormalizedPosition;
                            if (value != null) c.horizontalNormalizedPosition = System.Convert.ToSingle (value);
                            break;
                        case "verticalNormalizedPosition":
                            result = c.verticalNormalizedPosition;
                            if (value != null) c.verticalNormalizedPosition = System.Convert.ToSingle (value);
                            break;
                        case "velocity":
                            result = c.velocity;
                            if (value != null)
                            {
                                Vector2 velocity = (Vector2) value;
                                c.velocity = velocity;
                            }
                            break;
                        case "stopMovement":
                            c.StopMovement ();
                            break;
                        default:
                            Log.Warn ("ScrollRect.{0} in unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Scrollbar]
            public static object Scrollbar (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<Scrollbar> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "value":
                            result = c.value;
                            if (value != null) c.value = System.Convert.ToSingle (value);
                            break;
                        default:
                            Log.Warn ("Scrollbar.{0} in unsupported", func);
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:DOTween]
            public static object Tween (string name, int id, string func, params object[] args)
            {
                object result = null;
                var go = GetView (name, id);
                Tweener tweener = Tweening.Create (go, func, args);
                result = new System.Action<bool>((complete) =>
                {
                    if (tweener != null && tweener.IsPlaying ())
                    {
                        tweener.Pause();
                        tweener.Kill(complete);
                    }
                });
                return result;
            }

            #endregion

            #region [控件:Image]
            public static object Image (string name, int id, string func, params object[] vals)
            {
                object result = null;
                var view = GetView (name);
                var c = GetView<Image> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "sprite":
                            if (null != vals && vals.Length > 1)
                            {
                                var atlasName = System.Convert.ToString (vals[0]);
                                var spriteName = System.Convert.ToString (vals[1]);
                                if (!string.IsNullOrEmpty (atlasName) && !string.IsNullOrEmpty (spriteName))
                                {
                                    var atlasAsset = view.CreateAtlas (atlasName);
                                    if (vals.Length > 2)
                                        atlasAsset.SetSprite (c, spriteName, System.Convert.ToBoolean(vals[2]));
                                    else
                                        atlasAsset.SetSprite(c, spriteName);
                                }
                            }
                            break;
                        case "native":
                            c.SetNativeSize ();
                            break;
                        case "color":
                            if (vals != null && vals.Length > 0)
                            {
                                c.color = (Color)vals[0];
                            }
                            break;
                        case "fillAmount":
                            result = c.fillAmount;
                            if (vals != null && vals.Length > 0) c.fillAmount = System.Convert.ToSingle (vals[0]);
                            break;
                        case "texture":
                            UnityEngine.Texture2D texture = (UnityEngine.Texture2D)vals[0];
                            float width = System.Convert.ToSingle(vals[1]);
                            float height = System.Convert.ToSingle(vals[2]);
                            c.overrideSprite = Sprite.Create(texture, new Rect(0, 0, width, height), c.rectTransform.pivot);
                            break;
                        case "resize":
                            var filter = c.GetComponent<ContentSizeFitter> ();
                            if (filter)
                            {
                                filter.SetLayoutHorizontal ();
                                filter.SetLayoutVertical ();
                            }
                            break;
                        case "loadAtlas":
                            view.CreateAtlas(System.Convert.ToString (vals[0]));
                            break;
                        default:
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:RawImage]
            public static object RawImage (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<RawImage> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        // 后续修改该接口
                        case "asset":
                            if (value != null)
                            {
                                var asset = (ZF.Asset.Texture) value;
                                if (asset.complete)
                                {
                                    c.texture = asset.texture;
                                }
                                else
                                {
                                    asset.onComplete = (obj) =>
                                    {
                                        if(!obj.destroyed && c) {
                                            c.texture = ((ZF.Asset.Texture) obj).texture;
                                        }
                                    };
                                }
                            }
                            break;
                        case "texture":
                            if (value != null)
                            {
                                c.texture = (UnityEngine.Texture) value;
                            }
                            break;
                        case "raycastTarget":
                            if (value != null)
                            {
                                c.raycastTarget = (bool)value;
                            }
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [控件:Canvas]
            public static object Canvas (string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<Canvas> (name, id);
                if (c != null)
                {
                    switch (func)
                    {
                        case "encodeToPNG":
                            string fileName = value as string;
                            if (string.IsNullOrEmpty (fileName))
                            {
                                Log.Error ("canvas encode to png error, path is null");
                            }
                            UnityEngine.RenderTexture rt = null;
                            game.StartCoroutine (EncodeToPNG (fileName, c, rt));
                            break;
                        case "init":
                            if (null == viewCamera) viewCamera = game.viewroot.gameObject.GetComponentInChildren<UnityEngine.Camera> (true);
                            c.renderMode = RenderMode.ScreenSpaceCamera;
                            c.worldCamera = viewCamera;
                            if (null != value)
                                c.planeDistance = System.Convert.ToSingle (value);
                            break;
                        case "planeDistance":
                            result = c.planeDistance;
                            if (null != value) c.planeDistance = System.Convert.ToSingle (value);
                            break;
                        case "order":
                            result = c.sortingOrder;
                            if (null != value) {
                                c.overrideSorting = true;
                                c.sortingOrder = System.Convert.ToInt32 (value);
                            }
                            break;
                        case "sortingLayerName":
                            result = c.sortingLayerName;
                            if (null != value) {
                                var layerId = SortingLayer.NameToID(System.Convert.ToString(value));
                                c.sortingLayerID = layerId;
                                var childCanvas = c.GetComponentsInChildren<Canvas>();
                                foreach (var canvas in childCanvas) {
                                    canvas.sortingLayerID = layerId;
                                }
                            }
                            break;
                        case "forceUpdateCanvases":
                            UnityEngine.Canvas.ForceUpdateCanvases();
                            break;
                    }
                }
                return result;
            }

            private static IEnumerator EncodeToPNG (string fileName, Canvas canvas, UnityEngine.RenderTexture rt)
            {
                yield return new WaitForSeconds(0.1f);
                string path = PathExt.MakeCachePath (fileName);
                string dir = Path.GetDirectoryName (path);
                if (!Directory.Exists (dir))
                {
                    Directory.CreateDirectory (dir);
                }

                RectTransform transform = canvas.GetComponent<RectTransform> ();
                Rect rect = new Rect (transform.anchoredPosition, transform.sizeDelta);

                yield return new WaitForSeconds(0.1f);
                var camera = canvas.worldCamera;
                if(camera != null) {
                    rt = new  UnityEngine.RenderTexture(1200, 628, 24, RenderTextureFormat.ARGB32);
                    camera.targetTexture = rt;
                    camera.Render();
                }

                yield return new WaitForSeconds(0.1f);
                UnityEngine.RenderTexture.active = rt;
                var tex = new Texture2D (1200, 628, TextureFormat.ARGB4444, true);
                tex.ReadPixels (new Rect(0, 0, 1200, 628), 0, 0);
                tex.Apply ();
                
                yield return new WaitForSeconds(0.1f);
                if (camera != null) {
                    camera.targetTexture = null;
                    Object.Destroy(rt);
                }

                var texture = tex.EncodeToPNG();
                yield return new WaitForSeconds(0.1f);
                System.Threading.Tasks.Task.Factory.StartNew(() => {
                    File.WriteAllBytes(path, texture);
                });
            }
            #endregion

            #region [控件:CanvasGroup]
            public static object CanvasGroup(string name, int id, string func, object value)
            {
                object result = null;
                var c = GetView<CanvasGroup> (name, id);
                if (c != null) {
                    switch (func) {
                        case "alpha":
                            result = c.alpha;
                            if (null != value) c.alpha = System.Convert.ToSingle (value);
                            break;
                        case "interactable":
                            c.interactable = (bool) value;
                            break;
                    }
                }
                return result;
            }
            #endregion

            #region [射线检测]
            public static bool RaycastUI (Vector3 pos)
            {
                PointerEventData eventData = new PointerEventData (EventSystem.current);
                eventData.position = pos;
                List<RaycastResult> raycastResults = new List<RaycastResult> ();
                EventSystem.current.RaycastAll (eventData, raycastResults);
                return raycastResults.Count > 0;
            }

            public static string RaycastUIGetFrist (Vector3 pos)
            {
                PointerEventData eventData = new PointerEventData (EventSystem.current);
                eventData.position = pos;
                List<RaycastResult> raycastResults = new List<RaycastResult> ();
                EventSystem.current.RaycastAll (eventData, raycastResults);
                if (raycastResults.Count > 0)
                    return string.Format ("{0}-{1}", raycastResults[0].gameObject.name, raycastResults[0].module.name);
                else
                    return "";
            }
            #endregion

            #region [资源创建]
            public static IEffect CreateEffect (string name, int id, string filename)
            {
                var view = GetView (name);
                if (view != null) return view.CreateAsset<Effect> (id, filename);
                return null;
            }

            public static IFaceEffect CreateFaceEffect (string name, int id, string filename)
            {
                var view = GetView (name);
                if (view != null) return view.CreateAsset<FaceEffect> (id, filename);
                return null;
            }
            
            public static ICanvas CreateCanvas (string name, int id)
            {
                var view = GetView (name);
                if (view != null) return view.CreateCanvas(id);
                return null;
            }
            public static IRole CreateRole (string name, int id, string filename)
            {
                var view = GetView (name);
                if (view != null) return view.CreateAsset<Role> (id, filename);
                return null;
            }

            public static IReminder CreateReminder (string name, int id, string filename)
            {
                var view = GetView (name);
                if (view != null) return view.CreateAsset<Reminder> (id, filename);
                return null;
            }

            public static IAudio CreateAudio (string name, int id, string filename)
            {
                var view = GetView (name);
                if (view != null) return view.CreateAsset<Audio> (id, filename);
                return null;
            }

            public static IVideoPlayer CreateVideoPlayer(string name, int id) { 
                var view = GetView(name);
                if (view != null) return view.CreateVideoPlayer(id);
                return null;
            }

            #endregion

            #region Misc

            private static UnityEngine.Camera GetCamera(RectTransform tran) {
                Canvas canvas = tran.GetComponentInParent<Canvas>();
                if(!canvas) {
                    return null;
                }
                if(canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                    return null;
                }
                return canvas.worldCamera;
            }

            #endregion
        }
    }
}