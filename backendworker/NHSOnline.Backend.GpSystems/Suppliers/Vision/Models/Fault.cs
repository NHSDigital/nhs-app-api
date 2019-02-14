using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class Fault
    {
        [XmlElement(ElementName = "faultcode", Namespace = "")]
        public string FaultCode { get; set; }
        
        [XmlElement(ElementName = "faultstring", Namespace = "")]
        public string FaultString { get; set; }

        [XmlElement(ElementName = "detail", Namespace = "")]
        public Detail Detail { get; set; }

        [XmlAttribute(AttributeName = "vision", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vision { get; set; }
    }
}