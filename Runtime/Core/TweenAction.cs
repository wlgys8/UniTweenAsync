using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MS.TweenAsync{

    public delegate void OnStart<TState>(ref TState state);

    public delegate void OnUpdate<TState>(ActionState actionState,ref TState state);
    public delegate void OnComplete<TState>(ActionState actionState, ref TState state);

    public delegate void OnPreRelease<TState>(ActionState actionState,ref TState state);

    /// <summary>
    /// <para>TweenAction依次处理如下的事件: </para>
    /// <para> OnStart 动画启始时调用 </para>
    /// <para> OnUpdate 动画更新调用 </para>
    /// <para> OnComplete 动画结束调用 </para>
    /// <para> OnPreRelease 动画释放调用 </para>
    /// </summary>
    public class TweenAction<TState>{
        
        private static OnStart<TState> _onStart;
        private static OnComplete<TState> _onComplete;
        private static OnUpdate<TState> _onUpdate;

        private static OnPreRelease<TState> _onPreRelease;

        public static void RegisterStart(OnStart<TState> onStart){
            _onStart = onStart;
        }
        public static void RegisterComplete(OnComplete<TState> onComplete){
            _onComplete = onComplete;
        }
        public static void RegisterUpdate(OnUpdate<TState> onUpdate){
            _onUpdate = onUpdate;
        }

        public static void RegisterPreRelease(OnPreRelease<TState> onPreRelease){
            _onPreRelease = onPreRelease;
        }

        public static TweenOperation Prepare(TState state,TweenOptions options){
            var driver = TweenActionDriver<TState>.Prepare(state,options);
            return new TweenOperation(driver);
        }

        internal static void Start(ref TState state){
            if(_onStart != null){
                _onStart(ref state);
            }
        }
        internal static void Complete(ActionState actionState, ref TState state){
            if(_onComplete != null){
                _onComplete(actionState,ref state);
            }
        }
        internal static void Update(ActionState actionState,ref TState state){
            if(_onUpdate != null){
                _onUpdate(actionState,ref state);
            }
        }

        internal static void PreRelease(ActionState actionState, ref TState state){
            if(_onPreRelease != null){
                _onPreRelease(actionState,ref state);
            }
        }
  
    }


    public struct ActionState{
        
        private float _duration;

        private EaseFunction _ease;

        internal ActionState(float duration):this(duration,EaseFuncs.Linear){
        }
        internal ActionState(float duration,EaseFunction ease){
            _duration = duration;
            this.elapsedTime = 0;
            _ease = ease;
            this.status = TweenStatus.NotPrepared;
        }

        public TweenStatus status{
            get;internal set;
        }
        public float duration{
            get{
                return _duration;
            }
        }
        public float normalizedTime{
            get{
                return Mathf.Clamp01(this.elapsedTime / this.duration);
            }
        }

        /// <summary>
        /// Equals to Interpolate(normalizedTime)
        /// </summary>
        public float interpolatedTime{
            get{
                return _ease(normalizedTime);
            }
        }


        public float elapsedTime{
            get;internal set;
        }
    }

}
