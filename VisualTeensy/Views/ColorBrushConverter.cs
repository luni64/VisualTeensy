using System;
using System.Windows.Data;


namespace VisualTeensy
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(System.Windows.Media.SolidColorBrush))]
    public class SystemColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var drwCol = (System.Drawing.Color)value;
            var wpfCol = System.Windows.Media.Color.FromArgb(drwCol.A, drwCol.R, drwCol.G, drwCol.B);
            return new System.Windows.Media.SolidColorBrush(wpfCol);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = value as System.Windows.Media.SolidColorBrush;
            var color = brush.Color;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}