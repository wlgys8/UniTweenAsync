using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{



    internal static class TweenSequence{

        private struct State{

            public float duration;
            public TweenOperation[] operations;
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
            for(var i = 0; i < operations.Length;i++){
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
            var duration = 0f;
            for(var i = 0; i < operations.Length;i++){
                var op = operations[i];
                duration += op.duration;
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
    }
}
