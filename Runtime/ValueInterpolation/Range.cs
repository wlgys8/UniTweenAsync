
namespace MS.TweenAsync{

    public struct Range<TValue>{

        public TValue from;
        public TValue to;

        public Range(TValue from,TValue to){
            this.from = from;
            this.to = to;
        }

        public TweenOperation Property<TObject>(TObject target, PropertyAccesser<TObject,TValue> property,TweenOptions options){
            var context = new TargetProperty<TObject,TValue>(target,property);
            return this.Action(PropertyUpdateAction<TObject>.onUpdate,context,options);
        }

        public TweenOperation Property<TObject>(TObject target, PropertyAccesser<TObject,TValue> property,float duration){
            return this.Property(target,property,new TweenOptions(duration));
        }

        public TweenOperation Action<TContext>(InterpolationUpdate<TValue,TContext> onUpdate, TContext context,TweenOptions options){
            return UpdateController<TContext>.Prepare(from,to,onUpdate,context,options);
        }        

        private static class UpdateController<TContext>{
            private struct State{
                public TValue from;
                public TValue to;
                public TContext context;
                public InterpolationUpdate<TValue,TContext> onUpdate;
            }

            static UpdateController(){
                TweenAction<State>.RegisterStart(OnStart);
                TweenAction<State>.RegisterUpdate(OnUpdate);
            }

            private static void OnStart(ref State state){
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var value = ValueUtility.Lerp(state.from,state.to,actionState.interpolatedTime);
                state.onUpdate(value,state.context);
            }

            public static TweenOperation Prepare(TValue from, TValue to,InterpolationUpdate<TValue,TContext> onUpdate,TContext context,TweenOptions options){
                var state = new State(){
                    from = from,
                    to = to,
                    onUpdate = onUpdate,
                    context = context,
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }
        
        private static class PropertyUpdateAction<TObject>{
            public static InterpolationUpdate<TValue,TargetProperty<TObject,TValue>> onUpdate = (value,context)=>{
                context.property.Set(context.target,value);
            };
        }

    }


    public static partial class Values{

        public static Range<TValue> Range<TValue>(TValue from,TValue to){
            return new Range<TValue>(from,to);
        }

    }


}