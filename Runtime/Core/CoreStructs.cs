
namespace MS.TweenAsync{


    public struct TweenOptions{
        public float duration;
        public EaseFunction ease;
        public bool ignoreTimeScale;

        public TweenOptions(float duration){
            this.duration = duration;
            ease = EaseFuncs.Linear;
            ignoreTimeScale = false;
        }
    }
}
