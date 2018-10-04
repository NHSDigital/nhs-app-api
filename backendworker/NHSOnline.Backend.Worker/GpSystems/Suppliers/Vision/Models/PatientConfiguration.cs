using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientConfiguration
    {
        [XmlElement(ElementName = "account", Namespace = "urn:vision")]
        public Account Account { get; set; }

        [XmlAttribute(AttributeName = "mb", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mb { get; set; }
    }
}
