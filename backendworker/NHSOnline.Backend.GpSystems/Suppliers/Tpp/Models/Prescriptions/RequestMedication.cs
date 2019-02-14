using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions
{
    public class RequestMedication : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("notes")]
        public string Notes { get; set; }

        [XmlElement("Medication")]
        public List<MedicationRequest> Medications { get; set; } = new List<MedicationRequest>();

        [XmlIgnore]
        public override string RequestType => "RequestMedication";
    }
}