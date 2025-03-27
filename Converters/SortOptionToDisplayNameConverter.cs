using System;
using System.Globalization;
using System.Windows.Data;
using RecentActivity.UI;

namespace RecentActivity.Converters
{
    public class SortOptionToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SortOption sortOption)
            {
                return sortOption.GetDisplayName();
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}