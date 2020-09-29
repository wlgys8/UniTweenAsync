using System.Collections.Generic;

namespace MS.TweenAsync{
    public class Sequence<TValue>
    {
        private List<TValue> _values;

        public Sequence(TValue[] values){
            this.Validate();
            _values = new List<TValue>(values);
        }

        public Sequence(List<TValue> values){
            this.Validate();
            _values = new List<TValue>(values);
        }

        private void Validate(){
            if(!ValueUtility.IsSupportDistance<TValue>()){
                throw new System.NotImplementedException($"distance operation must be implemented for type {typeof(TValue)}");
            }
        }

        public TweenOperation Property<TObject>(TObject target,IPropertyAccesser<TObject,TValue> property,TweenOptions options){
            return PropertyController.Prepare(target,property,_values,options);
        } 

        public TweenOperation Property<TObject>(TObject target,IPropertyAccesser<TObject,TValue> property,float duration){
            return Property(target,property,new TweenOptions(duration));
        } 

        public TweenOperation Property(object target,IPropertyAccesser<TValue> property,TweenOptions options){
            return PropertyController.Prepare(target,property,_values,options);
        } 

        private static class PropertyController{

            private struct State{

                public List<TValue> values;
                public int seqIndex;

                public bool PreSeq(){
                    if(seqIndex == 0){
                        return false;
                    }
                    seqIndex --;
                    seqEndDis = seqStartDis;
                    seqStartDis = seqEndDis - ValueUtility.Distance(seqFrom,seqTo);
                    return true;
                }

                public bool NextSeq(){
                    if(seqIndex >= values.Count - 2){
                        return false;
                    }
                    seqIndex ++;
                    seqStartDis = seqEndDis;
                    seqEndDis = seqStartDis + ValueUtility.Distance(seqFrom,seqTo);
                    return true;
                }

                public TValue seqFrom{
                    get{
                        return values[seqIndex];
                    }
                }
                public TValue seqTo{
                    get{
                        return values[seqIndex + 1];
                    }
                }

                public float seqStartDis;
                public float seqEndDis;
                public float totalDis;
                public object target;
                public IPropertyAccesser<TValue> property;
            }

            static PropertyController(){
                TweenAction<State>.RegisterStart(OnStart);
                TweenAction<State>.RegisterUpdate(OnUpdate);
            }

            private static void OnStart(ref State state){
                state.seqIndex = 0;
                state.seqStartDis = 0;
                state.seqEndDis = state.seqStartDis + ValueUtility.Distance(state.seqFrom,state.seqTo);
                var totalDis = 0f;
                for(var index = 0; index < state.values.Count-1;index++){
                    totalDis += ValueUtility.Distance(state.values[index],state.values[index + 1]);
                }
                state.totalDis = totalDis;
            }

            private static void OnUpdate(ActionState actionState,ref State state){
                var targetDis = state.totalDis * actionState.interpolatedTime;
                while(targetDis > state.seqEndDis){
                    if(!state.NextSeq()){
                        break;
                    }
                }
                while(targetDis < state.seqStartDis){
                    if(!state.PreSeq()){
                        break;
                    }
                }
                var t = (targetDis - state.seqStartDis) / (state.seqEndDis - state.seqStartDis);
                var value = ValueUtility.Lerp(state.seqFrom,state.seqTo,t);
                state.property.UnsafeSet(state.target,value);
            }

            public static TweenOperation Prepare(object target,IPropertyAccesser<TValue> property, List<TValue> values,TweenOptions options){
                var state = new State(){
                    values = values,
                    target = target,
                    property = property
                };
                return TweenAction<State>.Prepare(state,options);          
            }
        }
    }

    public static partial class Values{

        public static Sequence<TValue> Sequence<TValue>(params TValue[] values){
            return new Sequence<TValue>(values);
        }

        public static Sequence<TValue> Sequence<TValue>(List<TValue> values){
            return new Sequence<TValue>(values);
        }
    }
}