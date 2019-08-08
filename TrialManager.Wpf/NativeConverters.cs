using MvvmCross.Platforms.Wpf.Converters;
using MvvmCross.Plugin.Color;
using MvvmCross.Plugin.Visibility;

namespace TrialManager.Wpf
{
    public class NativeMvxVisibilityValueConverter : MvxNativeValueConverter<MvxVisibilityValueConverter> { }

    public class NativeMvxInvertedVisibilityValueConverter : MvxNativeValueConverter<MvxInvertedVisibilityValueConverter> { }

    public class NativeMvxColorConverter : MvxNativeValueConverter<MvxNativeColorValueConverter> { }
}
