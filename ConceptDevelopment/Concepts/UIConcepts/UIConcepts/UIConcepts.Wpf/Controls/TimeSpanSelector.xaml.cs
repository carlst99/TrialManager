using System;
using System.Windows;
using System.Windows.Controls;

namespace UIConcepts.Wpf.Controls
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
            DataContext = this;
        }
    }
}
