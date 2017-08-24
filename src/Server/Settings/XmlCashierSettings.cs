using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Server.Settings
{
    public class XmlCashierSettings
    {
        #region prop

        public byte Id { get; set; }
        public byte Port { get; set; }
        public string Prefix { get; set; }
        public byte MaxCountTryHanding { get; set; }

        #endregion




        #region ctor

        private XmlCashierSettings(string id, string port, string prefix, string maxCountTryHanding)
        {
            Id = byte.Parse(id);
            Port = byte.Parse(port);
            Prefix = prefix;
            MaxCountTryHanding = byte.Parse(maxCountTryHanding);
        }

        #endregion




        #region Methode



        public static List<XmlCashierSettings> LoadXmlSetting(XElement xml)
        {
            var sett =
                from el in xml?.Element("Cashiers")?.Elements("Cashier")
                select new XmlCashierSettings(
                           (string)el.Attribute("Id"),
                           (string)el.Attribute("Port"),
                           (string)el.Attribute("Prefix"),
                           (string)el.Attribute("MaxCountTryHanding"));

            return sett.ToList();
        }

        #endregion
    }
}