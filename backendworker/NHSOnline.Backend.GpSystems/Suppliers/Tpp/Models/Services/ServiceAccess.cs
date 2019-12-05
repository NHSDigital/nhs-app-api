using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services
{
    public class ServiceAccess
    {
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("serviceIdentifier")]
        public string ServiceIdentifier { get; set; }
        [XmlAttribute("status")]
        public string Status { get; set; }
        [XmlAttribute("statusDesc")]
        public string StatusDesc { get; set; }
        [XmlAttribute("readOnly")]
        public string ReadOnly { get; set; }
    }
}