﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MS.Async;

namespace MS.TweenAsync{
    internal class TweenBehaviourBinding:MonoBehaviour
    {
        private List<OperationItem> _enableBehaviours = new List<OperationItem>();
        private List<OperationItem> _disableBehaviours = new List<OperationItem>();
        private List<OperationItem> _destroyBehaviours = new List<OperationItem>();

        private bool _processingEnable = false;
        private bool _processingDisable = false;

        private int _lastRemoveCheckFrame = 0;
        
        public void Bind(TweenOperation operation,BehaviourBindSetting setting){
            RemoveAllCompletedOperations();
            if(setting.behaviourOnEnable != BehaviourType.None){
                var item = new OperationItem(){
                    behaviour = setting.behaviourOnEnable,
                    operation = operation
                };
                if(_processingEnable){
                    AddItemToList(ref item,_enableBehaviours);
                }else{
                    var completed = false;
                    if(this.isActiveAndEnabled){
                        //if current is enabled, do behaviour immediately
                        completed = ProcessOperationItem(ref item);
                    }
                    if(!completed){
                        AddItemToList(ref item,_enableBehaviours);
                    }
                }
            }

            if(setting.behaviourOnDisable != BehaviourType.None){
                var item = new OperationItem(){
                    behaviour = setting.behaviourOnDisable,
                    operation = operation
                };
                if(_processingDisable){
                    AddItemToList(ref item,_disableBehaviours);
                }else{
                    var completed = false;
                    if(!this.isActiveAndEnabled){
                        //if current is disabled, do behaviour immediately.
                        completed = ProcessOperationItem(ref item);
                    }
                    if(!completed){
                        AddItemToList(ref item,_disableBehaviours);
                    }
                }
            }

            if(setting.behaviourOnDestroy != BehaviourType.None){
                var item = new OperationItem(){
                    behaviour = setting.behaviourOnDestroy,
                    operation = operation
                };
                if(this == null){
                    //already destroyed
                    ProcessOperationItem(ref item);
                }else{
                    AddItemToList(ref item,_destroyBehaviours);
                }
            }
        }

        /// <summary>
        /// 移除所有已经为完成状态的operations
        /// </summary>
        private void RemoveAllCompletedOperations(){
            if(_lastRemoveCheckFrame != Time.frameCount){
                _lastRemoveCheckFrame = Time.frameCount;
                RemoveCompletedOperationsInList(_enableBehaviours);
                RemoveCompletedOperationsInList(_disableBehaviours);
                RemoveCompletedOperationsInList(_destroyBehaviours);
            }
        }

        private void RemoveCompletedOperationsInList(List<OperationItem> list){
            for(var i = list.Count - 1; i >= 0;i --){
                var item = list[i];
                if(item.operation.isCompleted){
                    list.RemoveAt(i);
                }
            }
        }

        private void AddItemToList(ref OperationItem item,List<OperationItem> list){
            list.Add(item);
        }

        private bool ProcessOperationItem(ref OperationItem item){
            bool isCompleted = false;
            switch(item.behaviour){
                case BehaviourType.RanToEnd:
                item.operation.RanToEnd();
                isCompleted = true;
                break;
                case BehaviourType.Cancel:
                item.operation.Cancel();
                isCompleted = true;
                break;
                case BehaviourType.Resume:
                item.operation.paused = false;
                break;
                case BehaviourType.Pause:
                item.operation.paused = true;
                break;
            }    
            return isCompleted;    
        }

        private void ProcessOperationList(List<OperationItem> items){
            var index = 0;
            while(index < items.Count){
                var item = items[index];
                bool shouldRemove = ProcessOperationItem(ref item);
                if(shouldRemove){
                    items.RemoveAt(index);
                }else{
                    index ++;
                }
            }        
        }

        private void CancelAndClearAllOperationsInList(List<OperationItem> items){
            foreach(var item in items){
                if(!item.operation.isCompleted){
                    item.operation.Cancel();
                }
            }
            items.Clear();
        }

        internal void CancelAll(){
            CancelAndClearAllOperationsInList(_enableBehaviours);
            CancelAndClearAllOperationsInList(_disableBehaviours);
            CancelAndClearAllOperationsInList(_destroyBehaviours);
        }

        void OnEnable(){
            _processingEnable = true;
            ProcessOperationList(_enableBehaviours);
            _processingEnable = false;
        }

        void OnDisable(){
            _processingDisable = true;
            ProcessOperationList(_disableBehaviours);
            _processingDisable = false;
        }

        void OnDestroy(){
            ProcessOperationList(_destroyBehaviours);
        }


        private struct OperationItem{
            public TweenOperation operation;
            public BehaviourType behaviour;
        }
    }

    public static class TweenBehaviourBindingExts{

        /// <summary>
        /// 将TweenOperation与目标gameObject的生命周期进行绑定, 通过BehaviourBindSetting设置在不同的回调函数执行不同的行为。
        /// <para>注意: 当一个go首次被绑定时，会有创建MonoBehaviour的开销。所以不宜大量频繁的往不同的gameObject进行Bind操作。</para>
        /// <para>通常来说，一个比较好的方式是：在一个Scene场景创建一个通用的gameObject作为绑定目标，来处理这个场景里所有动画。例如，场景关闭时，暂停所有Tween动画，场景销毁时，销毁所有动画</para>
        /// </summary>
        public static TweenOperation Bind(this TweenOperation operation,GameObject go,BehaviourBindSetting setting){
            var b = go.GetComponent<TweenBehaviourBinding>();
            if(!b){
                b = go.AddComponent<TweenBehaviourBinding>();
            }
            b.Bind(operation,setting);
            return operation;
        }
        
        public static TweenOperation Bind(this TweenOperation operation,GameObject go){
            return Bind(operation,go,BehaviourBindSetting.Default);
        }
        
    }

    public partial struct TweenOperation{

        public static void CancelAllBindings(GameObject go){
            var b = go.GetComponent<TweenBehaviourBinding>();
            if(b == null){
                return;
            }
            b.CancelAll();
        }

    }


    public enum BehaviourType{
        None,
        RanToEnd,
        Cancel,
        Resume,
        Pause,
    }


    public struct BehaviourBindSetting{
        public BehaviourType behaviourOnEnable;
        public BehaviourType behaviourOnDisable;
        public BehaviourType behaviourOnDestroy;


        public readonly static BehaviourBindSetting Default = new BehaviourBindSetting(){
            behaviourOnDisable = BehaviourType.Pause,
            behaviourOnEnable = BehaviourType.Resume,
            behaviourOnDestroy = BehaviourType.Cancel,
        };
    }

}




