using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS.Async;

namespace MS.TweenAsync.UI{

    public static partial class GraphicColorExtensions{

        private struct TintToState{
            public Color from;
            public Color to;
            public Graphic target;
        }

        private struct AlphaToState{
            public float from;
            public float to;
            public Graphic target;
        }


        static GraphicColorExtensions(){
            TweenAction<TintToState>.RegisterStart(OnTintStart);
            TweenAction<TintToState>.RegisterUpdate(OnTintUpdate);
            TweenAction<AlphaToState>.RegisterStart(OnAlphaToStart);
        }

        private static void OnTintStart(ref TintToState state){
            state.from = state.target.color;
        }

        private static void OnTintUpdate(ActionState actionState,ref TintToState state){
            state.target.color = Color.LerpUnclamped(state.from,state.to,actionState.interpolatedTime);
        }

        public static TweenOperation TintToAsync(this Graphic graphic,TintToOptions options){
            var state = new TintToState(){
                target = graphic,
                to = options.color,
            };
            return TweenAction<TintToState>.Prepare(state,options.tweenOptions);
        }


        private static void OnAlphaToStart(ref AlphaToState state){
            state.from = state.target.color.a;
        }
        private static void OnAlphaToUpdate(ActionState actionState,ref AlphaToState state){
            var alpha = Mathf.LerpUnclamped(state.from,state.to,actionState.interpolatedTime);
            var color = state.target.color;
            color.a = alpha;
            state.target.color = color;
        }

        public static TweenOperation AlphaToAsync(this Graphic graphic, AlphaToOptions options){
            var state = new AlphaToState(){
                target = graphic,
                to = options.alpha,
            };
            return TweenAction<AlphaToState>.Prepare(state,options.tweenOptions);
        }
    }



    public static class CanvasGroupAlphaExtensions{

        private struct State{

            public CanvasGroup target;
            public float fromAlpha;
            public float toAlpha;
        }

        static CanvasGroupAlphaExtensions(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){
            state.fromAlpha = state.target.alpha;
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.target.alpha = Mathf.LerpUnclamped(state.fromAlpha,state.toAlpha,actionState.interpolatedTime);
        }

        public static TweenOperation AlphaToAsync(this CanvasGroup canvasGroup,AlphaToOptions options){
            var state = new State(){
                target = canvasGroup,
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
