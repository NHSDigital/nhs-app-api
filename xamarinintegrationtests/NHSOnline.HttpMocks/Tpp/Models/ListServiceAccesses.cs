using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class ListServiceAccesses : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")] public string? PatientId { get; set; }

        [XmlAttribute("onlineUserId")] public string? OnlineUserId { get; set; }

    }
}