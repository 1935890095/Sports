

namespace XFX.Game {
    using System.Collections;
    using UnityEngine;
    using XLua;

    public partial class LuaApi {

        [LuaCallCSharp]
        public static class Shaking {

            public static void RectTransform(RectTransform tran, float amount, float duration) {
                LuaApi.game.StartCoroutine(RectTransformShaking(tran, amount, duration));
            }

            public static void Vibrate() {
                Handheld.Vibrate();
            }
        }

        private static IEnumerator RectTransformShaking(RectTransform tran, float amount, float duration) {
            var pos = tran.anchoredPosition;
            UnityEngine.Debug.Log(pos);
            while (true) {
                var newPos = pos + Random.insideUnitCircle * amount;
                tran.anchoredPosition = newPos;
                duration -= Time.deltaTime;
                if (duration <= 0) {
                    tran.anchoredPosition = pos;
                    yield break;
                }
                yield return null;
            }
        }
    }
}