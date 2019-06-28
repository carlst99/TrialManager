using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIConcepts.Wpf
{
    /// <summary>
    /// Interaction logic for TimeSpanSelector.xaml
    /// </summary>
    public partial class TimeSpanSelector : UserControl
    {
        public static readonly DependencyProperty SelectedTimeProperty 
            = DependencyProperty.Register("SelectedTime", typeof(TimeSpan), typeof(TimeSpanSelector));

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public int Hours
        {
            get => SelectedTime.Hours;
            set => SelectedTime = new TimeSpan(SelectedTime.Ticks + value * TimeSpan.TicksPerHour);
        }

        public int Minutes
        {
            get => SelectedTime.Minutes;
            set => SelectedTime = new TimeSpan(SelectedTime.Ticks + value * TimeSpan.TicksPerMinute);
        }

        public TimeSpanSelector()
        {
            InitializeComponent();
        }
    }

    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return value.ToString();
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
