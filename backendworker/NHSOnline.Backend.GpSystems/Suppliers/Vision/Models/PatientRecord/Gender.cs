using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlRoot(ElementName="gender", Namespace="urn:vision")]
    public class Gender 
    {   
        [XmlText]
        public string Text { get; set; }
    }
}