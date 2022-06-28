using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
    public interface IVideoPlayer : IRenderObject {
        UnityEngine.Video.VideoPlayer player { get; }
        IRenderTexture renderTexture { get; }

        bool waitForFirstFrame { get; set; }
        bool skipOnDrop { get; set; }
        bool loop { get; set; }
        float frameRate { get; }

        bool isPlaying { get; }

        void AddClip(string filename);
        void Play();
        void Pause();
        void Stop(bool removeClip = true);
    }

    public class VideoPlayer : RenderObject, IVideoPlayer
    {
        public static IVideoPlayer Create(string name, IRenderObject parent)
        {
            IVideoPlayer player = RenderInstance.Create<VideoPlayer>("empty", parent);
            player.name = name;
            return player;
        }

        public UnityEngine.Video.VideoPlayer player { get; private set; }
        public IRenderTexture renderTexture { get; private set; }

        public bool isPlaying{ get; private set; }
        public bool waitForFirstFrame {
            get { return _waitForFirstFrame; }
            set {
                _waitForFirstFrame = value;
                if (null != player) player.waitForFirstFrame = value;
            }
        }
        public bool skipOnDrop {
            get { return _skipOnDrop; }
            set {
                _skipOnDrop = value;
                if (null != player) player.skipOnDrop = value;
            }
        }
        public float frameRate {
            get {
                if (null != player)
                    return player.frameRate;
                else
                    return Application.targetFrameRate;
            }
        }
        public bool loop { 
            get { return _loop; }
            set {
                _loop = value;
                if (null != player) player.isLooping = value;
            }
        }

        private bool _waitForFirstFrame = true;
        private bool _skipOnDrop = true;
        private bool _loop = false;

        private UnityEngine.UI.RawImage rawImage;
        private IVideoClip clip;

        private int appTargetFrameRate = 0;

        protected override void OnCreate(IRenderResource resource) {
            rawImage = this.parent.gameObject.GetComponent<UnityEngine.UI.RawImage>();
            if (null == rawImage) {
                ZF.Game.Log.Error("挂载点没有RawImage组件");
                return;
            }

            player = this.parent.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
            player.waitForFirstFrame = _waitForFirstFrame;
            player.skipOnDrop = _skipOnDrop;
            player.isLooping = _loop;
            player.playOnAwake = false;
            player.aspectRatio = UnityEngine.Video.VideoAspectRatio.Stretch;

            renderTexture = RenderTexture.Create("video", this);
            if (renderTexture.complete) {
                rawImage.texture = renderTexture.renderTexture;
                player.targetTexture = renderTexture.renderTexture;
            }
            else { 
                renderTexture.onComplete = (obj) => {
                    var rt = obj as RenderTexture;
                    rawImage.texture = rt.renderTexture;
                    player.targetTexture = rt.renderTexture;
                };
            }
        }

        protected override void OnDestroy() {
            GameObject.Destroy(player);
        }

        protected override void OnUpdate() {
            base.OnUpdate();
            if (isPlaying) {
                if (player.isPrepared && !player.isPlaying) Stop(false);
            }
        }

        public void AddClip(string filename) { 
            if (null != clip) clip.Destroy();
            clip = RenderInstance.Create<VideoClip>(filename, this);
            if (clip.complete) {
                Prepare();
            } else { 
                clip.onComplete = obj => {
                    Prepare();
                };
            }
        }

        public void Play() {
            if (isPlaying) return;

            if (null != clip) {
                isPlaying = true;
                if (player.isPrepared) {
                    appTargetFrameRate = Application.targetFrameRate;
                    Application.targetFrameRate = Mathf.FloorToInt(player.frameRate);
                    player.Play();
                }
            } else {
                ZF.Game.Log.Error("未添加视频，播放不成功");
            }
        }

        public void Pause() {
            if (isPlaying) {
                player.Pause();
                isPlaying = false;
            }
        }

        public void Stop(bool removeClip = true) {
            if (isPlaying) {
                Application.targetFrameRate = appTargetFrameRate;
                player.Stop();
                isPlaying = false;
                if (removeClip) {
                    player.clip = null;
                    clip.Destroy();
                    clip = null;
                }
            }
        }

        private void Prepare() { 
            player.clip = clip.clip;
            player.prepareCompleted += _ => {
                if (isPlaying) {
                    appTargetFrameRate = Application.targetFrameRate;
                    player.Play();
                }
            };
            player.Prepare();
        }
    }
}