using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class SiteDetails
    {
        [XmlAttribute("unitName")]
        public string? UnitName { get; set; }

        public Address? Address { get; set; }
    }
}