using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class PatientNumber
    {
        [XmlElement(ElementName = "numberType", Namespace = "urn:vision")]
        public string NumberType { get; set; }

        [XmlElement(ElementName = "number", Namespace = "urn:vision")]
        public string Number { get; set; }
    }
}
