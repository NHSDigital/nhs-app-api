using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class MaritalStatus 
    {
        [XmlAttribute(AttributeName="code")]
        public string Code { get; set; }
        
        [XmlText]
        public string Text { get; set; }
    }
}