/********************************************************
    id: Fps.cs
    desc: fps统计
    author: figo
    date: 2019/01/30 11:57:52

    Copyright (C) 2019 zwwx Ltd. All rights reserved.
*********************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Misc {

    public static class Fps {
        private static GameObject go = null;
        public static int fps { get; private set; }

        static void Setup() {
            if (go == null) {
                go = new GameObject("Fps");
                GameObject.DontDestroyOnLoad(go);
                go.transform.position = new Vector3(0.5f, 0.98f, 0f);
            }
        }

        // 开始在屏幂上显示fps
        public static void StartScreen() {
            Setup();
            if (go.GetComponent<FpsScreen>() != null) {
                go.GetComponent<FpsScreen>().enabled = true;
            } else {
                go.AddComponent<FpsScreen>().Init(0.2f);
            }
        }

        // 停止在屏幂上显示fps
        public static void StopScreen() {
            if (go == null) return;
            if (go.GetComponent<FpsScreen>() != null) {
                go.GetComponent<FpsScreen>().enabled = false;
            }
        }

        // 开始fps统计
        public static void StartRecord() {
            Setup();
            if (go.GetComponent<FpsRecorder>() != null)  {
                go.GetComponent<FpsRecorder>().Clear();
                go.GetComponent<FpsRecorder>().enabled = true;
            } else {
                int custs = 20, lts1 = 0, lts2 = 0, lts3 = 0, dotfreq = 3000;
                go.AddComponent<FpsRecorder>().Init(0.2f, 0-custs, lts1, lts2, lts3, dotfreq);
            }
        }

        // 停止fps统计
        public static void StopRecord() {
            if (go == null)
                return;
            if (go.GetComponent<FpsRecorder>() != null) {
                go.GetComponent<FpsRecorder>().Finish();
            }
        }

        // 获取统计信息
        public static FpsInfo GetFpsInfo() {
            if (go == null) return null;
            if (go.GetComponent<FpsRecorder>() != null) {
                return go.GetComponent<FpsRecorder>().GetLastFpsInfo();
            } else {
                return null;
            }
        }

        internal abstract class FpsComponent : MonoBehaviour {
            private const float min_interval = 0.01f;
            private double last_time = 0;
            private int frames = 0;
            private float interval = 1.0f;

            public void Init(float itv) {
                this.interval = itv > min_interval ? itv : min_interval;
            }

            protected void Start() {
                last_time = Time.realtimeSinceStartup;
                frames = 0;
            }

            protected void Update() {
                ++frames;
                double now = Time.realtimeSinceStartup;
                if (now > last_time + interval) {
                    double ffps = frames / (now - last_time);
                    frames = 0;
                    last_time = now;
                    int fps  = (int)ffps;
                    OnFps(fps);
                }
            }
            protected abstract void OnFps(int fps);
        }

        internal class FpsScreen : FpsComponent {
            private int fps = 0;
            protected override void OnFps(int fps) { this.fps = fps; }

            void OnGUI() {
                // GUI.color = Color.red;
                GUIStyle style = new GUIStyle();
                style.normal.background = null;
                style.normal.textColor = new Color(1, 0, 0);
                style.fontSize = 40;
                GUILayout.Label(string.Format("fps:{0}", this.fps), style);
            }
        }

        internal class FpsRecorder : FpsComponent {
            protected override void OnFps(int fps) { 
                Add(fps);
            }

            private const int hts = -10;    // 严重抖动阈值
            private const int llts = -10;   // 轻微抖动左阈值
            private const int lrts = -4;    // 轻微抖动右阈值
            private int lts1 = -1;          // 轻微抖动阈值1
            private int lts2 = -2;          // 轻微抖动阈值2
            private int lts3 = -3;          // 轻微抖动阈值3
            private int custs = 0;          // 自定义抖动右阈值
            private float dotfreq = 1.0f;   // 采集频率
            private double lastdottime;

            private List<int> fpss = new List<int>();
            private List<int> fpsdots = new List<int>();
            private int last_fps = -1;
            private int htimes = 0;         // 严重抖动次数
            private int ltimes = 0;         // 轻微抖动次数
            private int ltimes1 = 0;        // 轻微抖动次数1
            private int ltimes2 = 0;        // 轻微抖动次数2
            private int ltimes3 = 0;        // 轻微抖动次数3
            private int custimes = 0;

            private FpsInfo last_info = null;

            public void Init(float interval, int custs, int lts1, int lts2, int lts3, int dotfreq) {
                base.Init(interval);
                this.custs = custs;
                this.lts1 = lts1;
                this.lts2 = lts2;
                this.lts3 = lts3;
                this.dotfreq = dotfreq / 1000f;
                this.lastdottime = Time.realtimeSinceStartup;
            }

            public void Add(int fps) {
                if (fps < 0) 
                    return;
                fpss.Add(fps);
                // 如果不是第一次
                if (last_fps > 0) {
                    int dif = fps - last_fps;
                    if (dif < hts) {
                        ++htimes;
                    } else if (dif >= llts && dif < lrts) {
                        ++ltimes;
                    }
                    if (dif <= custs) {
                        ++custimes;
                    }
                }
                last_fps = fps;
                double curtime = Time.realtimeSinceStartup;
                if (dotfreq > 0 && lastdottime + dotfreq <= curtime) {
                    fpsdots.Add(fps);
                    lastdottime = curtime;
                    if (fps >= 0 && fps < lts1) {
                        ++ltimes1;
                    } else if (fps >= lts1 && fps < lts2) {
                        ++ltimes2;
                    } else if (fps >= lts2 && fps < lts3) {
                        ++ltimes3;
                    }
                }
            }

            public int GetFps() { return last_fps; }
            public FpsInfo GetLastFpsInfo() { return last_info; }

            public void Save() {
                if (fpss.Count <= 0) 
                    return;
                int sum = 0;
                int min = fpss[0];
                int max = fpss[0];
                for (int i = 0; i < fpss.Count; ++i) {
                    sum += fpss[i];
                    if (fpss[i] > max)
                        max = fpss[i];
                    else
                        min = fpss[i];
                }
                float avg = (float)sum / fpss.Count;
                avg = (float)Math.Round(avg, 1);
                string fps_dots_str = GetFpsDotsStr();

                FpsInfo info = new FpsInfo(avg, max, min, fpss.Count, htimes, ltimes, custimes, ltimes1, ltimes2, ltimes3, fps_dots_str);
                last_info = info;
            }

            string GetFpsDotsStr() {
                string str = "";
                int count = fpsdots.Count;
                for (int i = 0; i < count; ++i) {
                    str += "" + fpsdots[i] + ",";
                }
                return str;
            }

            public void Clear() {
                if (fpss != null) fpss.Clear();
                if (fpsdots != null) fpsdots.Clear();

                last_fps = -1;
                htimes = 0;
                ltimes = 0;
                ltimes1 = 0;
                ltimes2 = 0;
                ltimes3 = 0;
                custimes = 0;
                last_info = null;

                if (lts1 <= 0) ltimes1 = -1;
                if (lts2 <= 0) ltimes2 = -1;
                if (lts3 <= 0) ltimes3 = -1;
            }

            public void Finish() {
                if (last_info == null) {
                    Save();
                }
            }
        }
    }

    public class FpsInfo {
        public float avg;   // fps平均值，保留一位小数点
        public int max;     // fps最大值
        public int min;     // fps最小值
        public int total_times; // 总共统计次数
        public int heavy_times; // 严重抖动次数
        public int light_times; // 轻微抖动次数
        public int light_times1;// 轻微抖动次数1
        public int light_times2;// 轻微抖动次数2
        public int light_times3;// 轻微抖动次数3
        public int custom_times;// 自定义抖动次数
        public string fps_dots; // fps统计结果数组

        public FpsInfo(float avg, int max, int min, int total_times, 
                int heavy_times, int light_times, int light_times1, int light_times2, int light_times3,
                int custom_times, string fps_dots) {
            this.avg = avg;
            this.max = max;
            this.min = min;
            this.total_times = total_times;
            this.heavy_times = heavy_times;
            this.light_times = light_times;
            this.light_times1 = light_times1;
            this.light_times2 = light_times2;
            this.light_times3 = light_times3;
            this.custom_times = custom_times;
            this.fps_dots = fps_dots;
        }

        public override string ToString() {
            return "均值: " + avg
                + ", 最大值: " + max
                + ", 最小值: " + min
                + ", 统计次数: " + total_times
                + ", 严重抖动次数: " + heavy_times
                + ", 轻微抖动次数: " + light_times
                + ", 轻微抖动次数1: " + light_times1
                + ", 轻微抖动次数2: " + light_times2
                + ", 轻微抖动次数3: " + light_times3
                + ", 自定义抖动次数: " + custom_times + "\n";
        }
    }
}
