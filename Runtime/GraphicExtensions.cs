using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS.Async;

namespace MS.TweenAsync.UI{
    public static class GraphicExtensions 
    {
        private struct ColorToContext{

            public Graphic graphic;
            public Color fromColor;

            public Color toColor;
        }


        private static OnLerp<ColorToContext> _colorLerpFunc = (lerp,context)=>{
            context.graphic.color = Color.LerpUnclamped(context.fromColor,context.toColor,lerp);
        };

        public static LitTask ColorToAsync(this Graphic graphic,ColorToOptions options,ref TweenOperationToken operation){
            var context = new ColorToContext(){
                graphic = graphic,
                fromColor = graphic.color,
                toColor = options.color
            };
            return CoreUtils.RunLerpAsync<ColorToContext>(options.tweenOptions,_colorLerpFunc,context,operation);
        }
    }

    public struct ColorToOptions{
        public Color color;
        public TweenOptions tweenOptions;

        public ColorToOptions(Color color,float duration = 1){
            this.color = color;
            this.tweenOptions = new TweenOptions(duration);
        }

    }


}
