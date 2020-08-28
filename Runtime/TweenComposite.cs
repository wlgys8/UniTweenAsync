using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{



    internal static class TweenSequence{

        private struct State{
            public List <TweenOperation> operations;
        }

        static TweenSequence(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){

        }

        private static void OnUpdate(ActionState actionState, ref State state){
            var elapsedTime = actionState.elapsedTime;
            if(elapsedTime < 0){
                return;
            }
            var operations = state.operations;
            var opEndTime = 0f;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                var opStartTime = opEndTime;
                opEndTime += op.duration;
                if(elapsedTime < opEndTime){
                    var opNormalizedTime = (elapsedTime - opStartTime) / op.duration;
                    op.normalizedTime = opNormalizedTime;
                    break;
                }else{
                    if(op.isCompleted){
                        continue;
                    }
                    op.RanToEnd();
                }
            }           
        }

        public static TweenOperation Composite(params TweenOperation[] operations){
            return Composite(new List<TweenOperation>(operations));
        }

        public static TweenOperation Composite(List<TweenOperation> operations){
            var duration = 0f;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                duration += op.duration;
                op.paused = true;
            }
            var state = new State(){
                operations = operations,
            };
            var tweenOptions = new TweenOptions(duration);
            return TweenAction<State>.Prepare(state,tweenOptions);
        }
    }


    internal class TweenParallel{
        private struct State{
            public List<TweenOperation> operations;
        }

        static TweenParallel(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){

        }

        private static void OnUpdate(ActionState actionState, ref State state){
            var elapsedTime = actionState.elapsedTime;
            if(elapsedTime < 0){
                return;
            }
            var operations = state.operations;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                if(op.isCompleted){
                    continue;
                }
                op.time = elapsedTime;
            }           
        }
        public static TweenOperation Composite(params TweenOperation[] operations){
            return Composite(new List<TweenOperation>(operations));
        }

        public static TweenOperation Composite(List<TweenOperation> operations){
            var duration = 0f;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                duration = Mathf.Max(duration,op.duration);
                op.paused = true;
            }
            var state = new State(){
                operations = operations
            };
            var tweenOptions = new TweenOptions(duration);
            return TweenAction<State>.Prepare(state,tweenOptions);
        }
    }

    public class TweenComposite
    {
        
        public static TweenOperation Sequence(params TweenOperation[] operations){
            return TweenSequence.Composite(operations);
        }
        public static TweenOperation Sequence(List<TweenOperation> operations){
            return TweenSequence.Composite(operations);
        }

        public static TweenOperation Parallel(params TweenOperation[] operations){
            return TweenParallel.Composite(operations);
        }
        public static TweenOperation Parallel(List<TweenOperation> operations){
            return TweenParallel.Composite(operations);
        }
    }
}
