using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MS.TweenAsync{
    using System;
    using Async.CompilerServices;

    internal interface ITweenActionDriver{

        void RanToEnd();

        void Cancel();

        bool paused{
            get;set;
        }

        short token{
            get;
        }

        float duration{
            get;
        }

        float elapsedTime{
            get;set;
        }

        bool IsCancelled();

        bool IsCompleted();

        void AddOnComplete(Action action);
    }

    internal enum TweenActionDriverStatus{
        NotPrepared,
        Prepared,
        Running,
        Succeed,
        Cancelled,
    }

    internal class TweenActionDriver<TState>:ITweenActionDriver{

        private static Stack<TweenActionDriver<TState>> _pool = new Stack<TweenActionDriver<TState>>();
        private static TokenAllocator _tokenAllocator = new TokenAllocator();
        public static TweenActionDriver<TState> Prepare(TState state, TweenOptions options){
            TweenActionDriver<TState> driver = null;
            if(_pool.Count > 0){
                driver = _pool.Pop();
            }else{
                driver = new TweenActionDriver<TState>();
            }
            driver.Prepare(state,options,_tokenAllocator.Next());
            return driver;
        }

        private ActionState _actionState;
        private TState _userState;

        private List<Action> _continuations = new List<Action>();
        private short _token;

        private TweenActionDriverStatus _status = TweenActionDriverStatus.NotPrepared;

        private TweenTicker.TickAction _tickAction;

        private TweenOptions _tweenOptions;

        
        private TweenActionDriver(){
            _tickAction = (data)=>{
                this.Tick(data);
            };
        }

        private void Prepare(TState userState,TweenOptions options,short token){
            if(_status != TweenActionDriverStatus.NotPrepared){
                throw new System.InvalidOperationException("Status error:" + _status);
            }
            _tweenOptions = options;
            _actionState = new ActionState(options.duration,options.ease);
            _userState = userState;
            this.paused = false;
            _token = token;
            _status = TweenActionDriverStatus.Prepared;
            TweenTicker.AddTick(this._tickAction);
        }

        public float duration{
            get{
                return _actionState.duration;
            }
        }
        
        private void ChangeStatusToRunning(){
            AssertPrepared();
            _status = TweenActionDriverStatus.Running;
            TweenAction<TState>.Start(ref _userState);
        }

        private void ReturnToPool(){
            _continuations.Clear();
            _token = 0;
            _pool.Push(this);
            _status = TweenActionDriverStatus.NotPrepared;
            _userState = default(TState);
        }

        private void AssertPrepared(){
            if(_status != TweenActionDriverStatus.Prepared){
                throw new InvalidOperationException("Prepared Status Required");
            }           
        }

        private void AssertPreparedOrRunning(){
             if(_status != TweenActionDriverStatus.Prepared && _status != TweenActionDriverStatus.Running){
                throw new InvalidOperationException("Prepared or Running Status Required");
            }                  
        }

        private void AssertNotCompleted(){
            if(IsCompleted()){
                throw new InvalidOperationException("Already Completed");
            }
        }

        public short token{
            get{
                return _token;
            }
        }

        public bool paused{
            get;set;
        }

        public void RanToEnd(){
            AssertPreparedOrRunning();
            _actionState.elapsedTime = _actionState.duration;
            TweenAction<TState>.Update(_actionState,ref _userState);
            this.SucceedToEnd();
        }

        public void Cancel(){
            AssertPreparedOrRunning();
            _status = TweenActionDriverStatus.Cancelled;
            FireOnComplete();
        }

        public bool IsCancelled(){
            return _status == TweenActionDriverStatus.Cancelled;
        }

        public void AddOnComplete(Action continuation){
            AssertNotCompleted();
            _continuations.Add(continuation);
        }

        public float elapsedTime{
            set{
                if(_status != TweenActionDriverStatus.Running && _status != TweenActionDriverStatus.Prepared){
                    throw new InvalidOperationException("elapsedTime can only be set at running or prepared action.");
                }
                if(_status == TweenActionDriverStatus.Prepared){
                    ChangeStatusToRunning();
                }
                _actionState.elapsedTime = value;
                TweenAction<TState>.Update(_actionState,ref _userState);
                if(_actionState.elapsedTime >= _actionState.duration){
                    this.SucceedToEnd();
                }
            }get{
                return _actionState.elapsedTime;
            }
        }

        public void Tick(TweenTicker.TickData data){
            AssertPreparedOrRunning();
            if(paused){
                return;
            }
            if(_tweenOptions.ignoreTimeScale){
                this.elapsedTime += data.unscaledDeltaTime;
            }else{
                this.elapsedTime += data.scaledDeltaTime;
            }
        }

        private void SucceedToEnd(){
            AssertPreparedOrRunning();
            _status = TweenActionDriverStatus.Succeed;
            FireOnComplete();
        }

        private void FireOnComplete(){
            TweenTicker.RemoveTick(this._tickAction);
            try{
                TweenAction<TState>.Complete(ref _userState);
            }catch(System.Exception e){
                Debug.LogException(e);
            }
            try{
                foreach(var continuation in _continuations){
                    continuation();
                }
            }finally{
                this.ReturnToPool();
            }
        }

        public bool IsCompleted(){
            return _status == TweenActionDriverStatus.Succeed || _status == TweenActionDriverStatus.Cancelled;
        }
    }
}
