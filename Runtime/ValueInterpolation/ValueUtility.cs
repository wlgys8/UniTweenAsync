using UnityEngine;

namespace MS.TweenAsync{
    using System;

    public delegate T LerpFunction<T>(T from,T to,float t);

    public delegate T AddFunc<T>(T v1,T v2);
    public delegate T MulFunc<T>(T v,float mul);

    public delegate float DisFunc<T>(T v1, T v2);

    internal static class ValueUtility<T>{
        private static AddFunc<T> _addFunc;
        private static MulFunc<T> _mulFunc;
        private static DisFunc<T> _disFunc;

        public static void ImplLerp(AddFunc<T> addFunc,MulFunc<T> mulFunc){
            if(isSupportLerp){
                throw new System.InvalidOperationException($"duplicately impl for type : {typeof(T)}");
            }
            _addFunc = addFunc;
            _mulFunc = mulFunc;
        }

        public static void ImplDis(DisFunc<T> disFunc){
            if(isSupportDis){
                throw new System.InvalidOperationException($"duplicately impl distance operation for type : {typeof(T)}");
            }
            _disFunc = disFunc;
        }

        public static bool isSupportLerp{
            get{
                return isSupportAdd && isSupportMulWithConstant;
            }
        }

        public static bool isSupportAdd{
            get{
                return _addFunc != null;
            }
        }

        public static bool isSupportMulWithConstant{
            get{
                return _mulFunc != null;
            }
        }

        public static bool isSupportDis{
            get{
                return _disFunc != null;
            }
        }

        public static T Lerp(T from,T to,float t){
            if(!isSupportLerp){
                throw new NotImplementedException();
            }
            return _addFunc(_mulFunc(from,1 - t),_mulFunc(to,t));
        }

        public static T Add(T v1,T v2){
            return _addFunc(v1,v2);
        }

        public static T Mul(T v,float mul){
            return _mulFunc(v,mul);
        }

        public static float Distance(T v1, T v2){
            return _disFunc(v1,v2);
        }
    }

    public static class ValueUtility{
        static ValueUtility(){
            ValueUtility<float>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );
            ValueUtility<float>.ImplDis((v1,v2)=>{
                return Mathf.Abs(v2 - v1);
            });

            ValueUtility<int>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return (int)(value * t);
                }
            );

            ValueUtility<int>.ImplDis((v1,v2)=>{
                return Mathf.Abs(v2 - v1);
            });

            ValueUtility<double>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );

            ValueUtility<double>.ImplDis((v1,v2)=>{
                return (float)System.Math.Abs(v2 - v1);
            });


            ValueUtility<Color>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );

            ValueUtility<Vector2>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );

            ValueUtility<Vector2>.ImplDis((v1,v2)=>{
                return Vector2.Distance(v1,v2);
            });

            ValueUtility<Vector3>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );

            ValueUtility<Vector3>.ImplDis((v1,v2)=>{
                return Vector3.Distance(v1,v2);
            });


            ValueUtility<Vector4>.ImplLerp(
                addFunc:(from,to)=>{
                    return from + to;
                },
                mulFunc:(value,t)=>{
                    return value * t;
                }
            );

            ValueUtility<Vector4>.ImplDis((v1,v2)=>{
                return Vector4.Distance(v1,v2);
            });


            ValueUtility<Quaternion>.ImplLerp(
                addFunc:(from,to)=>{
                    return from * to;
                },
                mulFunc:(value,t)=>{
                    return Quaternion.LerpUnclamped(Quaternion.identity,value,t);
                }
            );

            ValueUtility<Quaternion>.ImplDis((v1,v2)=>{
                return Quaternion.Angle(v1,v2);
            });
        }

        /// <summary>
        /// do nothing, only for cctor be called
        /// </summary>
        public static void RegisterBuiltin(){}

        internal static void AssertSupport<T>(){
            if(!ValueUtility<T>.isSupportLerp){
                throw new NotImplementedException("not support ValueTween for type:" + typeof(T));
            }
        }

        public static void ImplLerp<T>(AddFunc<T> add,MulFunc<T> mul){
            ValueUtility<T>.ImplLerp(add,mul);
        }

        public static void ImplDistance<T>(DisFunc<T> dis){
            ValueUtility<T>.ImplDis(dis);
        }

        public static T Lerp<T>(T from,T to,float t){
            return ValueUtility<T>.Lerp(from,to,t);
        }

        public static T Add<T>(T from,T to){
            return ValueUtility<T>.Add(from,to);
        }

        public static float Distance<T>(T from,T to){
            return ValueUtility<T>.Distance(from,to);
        }

        public static bool IsSupportDistance<T>(){
            return ValueUtility<T>.isSupportDis;
        }

        public static bool IsSupportLerp<T>(){
            return ValueUtility<T>.isSupportLerp;
        }
    }
}
