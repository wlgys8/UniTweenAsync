using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{

    public static class TweenTicker{

        public delegate void TickAction(TickData tickData);

        private static List<TickAction> _tickActions = new List<TickAction>();

        private static HashSet<TickAction> _waitingToBeRemoved = new HashSet<TickAction>();

        private static bool _ticking = false;

        internal static void AddTick(TickAction action){
            if(_waitingToBeRemoved.Contains(action)){
                _waitingToBeRemoved.Remove(action);
            }else{
                _tickActions.Add(action);
            }
        }

        internal static bool RemoveTick(TickAction action){
            var res = _tickActions.Contains(action);
            if(_ticking){
                _waitingToBeRemoved.Add(action);
            }else{
                _tickActions.Remove(action);
            }
            return res;
        }

        public static int tickingCount{
            get{
                return _tickActions.Count;
            }
        }

        internal static void Clear(){
            _tickActions.Clear();
            _waitingToBeRemoved.Clear();
        }

        internal static void Tick(TickData tickData){
            _ticking = true;
            for(var i = 0; i < _tickActions.Count;i++){
                var tick = _tickActions[i];
                try{
                    tick(tickData);
                }catch(System.Exception e){
                    Debug.LogException(e);
                }
            }
            _ticking = false;
            if(_waitingToBeRemoved.Count > 0){
                foreach(var tick in _waitingToBeRemoved){
                    if(!_tickActions.Remove(tick)){
                        Debug.LogError("failed to remove action from ticker");
                    }
                }
                _waitingToBeRemoved.Clear();
            }
            
        }


        public struct TickData{
            
            private float _unscaledDeltaTime;
            private float _timeScale;
            internal TickData(float unscaledDeltaTime,float timeScale){
                _unscaledDeltaTime = unscaledDeltaTime;
                _timeScale = timeScale;
            }

            public float unscaledDeltaTime{
                get{
                    return _unscaledDeltaTime;
                }
            }

            public float timeScale{
                get{
                    return _timeScale;
                }
            }

            public float scaledDeltaTime{
                get{
                    return _unscaledDeltaTime * _timeScale;
                }
            }
        }
    }



    internal class TweenTickBehaviour:MonoBehaviour{


        private void Update(){
            var data = new TweenTicker.TickData(Time.unscaledDeltaTime,Time.timeScale);
            TweenTicker.Tick(data);
        }

        void OnApplicationQuit(){
            TweenTicker.Clear();
        }


        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad(){
            var b = new GameObject("TweenTickBehaviour").AddComponent<TweenTickBehaviour>();
            GameObject.DontDestroyOnLoad(b.gameObject);
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorOnLoadMethond(){
            UnityEditor.EditorApplication.update += OnEditorUpdate;
            _lastEditorUpdateTime = UnityEditor.EditorApplication.timeSinceStartup;
        }

        private static double _lastEditorUpdateTime;

        private static void OnEditorUpdate(){
            //only work when Application is not playing.
            if(Application.isPlaying){
                return;
            }
            var deltaTime = UnityEditor.EditorApplication.timeSinceStartup - _lastEditorUpdateTime;
            _lastEditorUpdateTime = UnityEditor.EditorApplication.timeSinceStartup;
            var data = new TweenTicker.TickData((float)deltaTime,Time.timeScale);
            TweenTicker.Tick(data);
        }
#endif

    }
}
