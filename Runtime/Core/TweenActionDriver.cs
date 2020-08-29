using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MS.TweenAsync{
    using MS.Async;
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

        TweenStatus status{
            get;
        }
    }

    internal enum TweenStatus{
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

        private TweenStatus _status = TweenStatus.NotPrepared;

        private TweenTicker.TickAction _tickAction;

        private TweenOptions _tweenOptions;

        private bool _paused = false;

        // private ManualSingal _startSingal;

        
        private TweenActionDriver(){
            _tickAction = (data)=>{
                this.Tick(data);
            };
        }

        private void Prepare(TState userState,TweenOptions options,short token){
            if(_status != TweenStatus.NotPrepared){
                throw new System.InvalidOperationException("Status error:" + _status);
            }
            _tweenOptions = options;
            _actionState = new ActionState(options.duration,options.ease);
            _userState = userState;
            _paused = false;
            _token = token;
            _status = TweenStatus.Prepared;
            TweenTicker.AddTick(this._tickAction);
        }

        // /// <summary>
        // /// if current action is cancelled. task will be cancelled.
        // /// if current action is running or succeed, task will completed immediately.
        // /// if current action is prepared,task will be succeed when action start run, or cancelled if the action cancelled before start run.
        // /// </summary>
        // /// <returns></returns>
        // public async LitTask WaitStartAsync(){
        //     if(status == TweenStatus.NotPrepared){
        //         throw new InvalidOperationException();
        //     }
        //     if(status == TweenStatus.Cancelled){
        //         LitCancelException.Throw();
        //         return;
        //     }
        //     if(status == TweenStatus.Succeed || status == TweenStatus.Running){
        //         return;
        //     }
        //     //only await in prepared status
        //     if(_startSingal == null){
        //         _startSingal = new ManualSingal();
        //     }
        //     await _startSingal;
        // }


        public float duration{
            get{
                return _actionState.duration;
            }
        }
        
        private void ChangeStatusToRunning(){
            AssertStatus(TweenStatus.Prepared);
            _status = TweenStatus.Running;
            TweenAction<TState>.Start(ref _userState);
            // if(_startSingal != null){
            //     _startSingal.SetResult();
            // }
        }

        private void ReturnToPool(){
            // if(_startSingal != null && _startSingal.awaitingCount > 0){
            //     _startSingal.Reset();
            // }
            _continuations.Clear();
            _token = 0;
            _pool.Push(this);
            _status = TweenStatus.NotPrepared;
            _userState = default(TState);
        }

        private void AssertPreparedOrRunning(){
             if(_status != TweenStatus.Prepared && _status != TweenStatus.Running){
                throw new InvalidOperationException("Prepared or Running Status Required");
            }                  
        }

        private void AssertStatus(TweenStatus status){
            if(_status != status){
                throw new InvalidOperationException($"Status error. need {status}, current is {_status}");
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
            get{
                return _paused;
            }set{
                if(_paused == value){
                    return;
                }
                _paused = value;
                if(value){
                    TweenTicker.RemoveTick(this._tickAction);
                }else{
                    TweenTicker.AddTick(this._tickAction);
                }
            }
        }

        public void RanToEnd(){
            if(_status == TweenStatus.Prepared){
                ChangeStatusToRunning();
            }
            AssertStatus(TweenStatus.Running);
            _actionState.elapsedTime = _actionState.duration;
            TweenAction<TState>.Update(_actionState,ref _userState);
            this.SucceedToEnd();
        }

        public void Cancel(){
            AssertPreparedOrRunning();
            _status = TweenStatus.Cancelled;
            // if(_startSingal != null && _startSingal.awaitingCount > 0){
            //     _startSingal.Cancel();
            // }
            FireOnComplete();
        }

        public bool IsCancelled(){
            return _status == TweenStatus.Cancelled;
        }

        public void AddOnComplete(Action continuation){
            AssertNotCompleted();
            _continuations.Add(continuation);
        }

        public float elapsedTime{
            set{
                if(_status != TweenStatus.Running && _status != TweenStatus.Prepared){
                    throw new InvalidOperationException("elapsedTime can only be set at running or prepared action.");
                }
                if(_status == TweenStatus.Prepared){
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
            _status = TweenStatus.Succeed;
            FireOnComplete();
        }

        private void FireOnComplete(){
            if(!this.paused){
                TweenTicker.RemoveTick(this._tickAction);
            }
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
            return _status == TweenStatus.Succeed || _status == TweenStatus.Cancelled;
        }

        public TweenStatus status{
            get{
                return _status;
            }
        }
    }
}
