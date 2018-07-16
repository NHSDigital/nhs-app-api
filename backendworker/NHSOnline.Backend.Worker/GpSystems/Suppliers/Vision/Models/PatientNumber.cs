using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "patientNumber", Namespace = "urn:vision")]
    public class PatientNumber
    {
        [XmlElement(ElementName = "numberType", Namespace = "urn:vision")]
        public string NumberType { get; set; }
        [XmlElement(ElementName = "number", Namespace = "urn:vision")]
        public string Number { get; set; }
    }
}
