using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{
    public struct By<TValue>
    {
        public TValue delta;

        public By(TValue delta){
            this.delta = delta;
        }

        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property,TweenOptions options){
            return PropertyController.Prepare(target,property,delta,options);
        }

        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property){
            return PropertyController.Prepare(target,property,delta,new TweenOptions(1));
        }

        public TweenOperation Property<TObject>(TObject target,PropertyAccesser<TObject,TValue> property,float duration){
            return PropertyController.Prepare(target,property,delta,new TweenOptions(duration));
        }

        public TweenOperation Property(object target,IPropertyAccesser<TValue> property,TweenOptions options){
            return PropertyController.Prepare(target,property,delta,options);
        }
   

        private static class PropertyController{

            private struct State{

                public TValue from;
                public TValue to;

                public TValue delta;

                public object target;
                public IPropertyAccesser<TValue> property;
            }

            static PropertyController(){
                TweenAction<State>.RegisterStart(OnStart);
                TweenAction<State>.RegisterUpdate(OnUpdate);
            }

            private static void OnStart(ref State state){
                state.from = state.property.UnsafeGet(state.target);
                state.to = ValueUtility.Add(state.from,state.delta);
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var value = ValueUtility.Lerp(state.from,state.to,actionState.interpolatedTime);
                state.property.UnsafeSet(state.target,value);
            }

            public static TweenOperation Prepare(object target,IPropertyAccesser<TValue> property, TValue delta,TweenOptions options){
                var state = new State(){
                    delta = delta,
                    target = target,
                    property = property
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }
    }

    public static partial class Values{

        public static By<TValue> By<TValue>(TValue delta){
            return new By<TValue>(delta);
        }

    }
}
