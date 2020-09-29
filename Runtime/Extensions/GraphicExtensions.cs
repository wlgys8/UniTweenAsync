using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS.Async;

namespace MS.TweenAsync.UI{

    public static partial class GraphicColorExtensions{
        public static TweenOperation TintToAsync(this Graphic graphic,TintToOptions options){
            return new To<Color>(options.color)
            .Property(graphic,Properties.graphic.color,options.tweenOptions);
        }

        public static TweenOperation AlphaToAsync(this Graphic graphic, AlphaToOptions options){
            return new To<float>(options.alpha)
            .Property(graphic,Properties.graphic.color.a,options.tweenOptions);
        }
    }



    public static class CanvasGroupAlphaExtensions{
        public static TweenOperation AlphaToAsync(this CanvasGroup canvasGroup,AlphaToOptions options){
            return new To<float>(options.alpha).Property(
                canvasGroup,
                Properties.canvasGroup.alpha,
                options.tweenOptions
            );
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
