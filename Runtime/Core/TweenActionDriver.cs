using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{
    using System;
    using Async.CompilerServices;
    using Async.Diagnostics;
    using MS.CommonUtils;

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

        bool autoRelease{
            get;set;
        }

        void Release();

        bool IsCancelled();

        bool IsCompleted();

        void AddOnComplete(Action action);

        void Restart();

        TweenStatus status{
            get;
        }
    }

    public enum TweenStatus{
        NotPrepared,
        Prepared,
        Running,
        Succeed,
        Cancelled,
    }

    internal class TweenActionDriver<TState>:ITweenActionDriver,ITracableObject{

        private static AutoAllocatePool<TweenActionDriver<TState>> _pool = new AutoAllocatePool<TweenActionDriver<TState>>();
        private static TokenAllocator _tokenAllocator = new TokenAllocator();
        public static TweenActionDriver<TState> Prepare(TState state, TweenOptions options){
            TweenActionDriver<TState> driver = _pool.Request();
            driver.Prepare(state,options,_tokenAllocator.Next());
            Trace.TraceAllocation(driver);
            return driver;
        }

        private ActionState _actionState;
        private TState _userState;

        private List<Action> _continuations = new List<Action>();
        private short _token;

        private TweenTicker.TickAction _tickAction;

        private TweenOptions _tweenOptions;

        private bool _paused = false;

        private bool _isTickRegistered = false;

        // private ManualSingal _startSingal;

        
        public TweenActionDriver(){
            _tickAction = (data)=>{
                this.Tick(data);
            };
        }

        public string DebugNameId{
            get{
                return typeof(TState).Name;
            }
        }


        private void Prepare(TState userState,TweenOptions options,short token){
            if(status != TweenStatus.NotPrepared){
                throw new System.InvalidOperationException("Status error:" + this.status);
            }
            _tweenOptions = options;
            _actionState = new ActionState(options.duration,options.ease);
            _userState = userState;
            _paused = false;
            _token = token;
            _actionState.status = TweenStatus.Prepared;
            this.autoRelease = true;
            this.RegisterTick();
        }

        private void RegisterTick(){
            if(_isTickRegistered){
                return;
            }
            _isTickRegistered = true;
            TweenTicker.AddTick(this._tickAction);
        }

        private void UnregisterTick(){
            if(!_isTickRegistered){
                return;
            }
            _isTickRegistered = false;
            TweenTicker.RemoveTick(this._tickAction);
        }



        public float duration{
            get{
                return _actionState.duration;
            }
        }
        
        private void ChangeStatusToRunning(){
            AssertStatus(TweenStatus.Prepared);
            _actionState.status = TweenStatus.Running;
            TweenAction<TState>.Start(ref _userState);
        }

        public void Restart(){
            AssertCompleted();
            if(autoRelease){
               throw new System.InvalidOperationException("autoRelease mode tween can not be restarted"); 
            }
            _actionState.elapsedTime = 0;
            _token = _tokenAllocator.Next();
            _actionState.status = TweenStatus.Prepared;
        }

        private void ReturnToPool(){
            AssertCompleted();
            TweenAction<TState>.PreRelease(_actionState,ref _userState);
            UnregisterTick();
            _continuations.Clear();
            _token = 0;
            _pool.Release(this);
            _actionState.status = TweenStatus.NotPrepared;
            _userState = default(TState);
            Trace.TraceReturn(this);
        }

        private void AssertPreparedOrRunning(){
             if(this.status != TweenStatus.Prepared && this.status != TweenStatus.Running){
                throw new InvalidOperationException($"Prepared or Running Status Required, current is {this.status}");
            }                  
        }

        private void AssertNotStatus(TweenStatus status){
            if(this.status == status){
                throw new InvalidOperationException($"Status error. can not be {status}");
            }
        }
        private void AssertStatus(TweenStatus status){
            if(this.status != status){
                throw new InvalidOperationException($"Status error. need {status}, current is {this.status}");
            }
        }

        private void AssertNotCompleted(){
            if(IsCompleted()){
                throw new InvalidOperationException("Already Completed");
            }
        }

        private void AssertCompleted(){
            if(!IsCompleted()){
                throw new InvalidOperationException($"status error. Need completed, current is {this.status}");
            }
        }

        public short token{
            get{
                return _token;
            }
        }

        public bool paused{
            get{
                return _paused;
            }set{
                if(_paused == value){
                    return;
                }
                AssertNotStatus(TweenStatus.NotPrepared);
                _paused = value;
                if(value){
                    UnregisterTick();
                }else{
                    RegisterTick();
                }
            }
        }

        public void RanToEnd(){
            if(this.status == TweenStatus.Prepared){
                ChangeStatusToRunning();
            }
            AssertStatus(TweenStatus.Running);
            _actionState.elapsedTime = _actionState.duration;
            TweenAction<TState>.Update(_actionState,ref _userState);
            this.SucceedToEnd();
        }

        public void Cancel(){
            AssertPreparedOrRunning();
            _actionState.status = TweenStatus.Cancelled;
            FireOnComplete();
        }

        public bool IsCancelled(){
            return this.status == TweenStatus.Cancelled;
        }

        public void AddOnComplete(Action continuation){
            AssertNotStatus(TweenStatus.NotPrepared);
            AssertNotCompleted();
            _continuations.Add(continuation);
        }

        public float elapsedTime{
            set{
                if(status != TweenStatus.Running && status != TweenStatus.Prepared){
                    throw new InvalidOperationException($"elapsedTime can only be set at running or prepared action. curret status is {this.status}");
                }
                if(status == TweenStatus.Prepared){
                    ChangeStatusToRunning();
                }
                _actionState.elapsedTime = value;
                try{
                    TweenAction<TState>.Update(_actionState,ref _userState);
                    if(_actionState.elapsedTime >= _actionState.duration){
                        this.SucceedToEnd();
                    }
                }catch(System.Exception e){
                    Debug.LogException(e);
                    this.Cancel();
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
            _actionState.status = TweenStatus.Succeed;
            FireOnComplete();
        }

        /// <summary>
        /// If AutoRelease is true, the ActionDriver will be released to pool automatically when it is go to completed.
        /// Otherwise, you should call Release manually when dont need use it anymore.
        /// By default, it is true.
        /// </summary>
        /// <value></value>
        public bool autoRelease{
            get;set;
        }

        /// <summary>
        /// Release the object back to pool.
        /// Can only be called when action completed.
        /// </summary>
        public void Release(){
            this.ReturnToPool();
        }

        private void FireOnComplete(){
            UnregisterTick();
            try{
                TweenAction<TState>.Complete(this._actionState, ref _userState);
            }catch(System.Exception e){
                Debug.LogException(e);
            }
            try{
                foreach(var continuation in _continuations){
                    try{
                        continuation();
                    }catch(System.Exception e){
                        Debug.LogException(e);
                    }
                }
            }finally{
                if(autoRelease){
                    this.ReturnToPool();
                }
            }
        }

        public bool IsCompleted(){
            return status == TweenStatus.Succeed || status == TweenStatus.Cancelled;
        }

        public TweenStatus status{
            get{
                return _actionState.status;
            }
        }
    }
}
