using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MS.TweenAsync{

    public static partial class Properties{

        [TweenTarget(typeof(Graphic))]
        public static partial class graphic{

            public static readonly PropertyColorAccesser<Graphic> color = new PropertyColorAccesser<Graphic>(
                setter:(graphic,value)=>{
                    graphic.color = value;
                },
                getter:(graphic)=>{
                    return graphic.color;
                }
            );
        }
    }
}
