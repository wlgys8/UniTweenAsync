using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MS.Async;

namespace MS.TweenAsync{
    using System;
    using MS.Async.CompilerServices;

    public partial struct TweenOperation
    {
        private ITweenActionDriver _actionDriver;
        private short _token;
        private float _duration;

        internal TweenOperation(ITweenActionDriver actionDriver){
            _actionDriver = actionDriver;
            _token = _actionDriver.token;
            _duration = actionDriver.duration;
        }

        /// <summary>
        /// Restart and return a new TweenOperation.
        /// The old operation can not be used anymore.
        /// </summary>
        internal TweenOperation Restart(){
            if(_actionDriver == null){
                return default(TweenOperation);
            }
            if(_actionDriver.token != _token){
                throw new InvalidOperationException("Tween has been disposed.");
            }
            _actionDriver.Restart();
            return new TweenOperation(_actionDriver);
        }

        private void AssertTokenNotExpired(){
            if(isExpired){
                throw new InvalidOperationException("The operation has beed disposed. You should not use it anymore.");
            }
        }

        private bool isExpired{
            get{
                if(_actionDriver == null){
                    return false;
                }
                return _token != _actionDriver.token;
            }
        }

        internal bool autoRelease{
            set{
                if(_actionDriver == null){
                    return;
                }
                AssertTokenNotExpired();
                _actionDriver.autoRelease = value;
            }
        }

        internal void Release(){
            if(_actionDriver == null){
                return;
            }
            AssertTokenNotExpired();
            _actionDriver.Release();
        }

        public bool isRunning{
            get{
                return !isCompleted && _actionDriver.status == TweenStatus.Running;
            }
        }

        public bool isInitialized{
            get{
                return _actionDriver != null;
            }
        }

        public float duration{
            get{
                return _duration;
            }
        }

        public float time{
            get{
                if(!isInitialized){
                    return 0;
                }
                if(this.isExpired){
                    return 0;
                }
                return _actionDriver.elapsedTime;
            }internal set{
                if(!isInitialized || isExpired){
                    return;
                }
                _actionDriver.elapsedTime = value;
            }
        }

        public float normalizedTime{
            get{
                return time / duration;
            }internal set{
                this.time = duration * value;
            }
        }

        /// <summary>
        /// Check if Tween is completed. <br/>
        /// A default(TweenOperation) is always completed.<br/>
        /// A released operation is also completed.
        /// </summary>
        public bool isCompleted{
            get{
                return !isInitialized || isExpired || _actionDriver.IsCompleted();
            }
        }

        /// <summary>
        /// Locate the tween to end immendiately
        /// </summary>
        public void RanToEnd(){
            if(isCompleted){
                return;
            }
            _actionDriver.RanToEnd();
        }

        /// <summary>
        /// Cancel the tween animation
        /// </summary>
        public void Cancel(){
            if(isCompleted){
                return;
            }
            _actionDriver.Cancel();
        }


        /// <summary>
        /// Pause or Resume the tween
        /// </summary>
        public bool paused{
            get{
                if(isCompleted){
                    return false;
                }
                return _actionDriver.paused;
            }set{
                if(isCompleted){
                    return;
                }
                _actionDriver.paused = value;
            }
        }

        /// <summary>
        /// Return awaitable Task. Task will be done until TweenOperation completed.
        /// </summary>
        public LitTask Task{
            get{
                if(isCompleted){
                    return default(LitTask);
                }
                var source = TweenSource.Request(_actionDriver);
                return new LitTask(source,source.Token);
            }
        }
    }

    internal class TweenSource : ILitTaskValueSource
    {

        private static TokenAllocator _tokenAllocator = new TokenAllocator();
        private static Stack<TweenSource> _pool = new Stack<TweenSource>();

        public static TweenSource Request(ITweenActionDriver driver){
            TweenSource source = null;
            if(_pool.Count > 0){
                source = _pool.Pop();
            }else{
                source = new TweenSource();
            }
            source.Initialize(driver,_tokenAllocator.Next());
            return source;
        }

        private ITweenActionDriver _driver;

        private Action _continuation;

        private short _token;
        
        private void Initialize(ITweenActionDriver driver,short token){
            _token = token;
            _driver = driver;
        }

        private void ReturnToPool(){
            _token = 0;
            _driver = null;
            _continuation = null;
            _pool.Push(this);
        }

        internal short Token{
            get{
                return _token;
            }
        }

        private void AssertTokenNotExpired(short token){
            if(_token != token){
                throw new InvalidOperationException("Token expired");
            }
        }

        public void Continue(short token, Action<LitTaskResult> action)
        {
            AssertTokenNotExpired(token);
        }

        public void GetResult(short token)
        {
             AssertTokenNotExpired(token);
             try{
                var status = GetStatus(token);
                switch(status){
                    case ValueSourceStatus.Canceled:
                    LitCancelException.Throw();
                    break;
                    default:
                    return;
                }
             }finally{
                this.ReturnToPool();
             }
        }

        public ValueSourceStatus GetStatus(short token)
        {
            AssertTokenNotExpired(token);
            if(_driver.IsCancelled()){
                return ValueSourceStatus.Canceled;
            }else if(_driver.IsCompleted()){
                return ValueSourceStatus.Succeed;
            }else{
                return ValueSourceStatus.Pending;
            }
        }

        public void OnCompleted(Action continuation, short token)
        {
            AssertTokenNotExpired(token);
            _driver.AddOnComplete(continuation);
        }
    }

}
