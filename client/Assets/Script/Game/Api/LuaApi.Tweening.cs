/********************************************************
    Id: .Tweening.cs
    Desc: Tween接口，单独拆出来
    Author: figo
    Date: 2020-07-29 11:48:41
*********************************************************/

namespace ZF.Game {
    using DG.Tweening;
    using System;
    using UnityEngine;
    using XLua;
    using ZF.Core.Render;

    public partial class LuaApi {

        [LuaCallCSharp]
        public static class Tweening {

            public static void Init(bool recyclable, bool safeMode, int tweenersCapacity, int sequencesCapacity) {
                DOTween.Init(recyclable, safeMode).SetCapacity(tweenersCapacity, sequencesCapacity);
                DOTween.defaultAutoKill = true;
            }

            public static void Clear(bool destroy) {
                DOTween.Clear(destroy);
            }

            public static object Tween(IRenderObject obj, string func, params object[] args) {
                object result = null;
                Tweener tweener = Create(obj.gameObject, func, args);
                result = new System.Action<bool>((complete)=>{
                    if(tweener != null && tweener.IsPlaying()) {
                        tweener.Pause();
                        tweener.Kill(complete);
                    }
                });
                return result;
            }

            [BlackList]
            internal static Tweener Create(GameObject go, string func, params object[] args) {
                Tweener tweener = null;
                if (go != null) {
                    switch (func) {
                        case "DoLocalMove": tweener = Move(go, -1, args); break;
                        case "DoLocalMoveX": tweener = Move(go, 0, args); break;
                        case "DoLocalMoveY": tweener = Move(go, 1, args); break;
                        case "DoLocalMoveZ": tweener = Move(go, 2, args); break;
                        case "DOAnchorMove": tweener = AnchorMove(go, -1, args); break;
                        case "DOAnchorMoveX": tweener = AnchorMove(go, 0, args); break;
                        case "DOAnchorMoveY": tweener = AnchorMove(go, 1, args); break;
                        case "DOAnchorMoveZ": tweener = AnchorMove(go, 2, args); break;
                        case "DoMove": tweener = WorldMove(go, -1, args); break;
                        case "DoMoveX": tweener = WorldMove(go, 0, args); break;
                        case "DoMoveY": tweener = WorldMove(go, 1, args); break;
                        case "DoMoveZ": tweener = WorldMove(go, 2, args); break;
                        case "DoLocalMoveLoop": tweener = MoveLoop(go, -1, args); break;
                        case "DoLocalMoveLoopX": tweener = MoveLoop(go, 0, args); break;
                        case "DoLocalMoveLoopY": tweener = MoveLoop(go, 1, args); break;
                        case "DoLocalMoveLoopZ": tweener = MoveLoop(go, 2, args); break;
                        case "DOScale": tweener = Scale(go, -1, args); break;
                        case "DOScaleX": tweener = Scale(go, 0, args); break;
                        case "DOScaleY": tweener = Scale(go, 1, args); break;
                        case "DOScaleZ": tweener = Scale(go, 2, args); break;
                        case "DOLocalRotate": tweener = Rotate(go, args); break;
                        case "DOLocalRotateLinear": tweener = RotateLocalAndLinear(go, args); break;
                        case "DOLocalRotateLoop": tweener = RotateLoop(go, args); break;
                        case "DOFade": tweener = DOFade(go, args); break;
                        case "DOFadeLoop": tweener = DOFadeLoop(go, args); break;
                        default:
                            Log.Warn("DOTweener.{0} is unsupported", func);
                            break;
                    }
                }
                return tweener;
            }

