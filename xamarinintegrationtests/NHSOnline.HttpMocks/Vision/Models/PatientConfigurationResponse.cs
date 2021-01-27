using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class PatientConfigurationResponse
    {
        [XmlElement(ElementName = "configuration", Namespace = "urn:vision")]
        public PatientConfiguration? Configuration { get; set; }
    }
}