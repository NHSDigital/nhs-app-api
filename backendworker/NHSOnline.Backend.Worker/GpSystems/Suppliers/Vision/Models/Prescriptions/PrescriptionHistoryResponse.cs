using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class PrescriptionHistoryResponse
    {
        [XmlElement(ElementName = "prescriptionHistory", Namespace = "urn:vision")]
        public PrescriptionHistory PrescriptionHistory { get; set; }
    }
}
