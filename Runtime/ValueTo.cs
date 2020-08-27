using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MS.TweenAsync{


    public static class ValueUtility<T>{
        private static Func<T,T,float,T> _lerpOperator;
        public static Func<T,T,float,T> lerpOperator{
            set{
                if(_lerpOperator != null){
                    throw new InvalidOperationException("alreay implemented lerpOperator for type:" + typeof(T));
                }
                _lerpOperator = value;
            }
        }

        public static bool isSupport{
            get{
                return _lerpOperator != null;
            }
        }

        public static T Lerp(T from,T to,float t){
            if(_lerpOperator == null){
                throw new NotImplementedException();
            }
            return _lerpOperator(from,to,t);
        }
    }

    public static class ValueUtility{
        static ValueUtility(){
            ValueUtility<float>.lerpOperator = (from,to,t)=>{
                return Mathf.LerpUnclamped(from,to,t);
            };
            ValueUtility<int>.lerpOperator = (from,to,t)=>{
                return from + (int)((to - from) * t);
            };
            ValueUtility<double>.lerpOperator = (from,to,t)=>{
                return from + (to - from) * t;
            };
        }

        internal static void AssertSupport<T>(){
            if(!ValueUtility<T>.isSupport){
                throw new NotImplementedException("not support ValueTween for type:" + typeof(T));
            }
        }

        public static T Lerp<T>(T from,T to,float t){
            return ValueUtility<T>.Lerp(from,to,t);
        }
    }

    public static class ValueTween<T>{

        static ValueTween(){
            ValueUtility.AssertSupport<T>();
            TweenAction<ValueToState>.RegisterUpdate(_valueToOnUpdate);
        }

        public static TweenOperation ToAsync(ValueToOptions<T> options){
            var state = new ValueToState(){
                options = options
            };
            return TweenAction<ValueToState>.Prepare(state,options.tweenOptions);
        }
   
        private static OnUpdate<ValueToState> _valueToOnUpdate = (ActionState actionState,ref ValueToState state)=>{
            var options = state.options;
            var value = ValueUtility.Lerp(options.from,options.to,actionState.interpolatedTime);
            if(options.onUpdate != null){
                options.onUpdate(value);
            }
        };

        private struct ValueToState{
            public ValueToOptions<T> options;
        }
    }

    public struct ValueToOptions<T>{
        public T from;
        public T to;
        public System.Action<T> onUpdate;
        public TweenOptions tweenOptions;

        public ValueToOptions(T from,T to,Action<T> onUpdate,TweenOptions options){
            this.from = from;
            this.to = to;
            this.onUpdate = onUpdate;
            this.tweenOptions = options;
        }
        public ValueToOptions(T from,T to,Action<T> onUpdate,float duration):this(from,to,onUpdate,new TweenOptions(duration)){}

        public ValueToOptions(T from,T to,Action<T> onUpdate):this(from,to,onUpdate,1){}

        public TweenOperation RunAsync(){
            return ValueTween<T>.ToAsync(this);
        }
    }
}
