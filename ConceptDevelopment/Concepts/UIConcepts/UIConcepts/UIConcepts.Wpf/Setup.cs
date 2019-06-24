extern alias mvvmcrossnew;

using MvvmCross.Converters;
using MvvmCross.Platforms.Wpf.Core;
using MvvmCross.Platforms.Wpf.Presenters;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System.Windows.Controls;

namespace UIConcepts.Wpf
{
    public class Setup : MvxWpfSetup<Core.App>
    {
        protected override IMvxWpfViewPresenter CreateViewPresenter(ContentControl root)
        {
            return new MasterDetailPresenter(root);
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);
            registry.AddOrOverwrite("Language", new mvvmcrossnew.MvvmCross.Localization.MvxLanguageConverter());
        }
    }
}
