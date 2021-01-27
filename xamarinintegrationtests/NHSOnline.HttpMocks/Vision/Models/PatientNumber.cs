using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class PatientNumber
    {
        [XmlElement(ElementName = "numberType", Namespace = "urn:vision")]
        public string? NumberType { get; set; }

        [XmlElement(ElementName = "number", Namespace = "urn:vision")]
        public string? Number { get; set; }
    }
}