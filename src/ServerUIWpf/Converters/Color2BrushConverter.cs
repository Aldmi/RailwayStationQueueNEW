using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ServerUi.Converters
{
    public class Color2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var color = (Color) value; //TODO: Exception приведения

                return new SolidColorBrush(color);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Colors.Brown;
        }
    }
}