using UnityEngine;

namespace MS.TweenAsync{


    public delegate float EaseFunction(float t);
    
    public static class EaseFuncs
    {

        public static readonly EaseFunction Linear = (float t)=>{
            return t;
        };

        public static readonly EaseFunction InOutLinear = (float t)=>{
            t *= 2;
            if (t < 1) {
                return t;
            } else {
                return 2-t;
            }
        };

        public static readonly EaseFunction InQuad = (float t)=>{
            return t*t;
        };

        public static readonly EaseFunction InBack = (float t)=>
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        };

        public static readonly EaseFunction OutBack = (float t)=>
        {
            float s = 1.70158f;
            t = t - 1;
            return (t * t * ((s + 1) * t + s) + 1);
        };

        public static readonly EaseFunction OutQuad = (float t)=>{
            return -t*(t-2);
        };
        
        public static readonly EaseFunction InOutQuad = (float t)=>{
            t*=2;
            if(t<1){
                return 0.5f*t*t;
            }
            t--;
            return -0.5f*(t*(t-2)-1);
        };
        
        public static readonly EaseFunction InCubic = (float t)=>{
            return t*t*t;
        };
        
        public static readonly EaseFunction OutCubic = (float t)=>{
            t--;
            return t*t*t+1;
        };
        
        public static readonly EaseFunction InOutCubic = (float t)=>{
            t*=2;
            if(t<1){
                return 0.5f*t*t*t*t;
            }
            t-=2;
            return -0.5f*(t*t*t*t-2);
        };
        
        public static readonly EaseFunction InElastic = (float t)=>{
            if(t==0||t==1){
                return t;
            }
            float p=0.3f;
            return -Mathf.Pow(2,10*(t-=1))*Mathf.Sin(t*2*Mathf.PI/p);
        };
        
        public static readonly EaseFunction OutElastic = (float t)=>{
            float b=0;
            float c=1;
            float d=1;
            float a=0;
            float p=0;
            float s=0;
            if (t==0) return b;  if ((t/=d)==1) return b+c;  if (p==0) p=d*.3f;
            if (a < Mathf.Abs(c)) {
                a=c;  s=p/4; }
            else {
                s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
            }
            return a*Mathf.Pow(2,-10*t) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p ) + c + b;
        };

        public static readonly EaseFunction Shake = (t)=>{
            if(t==0){
                return 0;
            }
            return Mathf.Sin(t*2*Mathf.PI*5)*(1-t);
        };
    }
}
