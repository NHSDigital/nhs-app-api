using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class TestResultsView : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")] public string? PatientId { get; set; }

        [XmlAttribute("onlineUserId")] public string? OnlineUserId { get; set; }

        [XmlAttribute("startDate")] public string? StartDate { get; set; }

        [XmlAttribute("endDate")] public string? EndDate { get; set; }
    }
}