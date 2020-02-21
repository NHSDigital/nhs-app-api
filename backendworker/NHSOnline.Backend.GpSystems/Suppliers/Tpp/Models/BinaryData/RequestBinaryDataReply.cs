using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData
{
    public class RequestBinaryDataReply
    {
        [XmlElement("BinaryData")]
        public BinaryDataElement BinaryData { get; set; }
    }
}