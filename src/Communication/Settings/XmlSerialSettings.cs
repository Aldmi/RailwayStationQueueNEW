using System;
using System.IO.Ports;
using System.Xml.Linq;
using Communication.Annotations;

namespace Communication.Settings
{
    public class XmlSerialSettings
    {
        #region prop

        public string Port { get; }
        public int BaudRate { get; }
        public int DataBits { get; }
        public StopBits StopBits { get; set; }
        public ushort TimeRespoune { get; }
        public ushort TimeCycleReConnect { get; set; }

        #endregion




        #region ctor

        private XmlSerialSettings(string port, string baudRate, string dataBits, string stopBits, string timeRespoune, string timeCycleReConnect)
        {
            Port = port;
            BaudRate = int.Parse(baudRate);
            DataBits = int.Parse(dataBits);
            StopBits = (int.Parse(stopBits) == 1) ? StopBits.One : StopBits.Two;
            TimeRespoune = ushort.Parse(timeRespoune);
            TimeCycleReConnect = ushort.Parse(timeCycleReConnect);
        }

        #endregion




        #region Methode

        /// <summary>
        /// Обязательно вызывать в блоке try{}
        /// </summary>
        public static XmlSerialSettings LoadXmlSetting(XElement xml)
        {
            XmlSerialSettings settServer =
                new XmlSerialSettings(
                    (string) xml.Element("Server")?.Element("Serial")?.Element("Port"),
                    (string) xml.Element("Server")?.Element("Serial")?.Element("BaudRate"),
                    (string) xml.Element("Server")?.Element("Serial")?.Element("DataBits"),
                    (string) xml.Element("Server")?.Element("Serial")?.Element("StopBits"),
                    (string) xml.Element("Server")?.Element("Serial")?.Element("TimeRespon"),
                    (string) xml.Element("Server")?.Element("Serial")?.Element("TimeCycleReConnect"));

            if(string.IsNullOrEmpty(settServer.Port))
                throw  new Exception("Порт не указанн");

            return settServer;
        }

        #endregion
    }
}