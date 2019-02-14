using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    [XmlType("TestResultsView")]
    public class TestResultsViewDetailed : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }
        
        [XmlAttribute("testResultId")]
        public string TestResultId { get; set; }
        
        [XmlIgnore]
        public override string RequestType => "TestResultsView";
    }
}