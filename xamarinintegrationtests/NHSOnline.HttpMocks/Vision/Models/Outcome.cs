using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "outcome", Namespace = "urn:vision")]
    public class Outcome
    {
        [XmlElement(ElementName = "successful", Namespace = "urn:vision")]
        public string Successful { get; set; } = "true";
    }
}