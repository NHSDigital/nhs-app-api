using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class ServiceContent : IXmlSerializable
    {
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _ = reader.ReadInnerXml();
        }

        XmlSchema? IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException("The ServiceContent class is used for requests only and doesn't need to be serialised from these stubs");
        }
    }
}