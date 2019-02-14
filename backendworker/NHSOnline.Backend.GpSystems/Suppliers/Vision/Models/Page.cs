using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class Page
    {
        [XmlElement(ElementName = "number")]
        public int Number { get; set; }
        
        [XmlElement(ElementName = "slotsPerPage")]
        public int SlotsPerPage { get; set; }
    }
}