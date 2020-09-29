using UnityEngine;
using MS.Async;

namespace MS.TweenAsync{


    public static class MoveExtensions{

        private struct State{

            public Vector3 fromPosition;

            public Vector3 toPosition;

            public MoveToOptions options;

            public Transform target;
        }

   
        static MoveExtensions(){
            TweenAction<State>.RegisterStart(OnStart);
            TweenAction<State>.RegisterUpdate(OnUpdate);
        }

        private static void OnStart(ref State state){
            var options = state.options;
            var transform = state.target;
            switch(options.spaceType){
                case SpaceType.Local:
                case SpaceType.World:
                    state.fromPosition = transform.localPosition;
                    break;
                case SpaceType.Custom:
                    state.fromPosition = options.customSpaceTransform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
                    break;
            }
            state.toPosition = options.position;
            if(options.spaceType == SpaceType.World && transform.parent){
                state.toPosition = transform.parent.worldToLocalMatrix.MultiplyPoint3x4(options.position);
            }
        }

        private static void OnUpdate(ActionState actionState, ref State state){
            var transform = state.target;
            var localPosition = Vector3.LerpUnclamped(state.fromPosition,state.toPosition,actionState.interpolatedTime);
            switch(state.options.spaceType){
                case SpaceType.Local:
                case SpaceType.World:
                transform.localPosition = localPosition;
                break;
                case SpaceType.Custom:
                transform.position = state.options.customSpaceTransform.TransformPoint(localPosition);
                break;
            }
        }

        public static TweenOperation MoveToAsync(this Transform transform,MoveToOptions options){
            var state = new State(){
                options = options,
                target = transform,
            };
            return TweenAction<State>.Prepare(state,options.tweenOptions);
        }

        public static TweenOperation MoveToAsync(this GameObject gameObject,MoveToOptions options){
            return MoveToAsync(gameObject.transform,options);
        }

        public static TweenOperation MoveToAsync(this Transform transform,Vector3 localPosition,float duration = 1){
            return transform.MoveToAsync(new MoveToOptions(localPosition,duration));
        }

        public static TweenOperation MoveByAsync(this Transform transform, MoveByOptions options){
            var offset = options.offset;
            var toPosition = default(Vector3);
            switch(options.spaceType){
                case SpaceType.Local:
                    toPosition = transform.localPosition + offset;
                    break;
                case SpaceType.World:
                    toPosition = transform.position + offset;
                    break;
                case SpaceType.Custom:
                    toPosition = options.customSpaceTransform.worldToLocalMatrix.MultiplyPoint3x4(transform.position) + offset;
                    break;
            }
            var moveToOptions = new MoveToOptions(toPosition);
            moveToOptions.tweenOptions = options.tweenOptions;
            moveToOptions.spaceType = options.spaceType;
            moveToOptions.customSpaceTransform = options.customSpaceTransform;
            return MoveToAsync(transform, moveToOptions);
        }

        public static TweenOperation MoveByAsync(this GameObject gameObject, MoveByOptions options){
            return gameObject.transform.MoveByAsync(options);
        }
    }


    public static class ScaleExtensions{
        public static TweenOperation ScaleToAsync(this GameObject gameObject,ScaleToOptions options){
            return new To<Vector3>(options.scale).Property(gameObject.transform,Properties.transform.localScale,options.tweenOptions);
        }
    }


    public static class RotateExtensions{
        public static TweenOperation RotateToAsync(this Transform transform,RotateToOptions options){
            return new To<Vector3>(options.eulerAngles).Property(transform,
            Properties.transform.localEulerAngles,
            options.tweenOptions);
        }

        public static TweenOperation RotateToAsync(this GameObject gameObject,RotateToOptions options){
            return RotateToAsync(gameObject.transform,options);
        }
    }


    public struct MoveToOptions{

        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// default spaceType is Local
        /// </summary>
        public SpaceType spaceType;

        /// <summary>
        /// Use for custom spaceType
        /// </summary>
        public Transform customSpaceTransform;
    
        public TweenOptions tweenOptions;

        public MoveToOptions(Vector3 position,float duration = 1){
            this.position = position;
            this.spaceType = SpaceType.Local;
            this.customSpaceTransform = null;
            this.tweenOptions = new TweenOptions(duration);
        }
    }

    public struct MoveByOptions{
        public Vector3 offset;
        public SpaceType spaceType;
        public Transform customSpaceTransform;
        public TweenOptions tweenOptions;    
        public MoveByOptions(Vector3 offset,float duration = 1){
            this.offset = offset;
            this.spaceType = SpaceType.Local;
            this.customSpaceTransform = null;
            this.tweenOptions = new TweenOptions(duration);
        }       
    }

    public struct ScaleToOptions{
       
        public Vector3 scale;
        public TweenOptions tweenOptions;
        public ScaleToOptions(Vector3 scale,float duration = 1){
            this.scale = scale;
            this.tweenOptions = new TweenOptions(duration);
        }
    }


    public struct RotateToOptions{
        public Vector3 eulerAngles;
        public TweenOptions tweenOptions;
        public RotateToOptions(Vector3 eulerAngles,float duration = 1){
            this.eulerAngles = eulerAngles;
            this.tweenOptions = new TweenOptions(duration);
        }
    }

    public enum SpaceType{
        Local,
        World,
        Custom,
    }


}
