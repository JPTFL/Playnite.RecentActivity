using System;

namespace RecentActivity.Converters
{
    public class TimeConverter
    {
        public static float SecondsToHours(long seconds, int roundDecimals = 1)
        {
            return (float) Math.Round(seconds / 3600f, roundDecimals);
        }
    }
}