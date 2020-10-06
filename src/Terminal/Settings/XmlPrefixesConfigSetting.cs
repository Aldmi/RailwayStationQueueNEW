using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Terminal.Model;

namespace Terminal.Settings
{
    public class XmlPrefixesConfigSetting
    {
        #region prop
        public readonly Dictionary<string, PrefixeConf> PrefixDict;
        #endregion


        #region ctor
        private XmlPrefixesConfigSetting(Dictionary<string, PrefixeConf> prefixDict)
        {
            PrefixDict = prefixDict;
        }
        #endregion


        #region Methode

        public static XmlPrefixesConfigSetting LoadXmlSetting(XElement xml)
        {
            var prefixes = xml?.Element("Prefixs")?.Elements("Prefix");
            if (prefixes == null)
            {
                throw new XmlException("Список Prefixs заданн не верно");
            }

            var prefixMapping= new Dictionary<string, PrefixeConf>();
            foreach (var prefix in prefixes)
            {
                var name = (string)prefix.Attribute("Name");
                var queueName = (string)prefix.Attribute("QueueName");
                var workTime = (string)prefix.Attribute("WorkTime");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(queueName))
                {
                    throw new XmlException("Список Prefixs не может содержать пустых тэгов Name или QueueName");
                }
                var prefConf= new PrefixeConf(name, queueName, WorkTime.Parse(workTime));
                prefixMapping.Add(name, prefConf);
            }

            if (prefixMapping == null || !prefixMapping.Any())
            {
                throw new XmlException("Список Prefixs не содержит ни одного элемента");
            }
            return new XmlPrefixesConfigSetting(prefixMapping);
        }

        #endregion
    }
}