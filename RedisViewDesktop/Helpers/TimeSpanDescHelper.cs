using System;

namespace RedisViewDesktop.Helpers
{
    public class TimeSpanDescHelper
    {
        public static string Dscription(TimeSpan span)
        {
            if (span.TotalMinutes < 1)
            {
                return ((long)span.TotalSeconds).ToString() + "s";
            }
            else if (span.TotalHours < 1)
            {
                return ((long)span.TotalMinutes).ToString() + "min";
            }
            else if (span.Days < 1)
            {
                return ((long)span.Hours).ToString() + "h";
            }
            return ((long)span.TotalDays).ToString() + "day";
        }
    }
}
