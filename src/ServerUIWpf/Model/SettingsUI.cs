using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace ServerUi.Model
{
    [Serializable]
    public class SettingsUi
    {
        #region prop

        public string HeaderBackgroundColorString { get; set; }
        public string HeaderFontColorString { get; set; }
        public string ColorListRowsString { get; set; }
        public string ColorListBackgroundString { get; set; }
        public string ListFontColorString { get; set; }


        public string FontCashierRowString { get; set; }
        public int FontCashierRowPadding { get; set; }


        public string Font8X2HeaderString { get; set; }
        public string Font8X2RowString { get; set; }
        public int Font8X2HeaderPadding { get; set; }
        public int Font8X2RowPadding { get; set; }


        public string Font4X4HeaderString { get; set; }
        public string Font4X4RowString { get; set; }
        public int Font4X4HeaderPadding { get; set; }
        public int Font4X4RowPadding { get; set; }

        #endregion




        #region ctor

        public SettingsUi(Brush headerBackgroundColor, 
                        Brush headerFontColor,
                        Brush colorListRows,
                        Brush colorListBackground,
                        Brush listFontColor,
                        FontSetting currentFontCash,
                        FontSetting currentFont8X2,
                        FontSetting currentFont4X4)
        {
            HeaderBackgroundColorString = headerBackgroundColor.ToString();
            HeaderFontColorString = headerFontColor.ToString();
            ColorListRowsString = colorListRows.ToString();
            ColorListBackgroundString = colorListBackground.ToString();
            ListFontColorString = listFontColor.ToString();

            var cvt = new FontConverter();

            FontCashierRowString = cvt.ConvertToString(currentFontCash.FontRow);
            FontCashierRowPadding = currentFontCash.PaddingRow;

            Font8X2HeaderString = cvt.ConvertToString(currentFont8X2.FontHeader);
            Font8X2RowString = cvt.ConvertToString(currentFont8X2.FontRow);
            Font8X2HeaderPadding = currentFont8X2.PaddingHeader;
            Font8X2RowPadding = currentFont8X2.PaddingRow;

            Font4X4HeaderString = cvt.ConvertToString(currentFont4X4.FontHeader);
            Font4X4RowString = cvt.ConvertToString(currentFont4X4.FontRow);
            Font4X4HeaderPadding = currentFont4X4.PaddingHeader;
            Font4X4RowPadding = currentFont4X4.PaddingRow;
        }

        #endregion




        public Brush ConvertString2Brush(string argb)
        {
            if (argb == null)
                return null;

            byte[] arr = new byte[4];

            arr[0] = byte.Parse(argb.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            arr[1] = byte.Parse(argb.Substring(3, 2), NumberStyles.AllowHexSpecifier);
            arr[2] = byte.Parse(argb.Substring(5, 2), NumberStyles.AllowHexSpecifier);
            arr[3] = byte.Parse(argb.Substring(7, 2), NumberStyles.AllowHexSpecifier);

            var color = Color.FromArgb(arr[0], arr[1], arr[2], arr[3]);//#FF696969
            return new SolidColorBrush(color);
        }



        public Font ConvertString2Font(string font)
        {
            if (font == null)
                return null;

            var cvt = new FontConverter();
            return cvt.ConvertFromString(font) as Font;
        }
    }
}