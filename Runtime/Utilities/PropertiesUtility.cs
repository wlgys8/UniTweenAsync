using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{
    public static class PropertiesUtility
    {

        /// <summary>
        /// 根据位置属性创建一个角度空间属性
        /// </summary>
        public static PropertyAccesser<TObject,float> Degree<TObject>(PropertyAccesser<TObject,Vector2> position,Vector2 center,float radius){
            return new PropertyAccesser<TObject, float>(
                setter:(obj,value)=>{
                    var x = Mathf.Cos(value * Mathf.Deg2Rad);
                    var y = Mathf.Sin(value * Mathf.Deg2Rad);
                    position.Set(obj,radius * new Vector2(x,y));
                },
                getter:(obj)=>{
                    var pos = position.Get(obj);
                    var vec = pos - center;
                    return Vector2.SignedAngle(Vector2.right,vec);
                }
            );
        }

        /// <summary>
        /// 创建一个属性，代表与目标对象的距离. 其中TValue必须支持距离运算符
        /// </summary>
        public static PropertyAccesser<TObject,float> Distance<TObject,TValue>(TObject target,PropertyAccesser<TObject,TValue> property){
            return new PropertyAccesser<TObject, float>(
                setter:(source,value)=>{
                    var from = property.Get(source);
                    var to = property.Get(target);
                    var dis = ValueUtility.Distance(from,to);
                    var resultValue = ValueUtility.Lerp(from,to,1 - value / dis);
                    property.Set(source,resultValue);
                },
                getter:(source)=>{
                    var from = property.Get(source);
                    var to = property.Get(target);
                    return ValueUtility.Distance(from,to);
                }
            );
        }
    }
}
