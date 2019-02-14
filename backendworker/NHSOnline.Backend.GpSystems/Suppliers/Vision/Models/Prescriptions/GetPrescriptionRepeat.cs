using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class GetPrescriptionRepeat : IRepeat
    {
        [XmlElement(ElementName = "drug", Namespace = "urn:vision")]
        public string Drug { get; set; }

        [XmlElement(ElementName = "dosage", Namespace = "urn:vision")]
        public string Dosage { get; set; }

        [XmlElement(ElementName = "quantity", Namespace = "urn:vision")]
        public string Quantity { get; set; }
    }
}
