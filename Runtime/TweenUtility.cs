using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{
    using System;

    public static class TweenUtility{

        private struct DelayState{}

        static TweenUtility(){

        }

        public static TweenOperation Delay(float seconds){
            var state = new DelayState();
            return TweenAction<DelayState>.Prepare(state,new TweenOptions(seconds));           
        }


        public static TweenOperation Callback<T>(Action<T> callback,T parameter){
            return CallbacksFactory<T>.Create(callback,parameter);
        }

        public static TweenOperation Callback(Action callback){
            return CallbacksFactory.Create(callback);
        }
    }


    internal static class CallbacksFactory{
        private struct State{
            public Action callback;
        }  

        static CallbacksFactory(){
            TweenAction<State>.RegisterStart(OnStart);
        }     

        private static void OnStart(ref State state){
            state.callback();
        }
        public static TweenOperation Create(Action callback){
            var state = new State(){
                callback = callback,
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(0));
        }
    }
    internal static class CallbacksFactory<T>{

        private struct State{
            public Action<T> callback;
            public T parameter;
        }

        static CallbacksFactory(){
            TweenAction<State>.RegisterStart(OnStart);
        }

        private static void OnStart(ref State state){
            state.callback(state.parameter);
        }

        public static TweenOperation Create(Action<T> callback,T parameter){
            var state = new State(){
                callback = callback,
                parameter = parameter
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(0));
        }
    }

}
