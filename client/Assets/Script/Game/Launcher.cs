using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFX.Game{
    public class Launcher : MonoBehaviour
    {
            Game game = null;

            private IEnumerator Start() {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                // Application.runInBackground = true;
                // LuaApi.Screen.NotchHeight = Screen.safeArea.y;

                // 1.游戏启动
                game = new Game();
                yield return game.Start();
            }
    }
}
