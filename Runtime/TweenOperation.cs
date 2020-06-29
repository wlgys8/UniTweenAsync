
namespace MS.Async{
    public class TweenOperation
    {
        private bool _cancellationRequest = false;
        private float _timeScale = 1;

        public TweenOperation(){
            this.Reset();
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset(){
            paused = false;
            _cancellationRequest = false;
            _timeScale = 1;
        }

        /// <summary>
        /// 暂停动画
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
           
        }

        public void ThrowIfCancellationRequested(){
            if(_cancellationRequest){
                LitCancelException.Throw();
            }
        }


        internal static readonly TweenOperation DEFAULT = new TweenOperation();

    }
}
