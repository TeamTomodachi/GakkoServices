using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public static class DateTimeHelpers
    {
        public const int Minute = 60;
        public const int Hour = Minute * 60;
        public const int Day = Hour * 24;
        public const int Year = Day * 365;

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static string ToRelativeDateTimeString(this DateTime utcValue)
        {
            // Calculate the Difference
            var difference = DateTime.UtcNow - utcValue;

            // Exit early if inputs are invalid
            if (utcValue == DateTime.MinValue) return "";
            else if (utcValue == DateTime.MaxValue) return "";

            // Begin converting to relative time
            string result = "";
            if (difference.TotalSeconds < 2.0)
                result = "a second ago";
            else if (difference.TotalSeconds < Minute)
                result = Math.Floor(difference.TotalSeconds) + " seconds ago";

            else if (difference.TotalSeconds < Minute * 2)
                result = "a minute ago";
            else if (difference.TotalSeconds < Hour)
                result = Math.Floor(difference.TotalMinutes) + " minutes ago";

            else if (difference.TotalSeconds < Hour * 2)
                result = "an hour ago";
            else if (difference.TotalSeconds < Day)
                result = Math.Floor(difference.TotalHours) + " hours ago";

            else if (difference.TotalSeconds < Day * 2)
                result = "yesterday";
            else if (difference.TotalSeconds < Day * 30)
                result = Math.Floor(difference.TotalDays) + " days ago";

            else if (difference.TotalSeconds < Day * 60)
                result = "a month ago";
            else if (difference.TotalSeconds < Year)
                result = Math.Floor(difference.TotalDays / 30) + " months ago";

            // And because no one cares once its past a certain point, just display a year
            else result = utcValue.ToString();

            //Debug.WriteLine(result.ToString());
            return result;
        }
    }
}
