using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class ViewPatientOverview : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")] public string? PatientId { get; set; }

        [XmlAttribute("onlineUserId")] public string? OnlineUserId { get; set; }
    }
}