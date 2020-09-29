
namespace MS.TweenAsync{
    public struct From<TValue>
    {

        public TValue value{
            get;private set;
        }

        public From(TValue value){
            this.value = value;
        }

        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property,TweenOptions options){
            return PropertyController<TObject>.Prepare(target,property,value,options);
        }

        private static class PropertyController<TObject>{
            private struct State{

                public TValue from;
                public TValue to;

                public TObject target;
                public PropertyAccesser<TObject,TValue> property;
            }

            static PropertyController(){
                TweenAction<State>.RegisterStart(OnStart);
                TweenAction<State>.RegisterUpdate(OnUpdate);
            }

            private static void OnStart(ref State state){
                state.to = state.property.Get(state.target);
                var value = ValueUtility.Lerp(state.from,state.to,0);
                state.property.Set(state.target,value);
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var value = ValueUtility.Lerp(state.from,state.to,actionState.interpolatedTime);
                state.property.Set(state.target,value);
            }

            public static TweenOperation Prepare(TObject target,PropertyAccesser<TObject,TValue> property, TValue from,TweenOptions options){
                var state = new State(){
                    from = from,
                    target = target,
                    property = property
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }
    }

    public static partial class Values{

        public static From<TValue> From<TValue>(TValue from){
            return new From<TValue>(from);
        }

    }
}
