using System;
using System.Text;

namespace Library.Convertion
{
    public static class Array2StringConverter
    {
        public static string ConertByteArray2String(this byte[] array)
        {
            var strB= new StringBuilder();
            foreach (var b in array)
            {
                strB.Append(b.ToString("X2") + " ");
            }

            return strB.ToString();
        }
    }
}