using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
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