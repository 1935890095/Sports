using System.Collections;
using UnityEngine;
using ZF.Core.Render;

namespace ZF.Game {
    public class Launcher : MonoBehaviour {
        Game game = null;
        LaunchView view = null;

        private IEnumerator Start() {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            // Application.runInBackground = true;
            LuaApi.Screen.NotchHeight = Screen.safeArea.y;

            // 1.游戏启动
            game = new Game();

            // 2.启动launch界面
            view = LaunchView.Create(game.viewroot);
            while (!view.enabled) yield return null;
            
            yield return game.Start();
            yield return new WaitForSeconds(0.3f);
            view.Destroy();
        }
    }
}