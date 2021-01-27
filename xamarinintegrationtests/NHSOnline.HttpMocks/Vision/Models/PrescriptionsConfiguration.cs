using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class PrescriptionsConfiguration
    {
        [XmlElement(ElementName = "repeat_enabled", Namespace = "urn:vision")]
        public bool? RepeatEnabled { get; set; }
    }
}