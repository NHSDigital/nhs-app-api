using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class OutcomeError
    {   
        [XmlElement(ElementName = "code", Namespace = "urn:vision")]
        public string Code { get; set; }

        [XmlElement(ElementName = "description", Namespace = "urn:vision")]
        public string Description { get; set; }
    }
}