using System.Diagnostics;

namespace MS.TweenAsync.Profiler{

    
    public class TweenProfiler
    {
        static internal int _actionDriverAllocateCount = 0;

        [Conditional("UNITY_EDITOR")]
        internal static void TraceAllocateActionDriver(){
            _actionDriverAllocateCount ++;
        }

        public static int totalActionDriverAllocateTimes{
            get{
                return _actionDriverAllocateCount;
            }
        }


    }
}
