using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{

    public delegate TweenOperation TweenFunction();

    public delegate TweenOperation TweenFunction<T>(T parameter);


    internal static class TweenDynamic{

        private struct State{
            public TweenFunction function;
            public TweenOperation operation;
        }


        static TweenDynamic(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){
            state.operation = state.function();
            state.operation.paused = true;
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.operation.normalizedTime = actionState.normalizedTime;
        }

        public static TweenOperation Create(TweenFunction function,float duration){
            var state = new State(){
                function = function
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(duration));
        }

    }
    internal static class TweenDynamic<T>{

        private struct State{
            public TweenFunction<T> function;
            public TweenOperation operation;
            public T parameter;
        }


        static TweenDynamic(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){
            state.operation = state.function(state.parameter);
            state.operation.paused = true;
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.operation.normalizedTime = actionState.normalizedTime;
        }

        public static TweenOperation Create(TweenFunction<T> function,T parameter,float duration){
            var state = new State(){
                function = function,
                parameter = parameter,
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(duration));
        }

    }

    public partial struct TweenOperation{

        public static TweenOperation Dynamic<T>(TweenFunction<T> function,T parameter,float duration){
            return TweenDynamic<T>.Create(function,parameter,duration);
        }

        public static TweenOperation Dynamic(TweenFunction function,float duration){
            return TweenDynamic.Create(function,duration);
        }
    }

}