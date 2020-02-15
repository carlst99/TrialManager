using System;
using System.Globalization;
using System.Windows.Data;

namespace TrialManager.Converters
{
    public class NullableDateTimeToDateTimeOffsetValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset offset)
                return (DateTime?)offset.UtcDateTime;
            else
                return (DateTime?)DateTime.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime uiValue)
                return new DateTimeOffset(uiValue);
            else
                return DateTimeOffset.MinValue;
        }
    }
}
