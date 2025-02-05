using Playnite.SDK;

namespace RecentActivity.Converters
{
    public class TextConverter
    {
        public static string SecondsToHoursText(ulong seconds, bool showHoursText = true)
        {
            var hours = 0f;
            if (seconds < 360)
            {
                hours = TimeConverter.SecondsToHours(seconds, 2);
            }
            else
            {
                hours = TimeConverter.SecondsToHours(seconds);
            }
            
            if (showHoursText)
            {
                return string.Format(ResourceProvider.GetString("LOC_RecentActivity_Hours"), hours);
            }

            return hours.ToString();
        }
    }
}