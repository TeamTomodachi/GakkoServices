using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public static class TimespanHelper
    {
        public static long Microseconds(this TimeSpan span) { return span.Milliseconds * 1000; }
        public static long Nanoseconds(this TimeSpan span) { return span.Milliseconds * 1000000; }
        public static double TotalMicroseconds(this TimeSpan span) { return span.TotalMilliseconds * 1000; }
        public static double TotalNanoseconds(this TimeSpan span) { return span.TotalMilliseconds * 1000000; }
    }
}
