using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class PersonName
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}