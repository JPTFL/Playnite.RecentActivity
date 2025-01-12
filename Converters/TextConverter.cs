using Playnite.SDK;

namespace RecentActivity.Converters
{
    public class TextConverter
    {
        public static string SecondsToHoursText(int seconds)
        {
            if (seconds < 360)
            {
                return string.Format(ResourceProvider.GetString("LOC_RecentActivity_Hours"),
                    TimeConverter.SecondsToHours(seconds, 2));
            }
            else
            {
                var hours = TimeConverter.SecondsToHours(seconds);
                return string.Format(ResourceProvider.GetString("LOC_RecentActivity_Hours"),
                    TimeConverter.SecondsToHours(seconds));
            }
        }
    }
}