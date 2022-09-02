using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Extensions
{
    public static class XmlHelper
    {
        public static string SerializeXml<T>(T toXml)
        {
            if (toXml == null)
            {
                return string.Empty;
            }

            var xmlSerializer = new XmlSerializer(typeof(T));

            var stringWriter = new StringWriterWithEncoding(Encoding.UTF8);

            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(writer, toXml);
                return stringWriter.ToString();
            }
        }

        private sealed class StringWriterWithEncoding : StringWriter
        {
            public override Encoding Encoding { get; }

            public StringWriterWithEncoding(Encoding encoding)
            {
                Encoding = encoding;
            }
        }
    }
}