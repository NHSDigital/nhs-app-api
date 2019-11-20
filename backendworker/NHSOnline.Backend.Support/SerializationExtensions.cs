using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Support
{
    public static class SerializationExtensions
    {
        public static string SerializeJson(this object toJson)
        {
            if (toJson == null)
            {
                return string.Empty;
            }
            
            return JsonConvert.SerializeObject(toJson);
        }

        public static T DeserializeJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"JSON was null or empty when attempting to deserialize: {typeof(T).FullName}");
                return default;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeXml<T>(this T toXml)
        {
            if (toXml == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(typeof(T));
            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            var stringWriter = new StringWriterWithEncoding(Encoding.UTF8);
                
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, toXml, xmlNamespaces);
                return stringWriter.ToString();
            }
        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            public override Encoding Encoding { get; }

            public StringWriterWithEncoding(Encoding encoding)
            {
                Encoding = encoding;
            }
        }

        public static T DeserializeXml<T>(this string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Console.WriteLine($"XML was null or empty when attempting to deserialize: { typeof(T).FullName }");
                return default;
            }
            
            var serializer = new XmlSerializer(typeof(T));

            return (T) serializer.Deserialize(new StringReader(xml));
        }
    }
}
