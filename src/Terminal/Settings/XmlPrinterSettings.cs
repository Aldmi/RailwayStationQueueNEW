using System.Xml.Linq;
using Communication.Annotations;


namespace Terminal.Settings
{
    public class XmlPrinterSettings
    {
        #region prop

        public string PrinterName { get; }

        #endregion




        #region ctor

        private XmlPrinterSettings([NotNull]string printerName)
        {
            PrinterName = printerName;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Обязательно вызывать в блоке try{}
        /// </summary>
        public static XmlPrinterSettings LoadXmlSetting(XElement xml)
        {
            XmlPrinterSettings settServer =
                new XmlPrinterSettings(
                    (string)xml.Element("Printer")?.Element("Name"));

            return settServer;
        }

        #endregion
    }
}