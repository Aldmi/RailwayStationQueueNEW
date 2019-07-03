using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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

        public XmlSerialSettings(string port, string baudRate, string dataBits, string stopBits, string timeRespoune, string timeCycleReConnect)
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
        public static IEnumerable<XmlSerialSettings> LoadXmlSetting(XElement xml)
        {
            var sett = from el in xml?.Element("Server")?.Element("SerialPorts")?.Elements("Serial")
                select new XmlSerialSettings(
                    (string)el.Element("Port"),
                    (string)el.Element("BaudRate"),
                    (string)el.Element("DataBits"),
                    (string)el.Element("StopBits"),
                    (string)el.Element("TimeRespon"),
                    (string)el.Element("TimeCycleReConnect"));


            foreach (var port in sett)
            {
                if (string.IsNullOrEmpty(port.Port))
                    throw new Exception($"Порт не указанн: {port.Port}");
            }

            return sett;
        }

        #endregion
    }
}