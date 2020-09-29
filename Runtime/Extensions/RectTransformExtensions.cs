using MS.Async;
using UnityEngine;

namespace MS.TweenAsync{

    public static partial class RectTransformExtensions
    {
        public static TweenOperation AnchoredPositionToAsync(this RectTransform transform,AnchoredPositionToOptions options){
            return Values.To(options.position).Property(transform, Properties.rectTransform.anchoredPosition,options.tweenOptions);
        }
        public static TweenOperation SizeToAsync(this RectTransform transform,SizeToOptions options){
            return Values.To(options.size).Property(transform,Properties.rectTransform.size,options.tweenOptions);
        }
    }

    public struct AnchoredPositionToOptions{
        public Vector2 position;
        public TweenOptions tweenOptions;
        public AnchoredPositionToOptions(Vector2 position,float duration){
            this.position = position;
            this.tweenOptions = new TweenOptions(duration);
        }
    }

    public struct SizeToOptions{

        public Vector2 size;
        public TweenOptions tweenOptions;
    }
}
