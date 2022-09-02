using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class RequestBinaryDataReply
    {
        [XmlElement("BinaryData")] public BinaryDataElement? BinaryData { get; set; }
    }
}