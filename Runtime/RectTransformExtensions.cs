using MS.Async;
using UnityEngine;

namespace MS.TweenAsync{

    public static partial class RectTransformAnchorExtensions
    {
        
        private struct State{
            public Vector2 fromPosition;
            public Vector2 toPosition;
            public RectTransform target;
        }

        static RectTransformAnchorExtensions(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }
        private static void OnStart(ref State state){
            state.fromPosition = state.target.anchoredPosition;
        }
        private static void OnUpdate(ActionState actionState,ref State state){
            state.target.anchoredPosition = Vector2.LerpUnclamped(state.fromPosition,state.toPosition,actionState.interpolatedTime);
        }

        public static TweenOperation AnchoredPositionToAsync(this RectTransform transform,AnchoredPositionToOptions options){
            var tweenOptions = options.tweenOptions;
            var state = new State(){
                toPosition = options.position,
                target = transform
            };
            return TweenAction<State>.Prepare(state,options.tweenOptions);
        }

    }


    public static class RectTransformSizeExtensions{
        private struct State{
            public Vector2 from;
            public Vector2 to;
            public RectTransform target;
        }

        static RectTransformSizeExtensions(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){
            state.from = state.target.rect.size;
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            var size = Vector2.LerpUnclamped(state.from,state.to, actionState.interpolatedTime);
            state.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,size.x);
            state.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,size.y);
        }

        public static TweenOperation SizeToAsync(this RectTransform transform,SizeToOptions options){
            var state = new State(){
                target = transform,
                to = options.size,
            };
            return TweenAction<State>.Prepare(state,options.tweenOptions);
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
