using System.Collections.Generic;
using System.Xml.Linq;
using Server.Entitys;

namespace Server.Settings
{
    public class XmlQueuesSettings
    {
        #region prop

        public byte Id { get; set; }
        public string Name { get; set; }
        public List<Prefix> Prefixes { get; set; }= new List<Prefix>();

        #endregion




        #region ctor

        private XmlQueuesSettings(string id, string name)
        {
            Id = byte.Parse(id);
            Name = name;
        }

        #endregion




        #region Methode



        public static List<XmlQueuesSettings> LoadXmlSetting(XElement xml)
        {
            var queuesXml= xml?.Element("Queues")?.Elements("Queue");
            if (queuesXml != null)
            {
                var queues = new List<XmlQueuesSettings>();
                foreach (var el in queuesXml)
                {
                    var q= new XmlQueuesSettings((string)el.Attribute("Id"), (string)el.Attribute("Name"));
                    var prefixes = new List<Prefix>();
                    foreach (var prefix in el.Elements("Prefix"))
                    {     
                        var name = (string) prefix.Attribute("Name");
                        var priority = (string) prefix.Attribute("Priority");
                        int intPriority;
                        if (int.TryParse(priority, out intPriority))
                        {
                            prefixes.Add(new Prefix { Name = name, Priority = intPriority });
                        }
                    }
                    q.Prefixes = prefixes;
                    queues.Add(q);
                }
                return queues;
            }

            return null;
        }

        #endregion
    }
}