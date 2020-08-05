using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace VisualTeensy
{
    [ValueConversion(typeof(bool), typeof(CursorType))]
    public class WaitCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == true ? CursorType.Wait: CursorType.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (CursorType)value == CursorType.Arrow;
        }
    }
}