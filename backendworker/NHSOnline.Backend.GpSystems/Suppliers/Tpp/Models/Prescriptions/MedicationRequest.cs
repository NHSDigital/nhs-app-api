using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions
{
    public class MedicationRequest
    {
        [XmlAttribute("drugId")]
        public string DrugId { get; set; }
        
        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}