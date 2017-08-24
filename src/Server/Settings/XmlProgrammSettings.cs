using System.Xml.Linq;

namespace Server.Settings
{
    public class XmlProgrammSettings
    {
        #region prop

        public int BlockClickTime { get; set; }
        public byte CashierMaxCountTryHanding { get; set; } 

        #endregion




        #region ctor

        private XmlProgrammSettings(string blockClickTime, string cashierMaxCountTryHanding)
        {
            BlockClickTime = int.Parse(blockClickTime);
            CashierMaxCountTryHanding = byte.Parse(cashierMaxCountTryHanding);
        }

        #endregion




        #region Methode

        public static XmlProgrammSettings LoadXmlSetting(XElement xml)
        {
            XmlProgrammSettings settings =
                new XmlProgrammSettings(
                    (string)xml.Element("Programm").Element("BlockClickTime"),
                    (string)xml.Element("Programm").Element("CashierMaxCountTryHanding"));
                   
            return settings;
        }

        #endregion
    }
}