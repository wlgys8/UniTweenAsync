using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MS.TweenAsync.Editor{
    public class TweenProfilerWindow : EditorWindow
    {
        
        [MenuItem("Window/TweenAsync/Profiler")]
        public static void Open(){
            EditorWindow.GetWindow<TweenProfilerWindow>().ShowUtility();
        }

        void OnGUI(){
            EditorGUILayout.LabelField("Tween Profile Datas");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Total Action Allocation Times",Profiler.TweenProfiler.totalActionDriverAllocateTimes.ToString());
            EditorGUILayout.LabelField("Ticking Actions Count",TweenTicker.tickingCount.ToString());
            EditorGUILayout.EndVertical();
        }
    }
}
