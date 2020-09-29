using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{


    public static partial class Properties{

        [TweenTarget(typeof(Transform))]
        public static partial class transform{
            public static readonly PropertyVector3Accesser<UnityEngine.Transform> localPosition = new PropertyVector3Accesser<UnityEngine.Transform>(
                setter:(transform,localPosition)=>{
                    transform.localPosition = localPosition;
                },
                getter:(transform)=>{
                    return transform.localPosition;
                }
            );

            public static readonly PropertyVector3Accesser<UnityEngine.Transform> position = new PropertyVector3Accesser<UnityEngine.Transform>(
                setter:(transform,value)=>{
                    transform.position = value;
                },
                getter:(transform)=>{
                    return transform.position;
                }
            );   

            public static readonly PropertyVector3Accesser<UnityEngine.Transform> localScale = new PropertyVector3Accesser<UnityEngine.Transform>(
                setter:(transform,value)=>{
                    transform.localScale = value;
                },
                getter:(transform)=>{
                    return transform.localScale;
                }
            );  

            public static readonly PropertyVector3Accesser<UnityEngine.Transform> localEulerAngles = new PropertyVector3Accesser<UnityEngine.Transform>(
                setter:(transform,value)=>{
                    transform.localEulerAngles = value;
                },
                getter:(transform)=>{
                    return transform.localEulerAngles;
                }
            );   
            public static readonly PropertyVector3Accesser<UnityEngine.Transform> eulerAngles = new PropertyVector3Accesser<UnityEngine.Transform>(
                setter:(transform,value)=>{
                    transform.eulerAngles = value;
                },
                getter:(transform)=>{
                    return transform.eulerAngles;
                }
            );  
            public static readonly PropertyAccesser<UnityEngine.Transform,Quaternion> localRotation = new PropertyAccesser<UnityEngine.Transform, Quaternion>(
                setter:(transform,value)=>{
                    transform.localRotation = value;
                },
                getter:(transform)=>{
                    return transform.localRotation;
                }
            );  

            public static readonly PropertyAccesser<UnityEngine.Transform,Quaternion> rotation = new PropertyAccesser<UnityEngine.Transform, Quaternion>(
                setter:(transform,value)=>{
                    transform.rotation = value;
                },
                getter:(transform)=>{
                    return transform.rotation;
                }
            );
        }

  
    }
}