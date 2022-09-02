using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    [XmlType("TestResultsView")]
    public class TestResultsViewDetailed : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")] public string? PatientId { get; set; }

        [XmlAttribute("onlineUserId")] public string? OnlineUserId { get; set; }

        [XmlAttribute("testResultId")] public string? TestResultId { get; set; }
    }
}