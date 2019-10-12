using System;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.Platforms.Wpf.Converters;
using MvvmCross.Plugin.Color;
using MvvmCross.Plugin.Visibility;

namespace TrialManager.Wpf
{
    public class NativeMvxVisibilityValueConverter : MvxNativeValueConverter<MvxVisibilityValueConverter> { }

    public class NativeMvxInvertedVisibilityValueConverter : MvxNativeValueConverter<MvxInvertedVisibilityValueConverter> { }

    public class NativeMvxColorConverter : MvxNativeValueConverter<MvxNativeColorValueConverter> { }

    public class NativeIntToStringConverter : MvxNativeValueConverter<IntToStringConverter> { }

    public class IntToStringConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        protected override int ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value);
        }
    }
}
