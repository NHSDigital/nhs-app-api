using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class MaritalStatus 
    {
        [XmlAttribute(AttributeName="code")]
        public string Code { get; set; }
        
        [XmlText]
        public string Text { get; set; }
    }
}