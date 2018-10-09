using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class Name 
    {
        [XmlElement(ElementName="title", Namespace="urn:vision")]
        public string Title { get; set; }

        [XmlElement(ElementName="forename", Namespace="urn:vision")]
        public string Forename { get; set; }
        
        [XmlElement(ElementName="surname", Namespace="urn:vision")]
        public string Surname { get; set; }
    }
}