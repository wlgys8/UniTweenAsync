using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS.Async;

namespace MS.TweenAsync.UI{
    public static class GraphicExtensions 
    {

//==== ColorAnimation =====//

        private struct ColorToContext{

            public Graphic graphic;
            public Color fromColor;

            public Color toColor;
        }


        private static OnLerp<ColorToContext> _colorLerpFunc = (lerp,context)=>{
            context.graphic.color = Color.LerpUnclamped(context.fromColor,context.toColor,lerp);
        };

        public static LitTask ColorToAsync(this Graphic graphic,ColorToOptions options,TweenOperationToken operation = default){
            var context = new ColorToContext(){
                graphic = graphic,
                fromColor = graphic.color,
                toColor = options.color
            };
            return TweenUtility.RunLerpAsync<ColorToContext>(options.tweenOptions,_colorLerpFunc,context,operation);
        }

//====== Alpha Animation ======= //


        public static LitTask AlphaTo(this Graphic graphic,AlphaToOptions options, TweenOperationToken operationToken = default){
            var fromColor = graphic.color;
            var toColor = fromColor;
            toColor.a = options.alpha;
            return graphic.ColorToAsync(new ColorToOptions(toColor,options.tweenOptions),operationToken);
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

    public struct ColorToOptions{
        public Color color;
        public TweenOptions tweenOptions;

        public ColorToOptions(Color color,float duration = 1){
            this.color = color;
            this.tweenOptions = new TweenOptions(duration);
        }

        public ColorToOptions(Color color,TweenOptions tweenOptions){
            this.color = color;
            this.tweenOptions = tweenOptions;
        }
    }


}
