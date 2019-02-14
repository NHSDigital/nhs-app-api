using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class RequestPatientRecord: AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "RequestPatientRecord";
    }
}