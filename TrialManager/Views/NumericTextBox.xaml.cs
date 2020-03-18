using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : UserControl
    {


        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ValuePropertyChanged,
                    CoerceValueProperty));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(
                    double.MinValue,
                    FrameworkPropertyMetadataOptions.None,
                    MinimumPropertyChanged,
                    CoerceMinimumProperty));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(
                    double.MaxValue,
                    FrameworkPropertyMetadataOptions.None,
                    MaximumPropertyChanged,
                    CoerceMaximumProperty));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(
                    1d,
                    FrameworkPropertyMetadataOptions.None));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public NumericTextBox()
        {
            InitializeComponent();
        }

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceValueProperty(DependencyObject d, object value)
        {
            NumericTextBox n = (NumericTextBox)d;
            double v = (double)value;
            if (v < n.Minimum)
                v = n.Minimum;
            if (v > n.Maximum)
                v = n.Maximum;
            return v;
        }

        private static void MinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ValueProperty);
        }

        private static object CoerceMinimumProperty(DependencyObject d, object value)
        {
            NumericTextBox n = (NumericTextBox)d;
            if ((double)value > n.Maximum)
                throw new ArgumentOutOfRangeException("Minimum cannot be more than maximum!");
            else
                return value;
        }

        private static void MaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ValueProperty);
        }

        private static object CoerceMaximumProperty(DependencyObject d, object value)
        {
            NumericTextBox n = (NumericTextBox)d;
            if ((double)value < n.Minimum)
                throw new ArgumentOutOfRangeException("Maximum cannot be less than minimum!");
            else
                return value;
        }

        private void BtnIncrementUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Increment;
        }

        private void BtnIncrementDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
        }

        /// <summary>
        /// Only allow number input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBxInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Regex.Match(e.Text, "[0-9]").Success)
                e.Handled = true;
        }

        /// <summary>
        /// Prevent null values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBxInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;

            if (string.IsNullOrEmpty(t.Text))
                Value = Minimum;
        }
    }
}
