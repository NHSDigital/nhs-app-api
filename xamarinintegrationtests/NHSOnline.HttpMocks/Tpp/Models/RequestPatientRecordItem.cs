using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class RequestPatientRecordItem
    {
        [XmlAttribute("type")] public string? Type { get; set; }

        [XmlAttribute("details")] public string? Details { get; set; }

        [XmlAttribute("binaryDataId")] public string? BinaryDataId { get; set; }
    }
}