using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace UIConcepts.Wpf.Controls
{
    public class TimeSpanToStringValueConverter : IMultiValueConverter
    {
        private string _format = @"hh\:mm";

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is TimeSpan time)
            {
                if (values[1] is string _format && !string.IsNullOrEmpty(_format))
                    return time.ToString(_format);
                else
                    return time.ToString(@"hh\:mm\:ss");
            }
            else
            {
                throw new ArgumentException("Value must be of type " + typeof(TimeSpan));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string time)
            {
                string validTime = BuildProperParseArgument(time);
                return new object[] { TimeSpan.Parse(validTime), string.Empty };
            }
            else
            {
                throw new ArgumentException("Value must be of type " + typeof(string));
            }
        }

        private string BuildProperParseArgument(string time)
        {
            if (string.IsNullOrEmpty(_format))
                return time;

            string[] formatPlaceholders = _format.Split(':');
            string[] components = time.Split(':');
            string newFormat = "";
            int count = 0;

            if (formatPlaceholders.Contains("hh"))
                newFormat += components[count++] + ":";
            else
                newFormat += "00:";

            if (formatPlaceholders.Contains("mm"))
                newFormat += components[count++] + ":";
            else
                newFormat += "00:";

            if (formatPlaceholders.Contains("ss"))
                newFormat += components[count++] + ":";
            else
                newFormat += "00";

            return newFormat;
        }
    }
}
