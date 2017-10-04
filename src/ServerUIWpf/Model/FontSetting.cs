using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace ServerUi.Model
{
    public class FontSetting : Screen
    {
        private Font _font;
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;

                var ffc = new FontFamilyConverter();
                FontFamily = (FontFamily)ffc.ConvertFromString(_font.Name);

                Size= _font.Size;

                FontWeight = _font.Bold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public float Size { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }



        private Brush _backGroundColorBrush;
        public Brush BackGroundColorBrush                 //TODO: реализовать Converter Color -> Brush. применить его на ColorPicker во View. Хранить только BackGroundColorBrush.
        {
            get { return _backGroundColorBrush; }
            set
            {
                _backGroundColorBrush = value;
                NotifyOfPropertyChange(() => BackGroundColorBrush);
            }
        }




        public Color FontColor { get; set; }



        public override string ToString()
        {
            return $@"{FontFamily};{Size}";
        }
    }
}