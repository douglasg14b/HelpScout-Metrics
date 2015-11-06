using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace HelpScoutMetrics.Model.SaveAndLoad
{
    public class XMLSerialize<T>
    {
        public static T Deserialize(string type)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T result = (T)serializer.Deserialize(new StringReader(type));

            return result;
        }

        public static string Serialize(T type)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string originalMessage;

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, type);
                stream.Position = 0;
                XmlDocument document = new XmlDocument();
                document.Load(stream);

                originalMessage = document.OuterXml;
            }

            return originalMessage;
        }

        public static T CopyData(T type)
        {
            return Deserialize(Serialize(type));
        }
    }
}
