using System;
using System.Globalization;
using System.Windows.Data;

namespace UIConcepts.Wpf.Controls
{
    public class TimeSpanToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
                return time.ToString(@"hh\:mm");
            else
                throw new ArgumentException("Value must be of type " + typeof(TimeSpan));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string time)
                return TimeSpan.Parse(time);
            else
                throw new ArgumentException("Value must be of type " + typeof(string));
        }
    }
}
