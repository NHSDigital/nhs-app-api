using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class RequestPatientRecordItem
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        
        [XmlAttribute("details")]
        public string Details { get; set; }
    }
}