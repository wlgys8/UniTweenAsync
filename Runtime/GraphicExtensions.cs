using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS.Async;

namespace MS.TweenAsync.UI{

    public static partial class GraphicColorExtensions{

        private struct State{

            public Color from;
            public Color to;
            public Graphic target;

        }

        static GraphicColorExtensions(){
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.target.color = Color.LerpUnclamped(state.from,state.to,actionState.interpolatedTime);
        }

        public static TweenOperation TintToAsync(this Graphic graphic,TintToOptions options){
            var state = new State(){
                target = graphic,
                from = graphic.color,
                to = options.color,
            };
            return TweenAction<State>.Prepare(state,options.tweenOptions);
        }

        public static TweenOperation AlphaToAsync(this Graphic graphic, AlphaToOptions options){
            var color = graphic.color;
            color.a = options.alpha;
            var colorOptions = new TintToOptions(){
                color = color,
                tweenOptions = options.tweenOptions,
            };
            return graphic.TintToAsync(colorOptions);
        }
    }



    public static class CanvasGroupAlphaExtensions{

        private struct State{

            public CanvasGroup target;
            public float fromAlpha;
            public float toAlpha;
        }

        static CanvasGroupAlphaExtensions(){
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.target.alpha = Mathf.LerpUnclamped(state.fromAlpha,state.toAlpha,actionState.interpolatedTime);
        }

        public static TweenOperation AlphaToAsync(this CanvasGroup canvasGroup,AlphaToOptions options){
            var state = new State(){
                target = canvasGroup,
                fromAlpha = canvasGroup.alpha,
                toAlpha = options.alpha,
            };
            return TweenAction<State>.Prepare(state,options.tweenOptions);
        }

    }

    public struct AlphaToOptions{
        public float alpha;
        public TweenOptions tweenOptions;
        public AlphaToOptions(float alpha,float duration = 1){
            this.alpha = alpha;
            tweenOptions = new TweenOptions(duration);
        }
    }

    public struct TintToOptions{
        public Color color;
        public TweenOptions tweenOptions;

        public TintToOptions(Color color,float duration = 1){
            this.color = color;
            this.tweenOptions = new TweenOptions(duration);
        }

        public TintToOptions(Color color,TweenOptions tweenOptions){
            this.color = color;
            this.tweenOptions = tweenOptions;
        }
    }


}
