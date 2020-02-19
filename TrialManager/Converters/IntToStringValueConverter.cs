﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace TrialManager.Converters
{
    public class IntToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
                return number.ToString();
            else
                return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string number)
            {
                if (int.TryParse(number, out int result))
                    return result;
                else
                    return 0;
            } else
            {
                return 0;
            }
        }
    }
}
