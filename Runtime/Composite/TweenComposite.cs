using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{
    using CommonUtils;


    internal static class TweenSequence{

        private struct State{
            public List <TweenOperation> operations;
        }

        static TweenSequence(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
            TweenAction<State>.RegisterComplete(OnComplete);
            TweenAction<State>.RegisterPreRelease(OnPreRelease);
        }

        private static void OnStart(ref State state){
            for(var i = 0; i < state.operations.Count;i++){
                var op = state.operations[i];
                if(op.isInitialized && op.isCompleted){
                    state.operations[i] = op.Restart();
                }
            }
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
                    op.time = elapsedTime - opStartTime;
                    break;
                }else{
                    if(op.isCompleted){
                        continue;
                    }
                    op.RanToEnd();
                }
            }           
        }

        private static void OnComplete(ActionState actionState, ref State state){
            foreach(var op in state.operations){
                if(!op.isCompleted){
                    if(actionState.status == TweenStatus.Cancelled){
                        op.Cancel();
                    }else if(actionState.status == TweenStatus.Succeed){
                        op.RanToEnd();
                    }else{
                        Debug.LogWarningFormat("Unexpected action status:{0}",actionState.status);
                    }
                }
            }
        }

        private static void OnPreRelease(ActionState actionState, ref State state){
            foreach(var op in state.operations){
                op.Release();
            }
            ListPool<TweenOperation>.Release(state.operations);
        }

        public static TweenOperation Composite(params TweenOperation[] operations){
            var pOperations = ListPool<TweenOperation>.Request();
            pOperations.AddRange(operations);
            return CompositeInternal(pOperations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2,TweenOperation p3){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            operations.Add(p3);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2,TweenOperation p3,TweenOperation p4){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            operations.Add(p3);
            operations.Add(p4);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(List<TweenOperation> operations){
            var pOperations = ListPool<TweenOperation>.Request();
            pOperations.AddRange(operations);
            return CompositeInternal(pOperations);
        }

        /// <summary>
        /// operations必须是从pool中取的
        /// </summary>
        private static TweenOperation CompositeInternal(List<TweenOperation> operations){
            var duration = 0f;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                duration += op.duration;
                op.paused = true;
                op.autoRelease = false;
            }
            if(duration < 0){
                throw new System.InvalidOperationException("duration of tweens out of bounds");
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
            TweenAction<State>.RegisterComplete(OnComplete);
            TweenAction<State>.RegisterPreRelease(OnPreRelease);
        }

        private static void OnStart(ref State state){
            for(var i = 0; i < state.operations.Count;i++){
                var op = state.operations[i];
                if(op.isInitialized && op.isCompleted){
                    state.operations[i] = op.Restart();
                }
            }           
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

        private static void OnComplete(ActionState actionState, ref State state){
            foreach(var op in state.operations){
                if(!op.isCompleted){
                    if(actionState.status == TweenStatus.Succeed){
                        op.RanToEnd();
                    }else if(actionState.status == TweenStatus.Cancelled){
                        op.Cancel();
                    }else{
                        Debug.LogWarningFormat("Unexpected action status:{0}",actionState.status);
                    }
                }
            }
        }

        private static void OnPreRelease(ActionState actionState, ref State state){
            foreach(var op in state.operations){
                op.Release();
            }
            ListPool<TweenOperation>.Release(state.operations);
        }

        public static TweenOperation Composite(params TweenOperation[] operations){
            return CompositeInternal(ListPool<TweenOperation>.RequestAndFill(operations));
        }

        public static TweenOperation Composite(List<TweenOperation> operations){
            operations = ListPool<TweenOperation>.RequestAndFill(operations);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2,TweenOperation p3){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            operations.Add(p3);
            return CompositeInternal(operations);
        }

        public static TweenOperation Composite(TweenOperation p1,TweenOperation p2,TweenOperation p3,TweenOperation p4){
            var operations = ListPool<TweenOperation>.Request();
            operations.Add(p1);
            operations.Add(p2);
            operations.Add(p3);
            operations.Add(p4);
            return CompositeInternal(operations);
        }

        private static TweenOperation CompositeInternal(List<TweenOperation> operations){
            var duration = 0f;
            for(var i = 0; i < operations.Count;i++){
                var op = operations[i];
                duration = Mathf.Max(duration,op.duration);
                op.paused = true;
                op.autoRelease = false;
            }
            var state = new State(){
                operations = operations
            };
            var tweenOptions = new TweenOptions(duration);
            return TweenAction<State>.Prepare(state,tweenOptions);
        }
    }

    internal static class TweenRepeat{

        private struct State{
            public TweenOperation source;
            public int repeatCount;
            public int repeatIndex;

        }


        static TweenRepeat(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
            TweenAction<State>.RegisterComplete(OnCompleted);
            TweenAction<State>.RegisterPreRelease(OnPreRelease);
        }

        private static void OnStart(ref State state){
            state.repeatIndex = 0;
            if(state.source.isInitialized && state.source.isCompleted){
                state.source = state.source.Restart();
            }
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.source.time = actionState.elapsedTime - state.repeatIndex * state.source.duration;
            if(state.source.isCompleted){
                var overflowSeconds = actionState.elapsedTime - state.repeatIndex * state.source.duration;
                var overflowRepeat = Mathf.Clamp(Mathf.FloorToInt(overflowSeconds / state.source.duration),1,state.repeatCount);
                state.repeatIndex += overflowRepeat;
                if(state.repeatIndex < state.repeatCount){
                    var extraSeconds = overflowSeconds % state.source.duration;
                    state.source = state.source.Restart();
                    state.source.time = extraSeconds;
                }
            }
        }

        private static void OnCompleted(ActionState actionState,ref State state){
            if(!state.source.isCompleted){
                if(actionState.status == TweenStatus.Cancelled){
                    state.source.Cancel();
                }else if(actionState.status == TweenStatus.Succeed){
                    state.source.RanToEnd();
                }else{
                    Debug.LogWarningFormat("Unexpected tween status:{0}",actionState.status);
                }
            }
        }

        private static void OnPreRelease(ActionState actionState,ref State state){
            state.source.Release();
        }


        
        public static TweenOperation Create(TweenOperation source,int repeatCount){
            source.autoRelease = false;
            source.paused = true;
            var state = new State(){
                source = source,
                repeatCount = repeatCount
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(repeatCount * source.duration));
        }
    }


    internal static class TweenRepeatForever{

        private struct State{
            public TweenOperation operation;
            public int repeatIndex;
        }

        static TweenRepeatForever(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
            TweenAction<State>.RegisterComplete(OnCompleted);
            TweenAction<State>.RegisterPreRelease(OnPreRelease);
        }

        private static void OnStart(ref State state){
            state.repeatIndex = 0;
            if(state.operation.isInitialized && state.operation.isCompleted){
                state.operation = state.operation.Restart();
            }
        }

        private static void OnUpdate(ActionState actionState,ref State state){
            state.operation.time = actionState.elapsedTime - state.repeatIndex * state.operation.duration;
            if(!state.operation.isCompleted){
                return;
            }
            //To avoid too many loops here, we jump to the last loop immediately.
            var overflowSeconds = actionState.elapsedTime - state.repeatIndex * state.operation.duration;
            var overflowRepeat = Mathf.FloorToInt(overflowSeconds / state.operation.duration);
            var extraSeconds = overflowSeconds % state.operation.duration;
            state.repeatIndex = Mathf.Clamp(state.repeatIndex + overflowRepeat,0,int.MaxValue);
            state.operation = state.operation.Restart();
            state.operation.time = extraSeconds;
        }

        private static void OnCompleted(ActionState actionState,ref State state){
            if(!state.operation.isCompleted){
                if(actionState.status == TweenStatus.Cancelled){
                    state.operation.Cancel();
                }else if(actionState.status == TweenStatus.Succeed){
                    state.operation.RanToEnd();
                }else{
                    Debug.LogWarningFormat("Unexpected tween status:{0}",actionState.status);
                }
            }
        }

        private static void OnPreRelease(ActionState actionState,ref State state){
            state.operation.Release();
        }


        public static TweenOperation Create(TweenOperation source){
            if(source.duration <= 0){
                throw new System.ArgumentException("The duration of the tween must be positive.");
            }
            source.autoRelease = false;
            source.paused = true;
            var state = new State(){
                operation = source
            };
            return TweenAction<State>.Prepare(state,new TweenOptions(float.MaxValue));
        }
    }

    [System.Obsolete("Use TweenOperation's static methods instead")]
    public static class TweenComposite
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

        public static TweenOperation Repeat(this TweenOperation operation,int repeatCount){
            return TweenRepeat.Create(operation,repeatCount);
        }

        public static TweenOperation RepeatForever(this TweenOperation operation){
            return TweenRepeatForever.Create(operation);
        }

    }


    public partial struct TweenOperation{

        /// <summary>
        /// Run tweens one by one
        /// </summary>
        public static TweenOperation Sequence(params TweenOperation[] operations){
            return TweenSequence.Composite(operations);
        }

        /// <summary>
        /// Run tweens one by one
        /// </summary>
        public static TweenOperation Sequence(List<TweenOperation> operations){
            return TweenSequence.Composite(operations);
        }

        public static TweenOperation Sequence(TweenOperation p1,TweenOperation p2){
            return TweenSequence.Composite(p1,p2);
        }

        public static TweenOperation Sequence(TweenOperation p1,TweenOperation p2,TweenOperation p3){
            return TweenSequence.Composite(p1,p2,p3);
        }

        public static TweenOperation Sequence(TweenOperation p1,TweenOperation p2,TweenOperation p3,TweenOperation p4){
            return TweenSequence.Composite(p1,p2,p3,p4);
        }

        /// <summary>
        /// Run tweens parallel
        /// </summary>
        public static TweenOperation Parallel(params TweenOperation[] operations){
            return TweenParallel.Composite(operations);
        }

        /// <summary>
        /// Run tweens parallel
        /// </summary>
        public static TweenOperation Parallel(List<TweenOperation> operations){
            return TweenParallel.Composite(operations);
        }

        public static TweenOperation Parallel(TweenOperation p1,TweenOperation p2){
            return TweenParallel.Composite(p1,p2);
        }

        public static TweenOperation Parallel(TweenOperation p1,TweenOperation p2,TweenOperation p3){
            return TweenParallel.Composite(p1,p2,p3);
        }

        public static TweenOperation Parallel(TweenOperation p1,TweenOperation p2,TweenOperation p3,TweenOperation p4){
            return TweenParallel.Composite(p1,p2,p3,p4);
        }

        /// <summary>
        /// Repeat the tween in specific times.
        /// </summary>
        public static TweenOperation Repeat(TweenOperation operation,int repeatCount){
            return TweenRepeat.Create(operation,repeatCount);
        }

        /// <summary>
        /// Repeat the tween forever
        /// </summary>
        public static TweenOperation RepeatForever(TweenOperation operation){
            return TweenRepeatForever.Create(operation);
        }
    }


}
