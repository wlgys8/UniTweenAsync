using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{


    public static partial class Properties{

        [TweenTarget(typeof(RectTransform))]
        public static partial class rectTransform{
            public static readonly PropertyVector2Accesser<UnityEngine.RectTransform> anchoredPosition = new PropertyVector2Accesser<UnityEngine.RectTransform>(
                setter:(transform,value)=>{
                    transform.anchoredPosition = value;
                },
                getter:(transform)=>{
                    return transform.anchoredPosition;
                }
            );
            public static readonly PropertyVector3Accesser<UnityEngine.RectTransform> anchoredPosition3D = new PropertyVector3Accesser<UnityEngine.RectTransform>(
                setter:(transform,value)=>{
                    transform.anchoredPosition3D = value;
                },
                getter:(transform)=>{
                    return transform.anchoredPosition3D;
                }
            );

            public static readonly PropertyVector2Accesser<UnityEngine.RectTransform> size = new PropertyVector2Accesser<UnityEngine.RectTransform>(
                setter:(transform,value)=>{
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,value.x);
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,value.y);
                },
                getter:(transform)=>{
                    return transform.rect.size;
                }
            );


        }

    }
}
