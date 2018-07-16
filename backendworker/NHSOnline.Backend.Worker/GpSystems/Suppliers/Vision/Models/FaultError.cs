using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class FaultError
    {
        [XmlElement(ElementName = "category", Namespace = "urn:vision")]
        public string Category { get; set; }
        
        [XmlElement(ElementName = "code", Namespace = "urn:vision")]
        public string Code { get; set; }

        [XmlElement(ElementName = "text", Namespace = "urn:vision")]
        public string Text { get; set; }
    }
}