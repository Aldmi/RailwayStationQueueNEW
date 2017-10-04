using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ServerUi.Converters
{
    public class Color2BrushConverter : IValueConverter
    {
        //(VM->View)  brush->color
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var str = value.ToString();
                byte[] arr = new byte[4];

                arr[0] = byte.Parse(str.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                arr[1] = byte.Parse(str.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                arr[2] = byte.Parse(str.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                arr[3] = byte.Parse(str.Substring(7, 2), NumberStyles.AllowHexSpecifier);

                var color = Color.FromArgb(arr[0], arr[1], arr[2], arr[3]);//#FF696969
                return color;
           }

            return null;
        }

        //(View->Vm)  color->brush
        //value format #FF696969
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var str =  value.ToString();
            byte[] arr= new byte[4];

            arr[0]= byte.Parse(str.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            arr[1] = byte.Parse(str.Substring(3, 2), NumberStyles.AllowHexSpecifier);
            arr[2] = byte.Parse(str.Substring(5, 2), NumberStyles.AllowHexSpecifier);
            arr[3] = byte.Parse(str.Substring(7, 2), NumberStyles.AllowHexSpecifier);

            var color = Color.FromArgb(arr[0], arr[1], arr[2], arr[3]);//#FF696969
            return new SolidColorBrush(color);
        }
    }
}