using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class NationalId
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        
        public string Value { get; set; }
    }
}