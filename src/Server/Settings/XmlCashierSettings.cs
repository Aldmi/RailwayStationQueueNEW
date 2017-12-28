using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Server.Settings
{
    public class XmlCashierSettings
    {
        #region prop

        public byte Id { get; set; }
        public byte AddressDevice { get; set; }
        public string Port { get; set; }
        public string NameQueue { get; set; }
        public List<string> Prefixs { get; set; }    //Указываем в порядке приоритета обработки, All - обработка с головы очереди, префиксы которые идут за All - это искючая обработку.       

        public byte MaxCountTryHanding { get; set; }

        #endregion




        #region ctor

        private XmlCashierSettings(string id, string addressDevice, string port, string nameQueue, List<string> prefixs, string maxCountTryHanding)
        {
            Id = byte.Parse(id);
            AddressDevice = byte.Parse(addressDevice);
            Port = port;
            NameQueue = nameQueue;
            Prefixs = prefixs;
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
                           (string)el.Attribute("Address"),
                           (string)el.Attribute("Port"),
                           (string)el.Attribute("NameQueue"),             
                           ParsePrefix((string)el.Attribute("Prefix")), 
                           (string)el.Attribute("MaxCountTryHanding"));

            return sett.ToList();
        }



        private static List<string> ParsePrefix(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return str.Split(',').ToList();
        }

        #endregion
    }
}