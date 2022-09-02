using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class BinaryDataElement
    {
        [XmlAttribute("fileType")] public string? FileType { get; set; }

        [XmlElement("BinaryDataPage")] public BinaryDataPage? BinaryDataPage { get; set; }
    }
}