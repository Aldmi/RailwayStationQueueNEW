using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Terminal.Settings
{
    public class XmlPrefixesMapping2QueueSetting
    {
        #region prop
        public Dictionary<string, string> PrefixMapping { get; set; }
        #endregion


        #region ctor
        private XmlPrefixesMapping2QueueSetting(Dictionary<string, string> prefixMapping)
        {
            PrefixMapping = prefixMapping;
        }
        #endregion


        #region Methode

        public static XmlPrefixesMapping2QueueSetting LoadXmlSetting(XElement xml)
        {
            var prefixes = xml?.Element("Prefixs")?.Elements("Prefix");
            if (prefixes == null)
            {
                throw new XmlException("Список Prefixs заданн не верно");
            }

            var prefixMapping= new Dictionary<string, string>();
            foreach (var prefix in prefixes)
            {
                var name = (string)prefix.Attribute("Name");
                var queueName = (string)prefix.Attribute("QueueName");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(queueName))
                {
                    throw new XmlException("Список Prefixs не может содержать пустых тэгов Name или QueueName");
                }
                prefixMapping.Add(name, queueName);
            }

            if (prefixMapping == null || !prefixMapping.Any())
            {
                throw new XmlException("Список Prefixs не содержит ни одного элемента");
            }
            return new XmlPrefixesMapping2QueueSetting(prefixMapping);
        }

        #endregion
    }
}