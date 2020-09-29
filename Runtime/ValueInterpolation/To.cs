

namespace MS.TweenAsync{

    public struct To<TValue>{

        public TValue value{
            get;private set;
        }

        public To(TValue to){
            this.value = to;
        }
        
        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property,TweenOptions options){
            return PropertyController<TObject>.Prepare(target,property,value,options);
        }

        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property){
            return Property<TObject>(target,property,1);
        }
        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property,float duration){
            return Property<TObject>(target,property,new TweenOptions(duration));
        }



        public TweenOperation Property(object target,IPropertyAccesser<TValue> property,TweenOptions options){
            return PropertyController.Prepare(target,property,value,options);
        }


        private static class PropertyController{

            private struct State{

                public TValue from;
                public TValue to;

                public object target;
                public IPropertyAccesser<TValue> property;
            }

            static PropertyController(){
                TweenAction<State>.RegisterStart(OnStart);
                TweenAction<State>.RegisterUpdate(OnUpdate);
            }

            private static void OnStart(ref State state){
                state.from = state.property.UnsafeGet(state.target);
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var value = ValueUtility.Lerp(state.from,state.to,actionState.interpolatedTime);
                state.property.UnsafeSet(state.target,value);
            }

            public static TweenOperation Prepare(object target,IPropertyAccesser<TValue> property, TValue to,TweenOptions options){
                var state = new State(){
                    to = to,
                    target = target,
                    property = property
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }

        private static class PropertyController<TObject>{
            private class State{

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
                state.from = state.property.Get(state.target);
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var value = ValueUtility.Lerp(state.from,state.to,actionState.interpolatedTime);
                state.property.Set(state.target,value);
            }

            public static TweenOperation Prepare(TObject target,PropertyAccesser<TObject,TValue> property, TValue to,TweenOptions options){
                var state = new State(){
                    to = to,
                    target = target,
                    property = property
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }
    }

    public static partial class Values{

        public static To<TValue> To<TValue>(TValue value){
            return new To<TValue>(value);
        }
    }

}

