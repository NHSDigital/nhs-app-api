using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class TestResultsView : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlAttribute("startDate")]
        public string StartDate { get; set; }
        
        [XmlAttribute("endDate")]
        public string EndDate { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "TestResultsView";
    }
}