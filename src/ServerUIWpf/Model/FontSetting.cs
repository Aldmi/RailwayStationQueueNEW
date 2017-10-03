using System.Drawing;
using System.Windows;
using System.Windows.Media;
using FontFamily = System.Windows.Media.FontFamily;

namespace ServerUi.Model
{
    public class FontSetting
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



        public override string ToString()
        {
            return $@"{FontFamily};{Size}";
        }
    }
}