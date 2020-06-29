using MS.Async;
using UnityEngine;

namespace MS.TweenAsync{

    public static partial class RectTransformExtensions
    {
        
        private struct AnchorToLerpContext{
            public Vector2 fromPosition;
            public Vector2 toPosition;
            public RectTransform transform;
        }

        private static OnLerp<AnchorToLerpContext> _anchorToLerpFunc = (lerp,context)=>{
            context.transform.anchoredPosition = Vector2.Lerp(context.fromPosition,context.toPosition,lerp);
        };

        public static async LitTask AnchorToAsync(this RectTransform transform,AnchorToOptions options,TweenOperation operation = null){
            var tweenOptions = options.tweenOptions;
            var context = new AnchorToLerpContext(){
                fromPosition = transform.anchoredPosition,
                toPosition = options.position,
                transform = transform
            };
            await CoreUtils.RunLerpAsync<AnchorToLerpContext>(tweenOptions,_anchorToLerpFunc,context,operation);
        }
    }

    public struct AnchorToOptions{

        public Vector2 position;
        public TweenOptions tweenOptions;

        public AnchorToOptions(Vector2 position,float duration){
            this.position = position;
            this.tweenOptions = new TweenOptions(duration);
        }
    }
}
