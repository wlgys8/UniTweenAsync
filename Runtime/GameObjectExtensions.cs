using UnityEngine;
using MS.Async;

namespace MS.TweenAsync{
    public static class GameObjectExtensions
    {

//Move animation

        private struct MoveLerpContext{
            public Transform transform;
            public Vector3 fromPosition;
            public Vector3 toPosition;
            public SpaceType spaceType;

            public Transform customSpaceTransform;
        }


        private static OnLerp<MoveLerpContext> moveLerpFunc = (float lerp,MoveLerpContext ctx)=>{
            var transform = ctx.transform;
            var position = Vector3.LerpUnclamped(ctx.fromPosition,ctx.toPosition,lerp);
            switch(ctx.spaceType){
                case SpaceType.Local:
                case SpaceType.World:
                transform.localPosition = position;
                break;
                case SpaceType.Custom:
                transform.position = ctx.customSpaceTransform.TransformPoint(position);
                break;
            }
        };


        public static LitTask MoveToAsync(this GameObject gameObject,MoveToOptions options,TweenOperationToken operation = default){
            Vector3 fromPosition = Vector3.zero;
            var transform = gameObject.transform;
            var toPosition = options.position;
            switch(options.spaceType){
                case SpaceType.Local:
                case SpaceType.World:
                    fromPosition = transform.localPosition;
                    break;
                case SpaceType.Custom:
                    fromPosition = options.customSpaceTransform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
                    break;
            }
            if(options.spaceType == SpaceType.World){
                toPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(toPosition);
            }
            var tweenOptions = options.tweenOptions;
            var moveLerpContext = new MoveLerpContext(){
                transform = transform,
                fromPosition = fromPosition,
                toPosition = toPosition,
                spaceType = options.spaceType,
                customSpaceTransform = options.customSpaceTransform
            };
            return CoreUtils.RunLerpAsync(tweenOptions,moveLerpFunc,moveLerpContext,operation);
        }

        public static LitTask MoveByAsync(this GameObject gameObject,MoveByOptions options,TweenOperationToken operation = default){
            var transform = gameObject.transform;
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
            return MoveToAsync(gameObject, moveToOptions,operation);

        }
        
//Scale Animation

        private struct ScaleLerpContext{
            public Transform transform;
            public Vector3 fromScale;
            public Vector3 toScale;
        }



        private static OnLerp<ScaleLerpContext> scaleLerpFunc = (float lerp,ScaleLerpContext ctx)=>{
            var transform = ctx.transform;
            var scale = Vector3.LerpUnclamped(ctx.fromScale,ctx.toScale,lerp);
            transform.localScale = scale;
        };

        public static LitTask ScaleToAsync(this GameObject gameObject,ScaleToOptions options,TweenOperationToken operation = default){
            var transform = gameObject.transform;
            Vector3 fromScale = transform.localScale;
            var toScale = options.scale;
            var tweenOptions = options.tweenOptions;
            var scaleLerpContext = new ScaleLerpContext(){
                transform = transform,
                fromScale = fromScale,
                toScale = toScale
            };
            return CoreUtils.RunLerpAsync<ScaleLerpContext>(tweenOptions,scaleLerpFunc,scaleLerpContext,operation);
        }

//Rotate Animation
        private struct RotateLerpContext{
            public Transform transform;
            public Vector3 fromAngles;
            public Vector3 toAngles;
        }

        private static OnLerp<RotateLerpContext> rotateLerpFunc = (float lerp,RotateLerpContext ctx)=>{
            var transform = ctx.transform;
            var angles = Vector3.LerpUnclamped(ctx.fromAngles,ctx.toAngles,lerp);
            transform.localEulerAngles = angles;
        };


        public static LitTask RotateToAsync(this GameObject gameObject,RotateToOptions options,TweenOperationToken operation = default){
            var transform = gameObject.transform;
            var fromAngles = transform.localEulerAngles;
            var targetAngles = options.eulerAngles;
            RotateLerpContext rotateCtx = new RotateLerpContext(){
                transform = transform,
                fromAngles = fromAngles,
                toAngles = targetAngles,
            };
            return CoreUtils.RunLerpAsync<RotateLerpContext>(options.tweenOptions,rotateLerpFunc, rotateCtx,operation);
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
