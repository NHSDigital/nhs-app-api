using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services
{
    public class ListServiceAccesses : AbstractTppRequestModel
    {               
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "ListServiceAccesses";

    }
}