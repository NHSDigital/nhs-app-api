using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class ServiceContent : IXmlSerializable
    {
        private string? _rawServiceContent;

        public T As<T>()
        {
            if (_rawServiceContent == null)
            {
                throw new InvalidOperationException("Missing service content");
            }

            var serializer = new XmlSerializer(typeof(T));
            using var xmlTextReader = new XmlTextReader(new StringReader(_rawServiceContent));
            return (T)serializer.Deserialize(xmlTextReader);
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _rawServiceContent = reader.ReadInnerXml();
        }

        XmlSchema? IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException("The ServiceContent class is used for requests only and doesn't need to be serialised from these stubs");
        }
    }
}