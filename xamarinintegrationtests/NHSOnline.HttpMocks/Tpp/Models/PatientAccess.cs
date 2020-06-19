using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class PatientAccess
    {
        [XmlAttribute("patientId")]
        public string? PatientId { get; set; }

        public SiteDetails? SiteDetails { get; set; }
    }
}