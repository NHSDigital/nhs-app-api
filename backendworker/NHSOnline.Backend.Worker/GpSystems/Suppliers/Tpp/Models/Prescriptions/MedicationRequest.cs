using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class MedicationRequest
    {
        [XmlAttribute("drugId")]
        public string DrugId { get; set; }
        
        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}