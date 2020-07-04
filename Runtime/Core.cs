using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MS.Async;
using System.Threading;

namespace MS.TweenAsync{

    public static class CoreUtils{

        public static async LitTask RunLerpAsync<T>(TweenOptions tweenOptions, OnLerp<T> onLerp,T state,TweenOperationToken operation = default){
            var postLerpType = tweenOptions.postLerpType;
            var ticker = new TimeTicker(tweenOptions.duration,tweenOptions.postLerpType);
            var ease = tweenOptions.ease;
            while(true){
                float deltaTime = await operation.WaitTickAsync(tweenOptions.ignoreTimeScale);
                ticker.Tick(deltaTime);
                var isEnd = ticker.isEnd;
                var normalizedTime = ticker.normalizedTime;
                if(isEnd && postLerpType == PostExtrapolateType.None){
                    break;
                }      
                var lerp = ease(normalizedTime);
                onLerp(lerp,state);
                if(isEnd){
                    break;
                }
            }
        }
           
    }

    public delegate void OnLerp<T>(float lerp,T state);   

    public enum PostExtrapolateType{
        None,
        Clamp
    }



    internal struct TimeTicker{
        private float _duration;
        private PostExtrapolateType _postExtrapoateType;

        private float _time;

        public TimeTicker(float duration = 1,PostExtrapolateType postExtrapolateType = PostExtrapolateType.Clamp){
            _duration = duration;
            _postExtrapoateType = postExtrapolateType;
            _time = 0;
        }

        public void Tick(float deltaTime){
            _time += deltaTime;
        }

        public float duration{
            get{
                if(_duration <= 0){
                    _duration = 1;
                }
                return _duration;
            }
        }

        public bool isEnd{
            get{
                return time >= this.duration;
            }
        }

        public float time{
            get{
                if(_time > duration && _postExtrapoateType == PostExtrapolateType.Clamp){
                    _time = duration;
                }
                return _time;
            }
        }

        public float normalizedTime{
            get{
                return time / duration;
            }
        }        
    }


    internal struct TweenClock{
        
        private float _startTime;
        private float _duration;
        private PostExtrapolateType _postExtrapoateType;
        private bool _ignoreTimeScale;

        public TweenClock(float duration = 1,PostExtrapolateType postExtrapolateType = PostExtrapolateType.Clamp,bool ignoreTimeScale = false){
            _duration = duration;
            _ignoreTimeScale = ignoreTimeScale;
            _postExtrapoateType = postExtrapolateType;
            _startTime = ignoreTimeScale?Time.realtimeSinceStartup:Time.time;
        }

        public float duration{
            get{
                if(_duration <= 0){
                    _duration = 1;
                }
                return _duration;
            }
        }

        public float currentTime{
            get{
                if(_ignoreTimeScale){
                    return Time.realtimeSinceStartup;
                }else{
                    return Time.time;
                }
            }
        }

        public void Start(){
            _startTime = currentTime;
        }

        public bool isEnd{
            get{
                return passedTime >= this.duration;
            }
        }
        public float passedTime{
            get{
                var res =  currentTime - _startTime;
                if(res > duration && _postExtrapoateType == PostExtrapolateType.Clamp){
                    res = duration;
                }
                return res;
            }
        }

        public float normalizedPassedTime{
            get{
                return passedTime / duration;
            }
        }

    }

    public struct TweenOptions{
        public float duration;
        public EaseFunction ease;
        public PostExtrapolateType postLerpType;
        public bool ignoreTimeScale;

        public TweenOptions(float duration){
            this.duration = duration;
            ease = EaseFuncs.Linear;
            postLerpType = PostExtrapolateType.Clamp;
            ignoreTimeScale = false;
        }
    }
}