            // axis: 0 X轴, 1 Y轴, 2 Z轴
            private static Tweener Move(GameObject go, int axis, params object[] args) {
                Vector3 endv = Vector3.zero;
                float endf = 0f;
                float duration = 0;
                bool snapping = false;
                System.Action onComplete = null;
                Ease easeType = Ease.Linear;

                if (args.Length < 2) return null;

                // parse endValue, dunration
                if (args[0].GetType() == typeof(Vector3)) endv = (Vector3)args[0];
                else endf = System.Convert.ToSingle(args[0]);
                duration = System.Convert.ToSingle(args[1]);

                // parse snapping, onComplete
                if (args.Length > 2) {
                    if (args[2].GetType() == typeof(bool)) snapping = (bool)args[2];
                    else {
                        var lfn = (LuaFunction)args[2];
                        onComplete = lfn.Cast<System.Action>();
                        lfn.Dispose();
                    }
                }
                if (args.Length > 3) {
                    var lfn = (LuaFunction)args[3];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                //要使用动画曲线，需要写满五个参数
                if (args.Length > 4){
                    easeType = (Ease)Enum.Parse(typeof(Ease), (string)args[4]);
                }
                Tweener tweener = null;
                if (axis == -1) tweener = go.transform.DOLocalMove(endv, duration, snapping).SetUpdate(true);
                if (axis == 0) tweener = go.transform.DOLocalMoveX(endf, duration, snapping).SetUpdate(true);
                if (axis == 1) tweener = go.transform.DOLocalMoveY(endf, duration, snapping).SetUpdate(true);
                if (axis == 2) tweener = go.transform.DOLocalMoveZ(endf, duration, snapping).SetUpdate(true);
                if (tweener != null) tweener.SetEase(easeType);
                if (tweener != null) tweener.Play();
                if (tweener != null && onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener AnchorMove(GameObject go, int axis, params object[] args) {
                Vector3 endv = Vector3.zero;
                float endf = 0f;
                float duration = 0;
                bool snapping = false;
                System.Action onComplete = null;
                Ease easeType = Ease.Linear;

                if (args.Length < 2) return null;

                // parse endValue, dunration
                if (args[0].GetType() == typeof(Vector3)) endv = (Vector3)args[0];
                else endf = System.Convert.ToSingle(args[0]);
                duration = System.Convert.ToSingle(args[1]);

                // parse snapping, onComplete
                if (args.Length > 2) {
                    if (args[2].GetType() == typeof(bool)) snapping = (bool)args[2];
                    else {
                        var lfn = (LuaFunction)args[2];
                        onComplete = lfn.Cast<System.Action>();
                        lfn.Dispose();
                    }
                }
                if (args.Length > 3) {
                    var lfn = (LuaFunction)args[3];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                //要使用动画曲线，需要写满五个参数
                if (args.Length > 4){
                    easeType = (Ease)Enum.Parse(typeof(Ease), (string)args[4]);
                }
                Tweener tweener = null;
                if (axis == -1) tweener = go.GetComponent<RectTransform>().DOAnchorPos3D(endv, duration, snapping).SetUpdate(true);
                if (axis == 0) tweener = go.GetComponent<RectTransform>().DOAnchorPos3DX(endf, duration, snapping).SetUpdate(true);
                if (axis == 1) tweener = go.GetComponent<RectTransform>().DOAnchorPos3DY(endf, duration, snapping).SetUpdate(true);
                if (axis == 2) tweener = go.GetComponent<RectTransform>().DOAnchorPos3DZ(endf, duration, snapping).SetUpdate(true);
                if (tweener != null) tweener.SetEase(easeType);
                if (tweener != null) tweener.Play();
                if (tweener != null && onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

             // axis: 0 X轴, 1 Y轴, 2 Z轴
            private static Tweener WorldMove(GameObject go, int axis, params object[] args) {
                Vector3 endv = Vector3.zero;
                float endf = 0f;
                float duration = 0;
                bool snapping = false;
                System.Action onComplete = null;

                if (args.Length < 2) return null;

                // parse endValue, dunration
                if (args[0].GetType() == typeof(Vector3)) endv = (Vector3)args[0];
                else endf = System.Convert.ToSingle(args[0]);
                duration = System.Convert.ToSingle(args[1]);

                // parse snapping, onComplete
                if (args.Length > 2) {
                    if (args[2].GetType() == typeof(bool)) snapping = (bool)args[2];
                    else {
                        var lfn = (LuaFunction)args[2];
                        onComplete = lfn.Cast<System.Action>();
                        lfn.Dispose();
                    }
                }
                if (args.Length > 3) {
                    var lfn = (LuaFunction)args[3];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }

                Tweener tweener = null;
                if (axis == -1) tweener = go.transform.DOMove(endv, duration, snapping).SetUpdate(true);
                if (axis == 0) tweener = go.transform.DOMoveX(endf, duration, snapping).SetUpdate(true);
                if (axis == 1) tweener = go.transform.DOMoveY(endf, duration, snapping).SetUpdate(true);
                if (axis == 2) tweener = go.transform.DOMoveZ(endf, duration, snapping).SetUpdate(true);
                if (tweener != null) tweener.Play();
                if (tweener != null && onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }


            // axis: 0 X轴, 1 Y轴, 2 Z轴
            private static Tweener Scale(GameObject go, int axis, params object[] args) {
                Vector3 scalev = Vector3.zero;
                float scalef = 0f;
                float duration = 0;
                System.Action onComplete = null;
                Ease easeType = Ease.Linear;

                if (args.Length < 2) return null;
                if (args[0].GetType() == typeof(Vector3)) scalev = (Vector3)args[0];
                else scalef = System.Convert.ToSingle(args[0]);
                duration = System.Convert.ToSingle(args[1]);
                if (args.Length > 2) {
                    var lfn = (LuaFunction)args[2];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                if (args.Length > 3){
                    easeType = (Ease)Enum.Parse(typeof(Ease), (string)args[3]);
                }

                Tweener tweener = null;
                if (axis == -1) {
                    if (scalev != Vector3.zero) tweener = go.transform.DOScale(scalev, duration);
                    else tweener = go.transform.DOScale(scalef, duration);
                }
                if (axis == 0) tweener = go.transform.DOScaleX(scalef, duration);
                if (axis == 1) tweener = go.transform.DOScaleY(scalef, duration);
                if (axis == 2) tweener = go.transform.DOScaleZ(scalef, duration);
                if (tweener != null){
                    tweener.SetEase(easeType);
                    tweener.Play();
                }
                if (tweener != null && onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener RotateLocalAndLinear(GameObject go, params object[] args) { 
                if (args.Length < 2) return null;

                Vector3 endVal = (Vector3)args[0];
                float duration = System.Convert.ToSingle(args[1]);
                System.Action onComplete = null;
                if (args.Length > 2) {
                    var lfn = (LuaFunction)args[2];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }

                Tweener tweener = go.transform.DOLocalRotate(endVal, duration, RotateMode.LocalAxisAdd);
                tweener.SetEase(Ease.Linear);
                tweener.Play();
                if (onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener Rotate(GameObject go, params object[] args) { 
                if (args.Length < 2) return null;

                Vector3 endVal = (Vector3)args[0];
                float duration = System.Convert.ToSingle(args[1]);
                System.Action onComplete = null;
                if (args.Length > 2) {
                    var lfn = (LuaFunction)args[2];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }

                Tweener tweener = go.transform.DOLocalRotate(endVal, duration);
                tweener.Play();
                if (onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener RotateLoop(GameObject go, params object[] args) { 
                if (args.Length < 2) return null;

                Vector3 endVal = (Vector3)args[0];
                float duration = System.Convert.ToSingle(args[1]);
                int loopCount = System.Convert.ToInt32(args[2]);

                LoopType loopType = (LoopType)Enum.Parse(typeof(LoopType), (string)args[3]);             
                Ease easeType= (Ease)Enum.Parse(typeof(Ease), (string)args[4]);

                System.Action onComplete = null;
                if (args.Length > 5) {
                    var lfn = (LuaFunction)args[5];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }

                Tweener tweener = go.transform.DOLocalRotate(endVal, duration, RotateMode.FastBeyond360);
                tweener.SetLoops(loopCount, loopType).SetEase(easeType);
                tweener.Play();
                if (onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener DOFade(GameObject go, params object[] args)
            {
                if (args.Length < 2) return null;

                float endVal = System.Convert.ToSingle(args[0]);
                float duration = System.Convert.ToSingle(args[1]);

                System.Action onComplete = null;
                if (args.Length > 2)
                {
                    var lfn = (LuaFunction)args[2];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                
                Tweener tweener = go.GetComponent<CanvasGroup>().DOFade(endVal, duration).SetUpdate(true);
                tweener.Play();
                if (onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            private static Tweener DOFadeLoop(GameObject go, params object[] args)
            {
                if (args.Length < 3) return null;

                float endVal = System.Convert.ToSingle(args[0]);
                float duration = System.Convert.ToSingle(args[1]);
                int loopConut = System.Convert.ToInt32(args[2]);

                System.Action onComplete = null;
                if (args.Length > 3)
                {
                    var lfn = (LuaFunction)args[3];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                Tweener tweener = go.GetComponent<CanvasGroup>().DOFade(endVal, duration);
                tweener.SetLoops(loopConut, LoopType.Yoyo);
                tweener.Play();
                if (onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }

            // axis: 0 X轴, 1 Y轴, 2 Z轴
            // 参数规定args 前面两个参数是end 和dunration,第三个是延迟时间，第四个循环次数,第五个循环方式,第六个动画曲线,第七个是snapping,第八个是回调函数。
            private static Tweener MoveLoop(GameObject go, int axis, params object[] args) {

                Vector3 endv = Vector3.zero;
                float endf = 0f;
                float duration = 0;
                bool snapping = false;
                float delayTime = 0;
                int loopCount = 1;                                  //循环次数-1为无限循环
                LoopType loopType = LoopType.Yoyo;                  //循环类型Yoyo，Restart，Incremental
                Ease easeType = Ease.OutQuad;                        //运动曲线默认为OutQuad，SetEase(Ease.OutQuad); 默认的运动曲线。
                System.Action onComplete = null;

                if (args.Length < 2) return null;

                // parse endValue, dunration
                if (args[0].GetType() == typeof(Vector3)) endv = (Vector3)args[0];
                else endf = System.Convert.ToSingle(args[0]);
                duration = System.Convert.ToSingle(args[1]);

                delayTime = System.Convert.ToSingle(args[2]);

                loopCount = System.Convert.ToInt32(args[3]);

                loopType = (LoopType)Enum.Parse(typeof(LoopType), (string)args[4]);
      
                easeType= (Ease)Enum.Parse(typeof(Ease), (string)args[5]);

                if (args.Length >= 7) {
                    if (args[6].GetType() == typeof(bool)) snapping = (bool)args[6];
                    else {
                        var lfn = (LuaFunction)args[6];
                        onComplete = lfn.Cast<System.Action>();
                        lfn.Dispose();
                    }
                }

                if (args.Length >= 8) {
                    var lfn = (LuaFunction)args[7];
                    onComplete = lfn.Cast<System.Action>();
                    lfn.Dispose();
                }
                Tweener tweener = null;
                if (axis == -1) tweener = go.transform.DOLocalMove(endv, duration, snapping);
                if (axis == 0) tweener = go.transform.DOLocalMoveX(endf, duration, snapping);
                if (axis == 1) tweener = go.transform.DOLocalMoveY(endf, duration, snapping);
                if (axis == 2) tweener = go.transform.DOLocalMoveZ(endf, duration, snapping);
                if (tweener != null)
                {
                    tweener.SetLoops(loopCount, loopType).SetEase(easeType).SetDelay(delayTime);
                    tweener.Play();
                }
                if (tweener != null && onComplete != null) tweener.OnComplete(() => { onComplete(); });
                return tweener;
            }
            
        }
    }
}