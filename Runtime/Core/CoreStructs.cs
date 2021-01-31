
namespace MS.TweenAsync{

    public struct TweenOptions{
        public float duration;
        private EaseFunction _ease;
        public bool ignoreTimeScale;

        public TweenOptions(float duration):this(duration,EaseFuncs.Linear){
        }

        public TweenOptions(float duration,EaseFunction ease){
            this.duration = duration;
            _ease = ease;
            ignoreTimeScale = false;
        }

        public EaseFunction ease{
            get{
                if(_ease == null){
                    _ease = EaseFuncs.Linear;
                }
                return _ease;
            }set{
                _ease = value;
            }
        }
    }

    public interface IPropertyAccesser{}

    public interface IPropertyAccesser<TValue>:IPropertyAccesser{

        TValue UnsafeGet(object target);

        void UnsafeSet(object target,TValue value);

    }

    public interface IPropertyAccesser<TObject,TValue>:IPropertyAccesser<TValue>{

        TValue Get(TObject target);

        void Set(TObject target,TValue value);

    }


    public class PropertyAccesser<TObject,TValue>:IPropertyAccesser<TObject,TValue>{

        public delegate TValue Getter(TObject target);

        public delegate void Setter(TObject target,TValue value);

        private readonly Getter _getter;

        private readonly Setter _setter;

        public PropertyAccesser(Getter getter,Setter setter){
            this._getter = getter;
            this._setter = setter;
        }

        public TValue Get(TObject target){
            return _getter(target);
        }

        public void Set(TObject target,TValue value){
            _setter(target,value);
        }

        /// <summary>
        /// 如果target类型不符合，会抛错
        /// </summary>
        public TValue UnsafeGet(object target)
        {
            return Get((TObject)target);
        }

        /// <summary>
        /// 如果target类型不符合，会抛错
        /// </summary>
        public void UnsafeSet(object target, TValue value)
        {
            Set((TObject)target,value);
        }
    }

    public struct TargetProperty<TObject,TValue>{

        public TObject target;
        public PropertyAccesser<TObject,TValue> property;

        public TargetProperty(TObject target,PropertyAccesser<TObject,TValue> property){
            this.target = target;
            this.property = property;
        }

        public void Set(TValue value){
            this.property.Set(target,value);
        }

        public TValue Get(){
            return this.property.Get(target);
        }
    }
    
    public delegate TObject DynamicTargetGetter<TObject>();

    public delegate TObject DynamicTargetGetter<TObject,T1>(T1 p1);

    public delegate void InterpolationUpdate<TValue,TContext>(TValue value,TContext context);







}
