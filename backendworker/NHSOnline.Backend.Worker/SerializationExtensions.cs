using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker
{
    public static class SerializationExtensions
    {
        public static string SerializeJson(this object toJson)
        {
            if (toJson == null)
            {
                return string.Empty;
            }
            try
            {
                return JsonConvert.SerializeObject(toJson);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when attempting to serialize object to json");
                Console.WriteLine("JSON");
                Console.WriteLine("--------------------");
                Console.WriteLine();
                Console.WriteLine("EXCEPTION");
                Console.WriteLine("--------------------");
                Console.WriteLine(e);
                throw;
            }
        }

        public static T DeserializeJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                // TODO: Check logging strategy
                Console.WriteLine($"JSON was null or empty when attempting to deserialize: { typeof(T).FullName }");
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when attempting to deserialize json: { typeof(T).FullName }");
                Console.WriteLine("JSON");
                Console.WriteLine("--------------------");
                Console.WriteLine(json);
                Console.WriteLine();
                Console.WriteLine("EXCEPTION");
                Console.WriteLine("--------------------");
                Console.WriteLine(e);
                return default(T);
            }
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
                return default(T);
            }
            
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(new StringReader(xml));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when attempting to deserialize xml: { typeof(T).FullName }");
                Console.WriteLine("XML");
                Console.WriteLine("--------------------");
                Console.WriteLine(xml);
                Console.WriteLine();
                Console.WriteLine("EXCEPTION");
                Console.WriteLine("--------------------");
                Console.WriteLine(e);

                throw;
            }
        }
    }
}
