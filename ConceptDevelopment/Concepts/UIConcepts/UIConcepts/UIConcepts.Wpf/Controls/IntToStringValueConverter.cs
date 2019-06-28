using System;
using System.Globalization;
using System.Windows.Data;

namespace UIConcepts.Wpf.Controls
{
    public class IntToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue.ToString("00");
            else
                throw new ArgumentException("Value must be of type int");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
                return int.Parse(stringValue);
            else
                throw new ArgumentException("Value must be of type string");
        }
    }
}
