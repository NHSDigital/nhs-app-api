using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class Page
    {
        [XmlElement(ElementName = "number")]
        public int Number { get; set; }
        
        [XmlElement(ElementName = "slotsPerPage")]
        public int SlotsPerPage { get; set; }
    }
}