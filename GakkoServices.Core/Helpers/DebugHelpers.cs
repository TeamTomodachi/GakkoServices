using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public static class DebugHelpers
    {
        public static bool DebugMode { get { return Debugger.IsAttached; } }
        public static bool DebugConditional
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
        
        public static void PrintTotaloMemoryInUse()
        {
            Debug.WriteLine("GC: TOTAL MEMORY {0}", GC.GetTotalMemory(false));
        }

        public static int Microseconds(this Stopwatch watch) { return (int)(watch.ElapsedTicks * 1.0e6 / Stopwatch.Frequency + 0.4999); }
        public static int Nanoseconds(this Stopwatch watch) { return (int)(watch.ElapsedTicks * 1.0e9 / Stopwatch.Frequency + 0.4999); }
    }
}
