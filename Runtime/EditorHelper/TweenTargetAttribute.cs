using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MS.TweenAsync{



    [AttributeUsage(AttributeTargets.Class)]
    public class TweenTargetAttribute : System.Attribute
    {

        private System.Type _targetType;
        public TweenTargetAttribute(System.Type type){
            _targetType = type;
        }

        public System.Type targetType{
            get{
                return _targetType;
            }
        }
    }



}
