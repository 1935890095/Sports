
namespace ZF.Asset {
    using UnityEngine;
    using ZF.Core.Render;

    public interface IVideoClip : IRenderObject
    {
        UnityEngine.Video.VideoClip clip { get; }
    }

    public class VideoClip : RenderObject, IVideoClip
    {
        public UnityEngine.Video.VideoClip clip { get { return _clip; } }
        UnityEngine.Video.VideoClip _clip;

        protected override void OnCreate(IRenderResource resource)
        {
            _clip = resource.asset as UnityEngine.Video.VideoClip;
        }
    }
}
