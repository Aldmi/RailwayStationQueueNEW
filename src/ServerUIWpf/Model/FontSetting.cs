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
        #region Шрифт Заголовка

        private Font _fontHeader;
        public Font FontHeader
        {
            get { return _fontHeader; }
            set
            {
                _fontHeader = value;

                var ffc = new FontFamilyConverter();
                FontFamilyHeader = (FontFamily)ffc.ConvertFromString(_fontHeader.Name);

                FontSizeHeader = _fontHeader.Size;

                FontWeightHeader = _fontHeader.Bold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public float FontSizeHeader { get; set; }
        public FontFamily FontFamilyHeader { get; set; }
        public FontWeight FontWeightHeader { get; set; }


        public string FontHeaderToString()
        {
            return $@"{FontFamilyHeader};{FontSizeHeader};{FontWeightHeader}";
        }

        #endregion





        #region Шрифт Строки

        private Font _fontRow;
        public Font FontRow
        {
            get { return _fontRow; }
            set
            {
                _fontRow = value;

                var ffc = new FontFamilyConverter();
                FontFamilyRow = (FontFamily)ffc.ConvertFromString(_fontRow.Name);

                FontSizeRow = _fontRow.Size;

                FontWeightRow = _fontRow.Bold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public float FontSizeRow { get; set; }
        public FontFamily FontFamilyRow { get; set; }
        public FontWeight FontWeightRow { get; set; }


        public string FontRowToString()
        {
            return $@"{FontFamilyRow};{FontSizeRow};{FontWeightRow}";
        }

        #endregion


        //ОТСТУП ЗАГОЛОВКА
        private int _paddingHeader;
        public int PaddingHeader
        {
            get { return _paddingHeader; }
            set
            {
                _paddingHeader = value;
                NotifyOfPropertyChange(() => PaddingHeader);
            }
        }


        //ОТСТУП ЗАГОЛОВКА
        private int _paddingRow;
        public int PaddingRow
        {
            get { return _paddingRow; }
            set
            {
                _paddingRow = value;
                NotifyOfPropertyChange(() => PaddingRow);
            }
        }

    }
}