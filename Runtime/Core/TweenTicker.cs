using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{

    public static class TweenTicker{

        public delegate void TickAction(TickData tickData);

        private static List<TickAction> _tickActions = new List<TickAction>();

        private static bool _ticking = false;

        private static int _tickingIndex = -1;

        internal static void AddTick(TickAction action){
            _tickActions.Add(action);
        }

        internal static bool RemoveTick(TickAction action){
            if(_ticking){
                var index = _tickActions.IndexOf(action);
                if(index < 0){
                    return false;
                }
                if(index <= _tickingIndex){
                    _tickingIndex --;
                }
                return _tickActions.Remove(action);
            }else{
                return _tickActions.Remove(action);
            }
        }

        public static int tickingCount{
            get{
                return _tickActions.Count;
            }
        }

        internal static void Clear(){
            _tickActions.Clear();
        }

        internal static void Tick(TickData tickData){
            _ticking = true;
            _tickingIndex = 0;
            while(_tickingIndex < _tickActions.Count){
                var tick = _tickActions[_tickingIndex];
                try{
                    tick(tickData);
                }catch(System.Exception e){
                    Debug.LogException(e);
                }
                _tickingIndex ++;
            }
            _ticking = false;
            
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
