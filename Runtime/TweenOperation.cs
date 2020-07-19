
using UnityEngine;

namespace MS.TweenAsync{
    using MS.Async;
    public class TweenOperation
    {
        private static short _globalTokenId;

        private static short NextTokenId(){
            return _globalTokenId ++;
        }


        private short _tokenId;

        private bool _cancellationRequest = false;
        private float _timeScale = 1;
        private AutoResetSingal<float> _manualTicker;
        private float _time = 0;
        private bool _manual = false;
        private float _lastCalculatedFrame = 0;

        public TweenOperation(){
            Reset();
        }

        public short tokenId{
            get{
                return _tokenId;
            }
        }

        /// <summary>
        /// reset all status and change the tokenId.
        /// </summary>
        public void Reset(){
            _tokenId = NextTokenId();
            _lastCalculatedFrame = 0;
            paused = false;
            _cancellationRequest = false;
            _timeScale = 1;
            _time = 0;
            if(_manualTicker != null){
                _manualTicker.SetCanceled(LitCancelException.DEFAULT);
            }
        }

        public void Tick(float seconds){
            if(!_manual){
                throw new System.InvalidOperationException("can only be called in manual mode.");
            }
            if(paused){
                //tick won't work when operation is paused.
                return;
            }
            if(seconds < 0){
                throw new System.ArgumentException($"tick seconds must >= 0, current is {seconds}");
            }
            _time += seconds;
            _manualTicker.SetResult(seconds);
        }

        public bool manual{
            get{
                return _manual;
            }set{
                if(_manual == value){
                    return;
                }
                _manual = value;
                if(value && _manualTicker == null){
                    _manualTicker = new AutoResetSingal<float>();
                }
            }
        }
        
        /// <summary>
        /// get current passed time.
        /// </summary>
        public float time{
            get{
                return _time;
            }
        }

        

        private void ThrowIfTokenIdChanged(short tokenId){
            if(_tokenId != tokenId){
                throw new System.ObjectDisposedException(this.GetType().Name);
            }
        }
        
        /// <summary>
        /// wait tick event and return the unscaled deltaTime between tick.
        /// </summary>
        internal async LitTask<float> WaitTickAsync(short tokenId,bool ignoreTimeScale = false){
            ThrowIfCancellationRequested(tokenId);
            if(_manual){
                var deltaTime = await _manualTicker;
                ThrowIfCancellationRequested(tokenId);
                if(!ignoreTimeScale){
                    deltaTime *= _timeScale;
                }
                return deltaTime;
            }else{
                var lastTime = 0f;
                do{
                    lastTime = Time.realtimeSinceStartup;
                    await new UnityUpdate();
                    ThrowIfCancellationRequested(tokenId);
                }while(paused);
                var deltaTime = Time.realtimeSinceStartup - lastTime;
                if(_lastCalculatedFrame != Time.frameCount){
                    //if current frame deltaTime not added to time
                    _lastCalculatedFrame = Time.frameCount;
                    _time += deltaTime ;
                }
                if(!ignoreTimeScale){
                    deltaTime *= _timeScale;
                }
                return deltaTime;
            }
        }

        /// <summary>
        /// pause/resume all tweens that controlled by this operation.
        /// </summary>
        public bool paused{
            get;set;
        }

        /// <summary>
        /// 独立控制动画的timeScale
        /// </summary>
        public float timeScale{
            get{
                return _timeScale;
            }set{
                if(value < 0 ){
                    value = 0;
                }
                _timeScale = value;
            }
        }

        /// <summary>
        /// 取消动画，会抛出LitCancelException
        /// </summary>
        public void Cancel(){
            _cancellationRequest = true;
            if(_manualTicker != null){
                _manualTicker.SetCanceled(LitCancelException.DEFAULT);
            }
        }

        public bool isCancellationRequest{
            get{
                return _cancellationRequest;
            }
        }

        private void ThrowIfCancellationRequested(){
            if(_cancellationRequest){
                LitCancelException.Throw();
            }   
        }
        internal void ThrowIfCancellationRequested(short tokenId){
            ThrowIfTokenIdChanged(tokenId);
            ThrowIfCancellationRequested();
        }

        public TweenOperationToken Token{
            get{
                return new TweenOperationToken(this);
            }
        }

        public static readonly TweenOperation GLOBAL = new TweenOperation();

    }



    public struct TweenOperationToken{

        private TweenOperation _operation;
        private short _tokenId;

        internal TweenOperationToken(TweenOperation operation){
            _operation = operation;
            _tokenId = operation.tokenId;
        }

        private TweenOperation operation{
            get{
                if(_operation == null){
                    _operation =  TweenOperation.GLOBAL;
                    _tokenId = _operation.tokenId;
                }
                return _operation;
            }
        }
        public LitTask<float> WaitTickAsync(bool ignoreTimeScale = false){
            return operation.WaitTickAsync(_tokenId,ignoreTimeScale);
        }

        public void ThrowIfCancellationRequested(){
            operation.ThrowIfCancellationRequested(_tokenId);
        }
    }
}
