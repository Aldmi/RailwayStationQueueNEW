using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;


namespace ServerUi.Model
{
    public class FontSetting : Screen  //TODO: Screen заменить
    {
        #region Шрифт Заголовка

        private Font _fontHeader;
        public Font FontHeader
        {
            get { return _fontHeader; }
            set
            {
                _fontHeader = value;
                if(_fontHeader == null)
                    return;

                var ffc = new FontFamilyConverter();
                FontFamilyHeader = (FontFamily)ffc.ConvertFromString(_fontHeader.Name);
                NotifyOfPropertyChange(() => FontFamilyHeader);

                FontSizeHeader = _fontHeader.Size;
                NotifyOfPropertyChange(() => FontSizeHeader);

                FontWeightHeader = _fontHeader.Bold ? FontWeights.Bold : FontWeights.Normal;
                NotifyOfPropertyChange(() => FontWeightHeader);

                NotifyOfPropertyChange(() => FontHeaderToString);
            }
        }

        public float FontSizeHeader { get; private set; }
        public FontFamily FontFamilyHeader { get; private set; }
        public FontWeight FontWeightHeader { get; private set; }
        public string FontHeaderToString => $@"{FontFamilyHeader};{FontSizeHeader};{FontWeightHeader}";

        #endregion





        #region Шрифт Строки

        private Font _fontRow;
        public Font FontRow
        {
            get { return _fontRow; }
            set
            {
                _fontRow = value;
                if (_fontRow == null)
                    return;

                var ffc = new FontFamilyConverter();
                FontFamilyRow = (FontFamily)ffc.ConvertFromString(_fontRow.Name);
                NotifyOfPropertyChange(() => FontFamilyRow);

                FontSizeRow = _fontRow.Size;
                NotifyOfPropertyChange(() => FontSizeRow);

                FontWeightRow = _fontRow.Bold ? FontWeights.Bold : FontWeights.Normal;
                NotifyOfPropertyChange(() => FontSizeRow);

                NotifyOfPropertyChange(() => FontRowToString);
            }
        }

        public float FontSizeRow { get; private set; }
        public FontFamily FontFamilyRow { get; private set; }
        public FontWeight FontWeightRow { get; private set; }
        public string FontRowToString => $@"{FontFamilyRow};{FontSizeRow};{FontWeightRow}";

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