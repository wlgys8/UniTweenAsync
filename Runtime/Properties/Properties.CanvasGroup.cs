using UnityEngine;
using UnityEngine.UI;
namespace MS.TweenAsync{
    

    public static partial class Properties{

        [TweenTarget(typeof(CanvasGroup))]
        public static partial class canvasGroup{
            public static readonly PropertyAccesser<CanvasGroup,float> alpha = new PropertyAccesser<CanvasGroup, float>(
                setter:(canvasGroup,value)=>{
                    canvasGroup.alpha = value;
                },
                getter:(canvasGroup)=>{
                    return canvasGroup.alpha;
                }
            );
        }
    }


}
